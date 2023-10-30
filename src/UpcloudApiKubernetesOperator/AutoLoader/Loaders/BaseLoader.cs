using KubeOps.KubernetesClient;
using Microsoft.Extensions.Options;
using UpcloudApiKubernetesOperator.AutoLoader.Options;

namespace UpcloudApiKubernetesOperator.AutoLoader.Loaders;

internal abstract class BaseLoader : ILoader
{
    protected readonly AutoLoaderOptions Options;
    protected readonly IKubernetesClient KubernetesClient;
    protected readonly ILogger Logger;

    public BaseLoader(IOptions<AutoLoaderOptions> options, IKubernetesClient kubernetesClient, ILogger<AutoLoaderService> logger) =>
        (Options, KubernetesClient, Logger) = (options.Value, kubernetesClient, logger);

    public abstract Task Run(CancellationToken cancellationToken);
}
