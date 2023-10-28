using k8s.Models;
using KubeOps.Operator.Finalizer;
using UpcloudApiKubernetesOperator.Entities;

namespace UpcloudApiKubernetesOperator.Finalizer;

public class V1Alpha1StorageFinalizer : IResourceFinalizer<V1Alpha1Storage>
{
    private readonly ILogger<V1Alpha1StorageFinalizer> _logger;

    public V1Alpha1StorageFinalizer(ILogger<V1Alpha1StorageFinalizer> logger) => _logger = logger;

    public Task FinalizeAsync(V1Alpha1Storage entity)
    {
        _logger.LogInformation($"entity {entity.Name()} called {nameof(FinalizeAsync)}.");
        return Task.CompletedTask;
    }
}
