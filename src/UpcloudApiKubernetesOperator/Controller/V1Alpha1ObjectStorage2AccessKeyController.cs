using System.Text.Json;
using System.Collections.ObjectModel;

using k8s.Models;
using KubeOps.Operator.Controller;
using KubeOps.Operator.Controller.Results;
using KubeOps.Operator.Finalizer;
using KubeOps.Operator.Rbac;
using KubeOps.KubernetesClient;

using UpcloudApiKubernetesOperator.Finalizer;
using UpcloudApiKubernetesOperator.Entities;
using UpcloudApiKubernetesOperator.UpCloudApi.ObjectStorageV2;
using UpcloudApiKubernetesOperator.UpCloudApi.ObjectStorageV2.Models.Requests;
using UpcloudApiKubernetesOperator.UpCloudApi.ObjectStorageV2.Models.Responses;
using System.Threading.Tasks.Dataflow;
using Microsoft.IdentityModel.Tokens;
using System.Data;

namespace UpcloudApiKubernetesOperator.Controller.Storage;

[EntityRbac(typeof(V1Alpha1ObjectStorage2), Verbs = RbacVerb.All)]
public class V1Alpha1ObjectStorage2Controller : IResourceController<V1Alpha1ObjectStorage2>
{
    private readonly IKubernetesClient KubernetesClient;
    private readonly IObjectStorageV2Client UpcApiClient;
    private readonly ILogger<V1Alpha1ObjectStorage2Controller> Logger;
    private readonly IFinalizerManager<V1Alpha1ObjectStorage2> FinalizerManager;
    private readonly static TimeSpan ReconcileInterval = TimeSpan.FromSeconds(30);
    private readonly JsonSerializerOptions SerializerOptions;

    public V1Alpha1ObjectStorage2Controller(
        IKubernetesClient kubernetesClient,
        IObjectStorageV2Client upcApiClient,
        ILogger<V1Alpha1ObjectStorage2Controller> logger,
        IFinalizerManager<V1Alpha1ObjectStorage2> finalizerManager
    ) {
        KubernetesClient  = kubernetesClient;
        UpcApiClient      = upcApiClient;
        Logger            = logger;
        FinalizerManager  = finalizerManager;
        SerializerOptions = new JsonSerializerOptions {
            IgnoreReadOnlyFields     = false,
            IgnoreReadOnlyProperties = false,
            WriteIndented            = true
        };
    }

    public async Task<ResourceControllerResult?> ReconcileAsync(V1Alpha1ObjectStorage2 entity)
    {
        if (entity.Status.ReconcileState is V1Alpha1ObjectStorage2.ReconcileState.FAILURE or V1Alpha1ObjectStorage2.ReconcileState.PAUSED) {
            Logger.LogInformation("Reconciling skipped (entity: {entity}, spec: {entitySpec}, status: {entityStatus})",
                new { kind = entity.GetType(), name = entity.Name(), reconcileStatus = entity.Status.ReconcileState },
                JsonSerializer.Serialize(entity.Spec,   options: SerializerOptions),
                JsonSerializer.Serialize(entity.Status, options: SerializerOptions)
            );
        }

        await FinalizerManager.RegisterFinalizerAsync<V1Alpha1ObjectStorage2Finalizer>(entity);

        Logger.LogInformation("Reconciling (entity: {entity}, spec: {entitySpec}, status: {entityStatus})",
            new { kind = entity.GetType(), name = entity.Name() },
            JsonSerializer.Serialize(entity.Spec,   options: SerializerOptions),
            JsonSerializer.Serialize(entity.Status, options: SerializerOptions)
        );

        // Currently object storage v2 api does not support creation of multiple instances
        // TODO: update this logic to set instance uuid to the status when API is updated.
        if (string.IsNullOrEmpty(entity.Status.Id) && string.IsNullOrEmpty(entity.Spec.Id)) {
            await CreateInstance(entity);
            return ResourceControllerResult.RequeueEvent(ReconcileInterval);
        }

        var instanceDetails = await UpcApiClient.GetDetails(instanceUuid: entity.Spec.Id);
        if (instanceDetails is null) {
            Logger.LogWarning("Entity deleted because not found in upc api (entity: {entity}, spec: {entitySpec}, status: {entityStatus})",
                new { kind = entity.GetType(), name = entity.Name(), reconcileStatus = entity.Status.ReconcileState },
                JsonSerializer.Serialize(entity.Spec,   options: SerializerOptions),
                JsonSerializer.Serialize(entity.Status, options: SerializerOptions)
            );
            await KubernetesClient.Delete(entity);
            return ResourceControllerResult.RequeueEvent(ReconcileInterval);
        }

        if (CheckIfUpdateIsRequired(entity, instanceDetails.Value)) {
            await UpdateStatus(V1Alpha1ObjectStorage2.ReconcileState.UPDATING, entity, instanceDetails.Value);
            await TryUpdateInstance(entity);
            return ResourceControllerResult.RequeueEvent(ReconcileInterval);
        }

        await UpdateStatus(V1Alpha1ObjectStorage2.ReconcileState.UP_TO_DATE, entity, instanceDetails.Value);
        return ResourceControllerResult.RequeueEvent(ReconcileInterval);
    }

