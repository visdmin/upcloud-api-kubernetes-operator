using k8s.Models;
using KubeOps.Operator.Finalizer;

using UpcloudApiKubernetesOperator.Entities;

namespace UpcloudApiKubernetesOperator.Finalizer;

public class V1Alpha1ObjectStorage2Finalizer : IResourceFinalizer<V1Alpha1ObjectStorage2>
{
    private readonly ILogger<V1Alpha1ObjectStorage2Finalizer> _logger;

    public V1Alpha1ObjectStorage2Finalizer(ILogger<V1Alpha1ObjectStorage2Finalizer> logger) => _logger = logger;

    public Task FinalizeAsync(V1Alpha1ObjectStorage2 entity)
    {
        _logger.LogInformation($"entity {entity.Name()} called {nameof(FinalizeAsync)}.");
        return Task.CompletedTask;
    }
}
