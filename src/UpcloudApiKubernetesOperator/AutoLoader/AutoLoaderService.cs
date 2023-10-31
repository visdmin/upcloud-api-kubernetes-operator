using System.Collections.ObjectModel;
using Microsoft.Extensions.Options;
using UpcloudApiKubernetesOperator.AutoLoader.Loaders;
using UpcloudApiKubernetesOperator.AutoLoader.Options;

namespace UpcloudApiKubernetesOperator.AutoLoader;

internal class AutoLoaderService : IHostedService
{
    private readonly ReadOnlyCollection<ILoader> Loaders;
    private readonly ILogger Logger;
    private readonly AutoLoaderOptions Options;
    private Task? Runner;
    private readonly CancellationTokenSource CancellationTokenSource = new();

    public AutoLoaderService(
        ObjectStorageV2Loader objectStorageV2Loader,
        IOptions<AutoLoaderOptions> options,
        ILogger<AutoLoaderService> logger
    ) {
        Loaders = new (
            new ILoader[] {
                objectStorageV2Loader
            }
        );
        Options = options.Value;
        Logger  = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Runner = RunLoaders();
        Logger.LogInformation("UpCloud entity auto loader enabled (interval: {refreshInterval})", Options.RefreshInterval);
        return Task.CompletedTask;
    }

    private async Task RunLoaders()
    {
        var interval = TimeSpan.FromSeconds(Options.RefreshInterval);

        while (CancellationTokenSource.IsCancellationRequested is false) {
            await Task.Delay(interval, cancellationToken: CancellationTokenSource.Token);

            foreach (var loader in Loaders) {
                try {
                    await loader.Run(CancellationTokenSource.Token);
                }
                catch (Exception ex) {
                    Logger.LogError(ex, "Executing loader failed (type: {loaderType})", loader.GetType().Name);
                }
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        CancellationTokenSource.Cancel();

        if (Runner is not null && Runner.IsCompleted is false) {
            try {
                await Runner;
            }
            catch (TaskCanceledException) { }
            Runner = null;
        }

        CancellationTokenSource.Dispose();
    }
}