    private async Task CreateInstance(V1Alpha1ObjectStorage2 entity)
    {
        var response = await UpcApiClient.CreateInstance(
            instanceDetails: InstanceDetails.FromKubernetesSpec(entity.Spec)
        );

        if (response.Success is false) {
            Logger.LogWarning("Creating object storage instance failed, setting reconcile status to failed (error: {errorResponse})",
                (response.ErrorResponse is not null) ? JsonSerializer.Serialize(response.ErrorResponse) : "n/a"
            );

            entity.Status.ReconcileState = V1Alpha1ObjectStorage2.ReconcileState.FAILURE;
            await KubernetesClient.UpdateStatus(entity);
            return;
        }

        Logger.LogInformation("Object storage v2 instance created successfully");
        return;
    }

    private async Task TryUpdateInstance(V1Alpha1ObjectStorage2 entity)
    {
        var response = await UpcApiClient.ReplaceInstance(
            instanceUuid:    entity.Spec.Id,
            instanceDetails: InstanceDetails.FromKubernetesSpec(entity.Spec)
        );

        if (response.Success is false) {
            Logger.LogWarning("Updating object storage instance failed, setting reconcile status to failed (error: {errorResponse})",
                (response.ErrorResponse is not null) ? JsonSerializer.Serialize(response.ErrorResponse) : "n/a"
            );

            entity.Status.ReconcileState = V1Alpha1ObjectStorage2.ReconcileState.FAILURE;
            await KubernetesClient.UpdateStatus(entity);
            return;
        }

        Logger.LogInformation("Updating storage v2 instance created successfully");
        return;
    }

    private static bool CheckIfUpdateIsRequired(V1Alpha1ObjectStorage2 entity, InstanceDetailsResponse state)
    {
        if (state.ConfiguredStatus != entity.Spec.ConfiguredStatus) {
            return true;
        }

        if (state.Region != entity.Spec.Region) {
            return true;
        }

        if (state.Networks.Count != entity.Spec.Networks.Count) {
            return true;
        }

        foreach (var network in state.Networks) {
            var equal = entity.Spec.Networks.Where(x =>
                network.Name   == x.Name   && network.Type == x.Type &&
                network.Family == x.Family && (network.UUID is null) || network.UUID == x.Id
            );

            if (equal.IsNullOrEmpty()) {
                return true;
            }
        }

        if (state.Labels.Count != entity.Spec.Labels.Count) {
            return true;
        }

        foreach (var label in state.Labels) {
            var equal = entity.Spec.Labels.Where(x => label.Key == x.Name && label.Value == x.Value);
            if (equal.IsNullOrEmpty()) {
                return true;
            }
        }

        if (state.Users.Count != entity.Spec.Users.Count) {
            return true;
        }

        foreach (var user in state.Users) {
            var equal = entity.Spec.Users.Where(x => user.Username == x.Username);
            if (equal.IsNullOrEmpty()) {
                return true;
            }
        }

        return false;
    }

    public Task UpdateStatus(V1Alpha1ObjectStorage2.ReconcileState reconcileState, V1Alpha1ObjectStorage2 entity, InstanceDetailsResponse state)
    {
        entity.Status = new V1Alpha1ObjectStorage2.V1Alpha1ObjectStorageStatus {
            ReconcileState   = reconcileState,
            Id               = state.UUID,
            ConfiguredStatus = state.ConfiguredStatus,
            CreatedAt        = state.CreatedAt.ToString("s"),
            Endpoints        = new (
                state.Endpoints.Select(x => new V1Alpha1ObjectStorage2.Endpoint {
                    DomainName = x.DomainName,
                    Type       = x.Type
                }).ToList()
            ),
            Networks         = new (
                state.Networks.Select(x => new V1Alpha1ObjectStorage2.Network {
                    Name   = x.Name,
                    Family = x.Family,
                    Type   = x.Type,
                    Id     = x.UUID
                }).ToList()
            ),
            OperationalState = state.OperationalState,
            Region           = state.Region,
            UpdatedAt        = state.UpdatedAt.ToString("s"),
            Users            = new (
                state.Users.Select(x => new V1Alpha1ObjectStorage2.ConnectedUser{
                    Username         = x.Username,
                    OperationalState = x.OperationalState,
                    UpdatedAt        = x.UpdatedAt.ToString("s"),
                    CreatedAt        = x.CreatedAt.ToString("s"),
                    AccessKeys       = new (
                        x.AccessKeys.Select(x => new V1Alpha1ObjectStorage2.AccessKey {
                            AccessKeyId = x.Id,
                            Name        = x.Name,
                            CreatedAt   = x.CreatedAt.ToString("s"),
                            LastUsedAt  = x.LastUsedAt.ToString("s"),
                            UpdatedAt   = x.UpdatedAt.ToString("s")
                        }).ToList()
                    )
                }).ToList()
            )
        };

        return KubernetesClient.UpdateStatus(entity);
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
