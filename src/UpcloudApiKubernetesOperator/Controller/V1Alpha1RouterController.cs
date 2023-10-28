using System.Text.Json;

using k8s;
using k8s.Models;
using KubeOps.Operator.Controller;
using KubeOps.Operator.Controller.Results;
using KubeOps.Operator.Finalizer;
using KubeOps.Operator.Kubernetes;
using KubeOps.Operator.Rbac;

using UpcloudApiKubernetesOperator.Finalizer;
using UpcloudApiKubernetesOperator.Entities;
using KubeOps.KubernetesClient;


namespace UpcloudApiKubernetesOperator.Controller.Storage;

[EntityRbac(typeof(V1Alpha1Router), Verbs = RbacVerb.All)]
public class V1Alpha1RouterController : IResourceController<V1Alpha1Router>
{
    private readonly IKubernetesClient KubernetesClient;
    private readonly ILogger<V1Alpha1RouterController> Logger;
    private readonly IFinalizerManager<V1Alpha1Router> FinalizerManager;
    private readonly static TimeSpan ResourceInterval = TimeSpan.FromSeconds(30);

    public V1Alpha1RouterController(
        IKubernetesClient kubernetesClient,
        ILogger<V1Alpha1RouterController> logger,
        IFinalizerManager<V1Alpha1Router> finalizerManager
    ) => (KubernetesClient, Logger, FinalizerManager) = (kubernetesClient, logger, finalizerManager);

    public async Task<ResourceControllerResult?> ReconcileAsync(V1Alpha1Router entity)
    {
        Logger.LogInformation($"entity {entity.Name()} called {nameof(ReconcileAsync)}.");
        await FinalizerManager.RegisterFinalizerAsync<V1Alpha1RouterFinalizer>(entity);

        var options = new JsonSerializerOptions { WriteIndented = true };

        Logger.LogInformation(JsonSerializer.Serialize(entity.Spec, options: options));
        Logger.LogInformation(JsonSerializer.Serialize(entity.Status, options: options));



        return ResourceControllerResult.RequeueEvent(ResourceInterval);
    }

    public Task StatusModifiedAsync(V1Alpha1Router entity)
    {
        Logger.LogInformation($"entity {entity.Name()} called {nameof(StatusModifiedAsync)}.");

        return Task.CompletedTask;
    }

    public Task DeletedAsync(V1Alpha1Router entity)
    {
        Logger.LogInformation($"entity {entity.Name()} called {nameof(DeletedAsync)}.");

        return Task.CompletedTask;
    }
}
