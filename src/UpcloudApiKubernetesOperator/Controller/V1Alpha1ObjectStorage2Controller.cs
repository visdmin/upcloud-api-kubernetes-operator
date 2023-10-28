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
using IdentityModel.Client;


namespace UpcloudApiKubernetesOperator.Controller.Storage;

[EntityRbac(typeof(V1Alpha1ObjectStorage2), Verbs = RbacVerb.All)]
public class V1Alpha1ObjectStorage2Controller : IResourceController<V1Alpha1ObjectStorage2>
{
    private readonly IKubernetesClient KubernetesClient;
    private readonly HttpClient HttpClient;
    private readonly ILogger<V1Alpha1ObjectStorage2Controller> Logger;
    private readonly IFinalizerManager<V1Alpha1ObjectStorage2> FinalizerManager;
    private readonly static TimeSpan ResourceInterval = TimeSpan.FromSeconds(30);
    private readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };

    public V1Alpha1ObjectStorage2Controller(
        IKubernetesClient kubernetesClient,
        IHttpClientFactory httpClientFactory,
        ILogger<V1Alpha1ObjectStorage2Controller> logger,
        IFinalizerManager<V1Alpha1ObjectStorage2> finalizerManager
    ) {
        KubernetesClient = kubernetesClient;
        HttpClient       = httpClientFactory.CreateClient();
        Logger           = logger;
        FinalizerManager = finalizerManager;
    }

    public async Task<ResourceControllerResult?> ReconcileAsync(V1Alpha1ObjectStorage2 entity)
    {
        Logger.LogInformation("Reconciling (entity: {entity}, spec: {entitySpec}, status: {entityStatus})",
            new { kind = entity.GetType(), name = entity.Name() },
            JsonSerializer.Serialize(entity.Spec,   options: JsonSerializerOptions),
            JsonSerializer.Serialize(entity.Status, options: JsonSerializerOptions)
        );

        // TODO: Research what is the best way to obtain required api credentials from a secret.
        //       Maybe during the operator startup ?
        // KubernetesClient.Get<V1Secret>()

        await FinalizerManager.RegisterFinalizerAsync<V1Alpha1ObjectStorage2Finalizer>(entity);

        return ResourceControllerResult.RequeueEvent(ResourceInterval);
    }

    public Task StatusModifiedAsync(V1Alpha1ObjectStorage2 entity)
    {
        Logger.LogInformation($"entity {entity.Name()} called {nameof(StatusModifiedAsync)}.");
        return Task.CompletedTask;
    }

    public Task DeletedAsync(V1Alpha1ObjectStorage2 entity)
    {
        Logger.LogInformation($"entity {entity.Name()} called {nameof(DeletedAsync)}.");

        return Task.CompletedTask;
    }
}
