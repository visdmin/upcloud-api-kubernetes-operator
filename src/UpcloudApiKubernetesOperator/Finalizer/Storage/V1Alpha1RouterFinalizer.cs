using k8s.Models;
using KubeOps.Operator.Finalizer;

using UpcloudApiKubernetesOperator.Entities;

namespace UpcloudApiKubernetesOperator.Finalizer;

public class V1Alpha1RouterFinalizer : IResourceFinalizer<V1Alpha1Router>
{
    private readonly ILogger<V1Alpha1RouterFinalizer> _logger;

    public V1Alpha1RouterFinalizer(ILogger<V1Alpha1RouterFinalizer> logger) => _logger = logger;

    public Task FinalizeAsync(V1Alpha1Router entity)
    {
        _logger.LogInformation($"entity {entity.Name()} called {nameof(FinalizeAsync)}.");
        return Task.CompletedTask;
    }
}
