using System.Collections.ObjectModel;

using Microsoft.Extensions.Options;

using k8s.Models;
using k8s.Autorest;
using KubeOps.KubernetesClient;

using UpcloudApiKubernetesOperator.AutoLoader.Options;
using UpcloudApiKubernetesOperator.Entities;
using UpcloudApiKubernetesOperator.UpCloudApi.ObjectStorageV2;
using UpcloudApiKubernetesOperator.UpCloudApi.ObjectStorageV2.Models.Responses;

namespace UpcloudApiKubernetesOperator.AutoLoader.Loaders;

internal class ObjectStorageV2Loader : BaseLoader
{
    protected readonly IObjectStorageV2Client UpCApiClient;

    public ObjectStorageV2Loader(IOptions<AutoLoaderOptions> options, IObjectStorageV2Client upcApiClient, IKubernetesClient kubernetesClient, ILogger<AutoLoaderService> logger)
        : base(options, kubernetesClient, logger)
        => UpCApiClient = upcApiClient;

    public override async Task Run(CancellationToken cancellationToken)
    {
        Logger.LogDebug("Loading object storage instances from upc api");

        var instancesFromApi = await UpCApiClient.ListInstances(cancellationToken: cancellationToken);

        if (instancesFromApi is null || instancesFromApi.Count == 0) {
            Logger.LogDebug("Loading object storage instances from upc api completed, no instances found");
            return;
        }

        var instancesFromK8s = await KubernetesClient.List<V1Alpha1ObjectStorage2>(@namespace: Options.Namespace, labelSelector: null);

        Collection<InstanceDetailsResponse>? instancesToCreate = null;
        foreach (var instanceFromApi in instancesFromApi) {
            if (DoesInstanceExistInK8s(in instancesFromK8s, in instanceFromApi)) {
                Logger.LogDebug("Loading object storage instance list from upc api, already existing instance skipped (uuid: {instanceUuid})",
                    instanceFromApi.UUID
                );

                continue;
            }

            (instancesToCreate ??= new ()).Add(instanceFromApi);
        }

        if (instancesToCreate is null || instancesToCreate.Count == 0) {
            Logger.LogDebug("Loading object storage instances from upc api completed, no new instances found");
            return;
        }

        var created = 0;
        foreach (var newInstanceDetails in instancesToCreate) {
            var newResource = CreateNewResource(in newInstanceDetails);
            Logger.LogInformation("Loading object storage instances from upc api completed, creating new instance (uuid: {newInstanceUuid})",
                newResource.Spec.Id
            );

            try {
                _ = await KubernetesClient.Create(resource: newResource);

                Logger.LogInformation("Loading object storage instances from upc api completed, new instance created (uuid: {newInstanceUuid})",
                    newResource.Spec.Id
                );
                created += 1;
            }
            catch (HttpOperationException hex) {
                Logger.LogError(hex, "Loading object storage instances from upc api completed, creating new instance failed (uuid: {newInstanceUuid}, error: {errorContent})",
                    newResource.Spec.Id,
                    hex.Response.Content.AsSpan().ToString()
                );
            }
            catch (Exception ex) {
                Logger.LogError(ex, "Loading object storage instances from upc api completed, creating new instance failed (uuid: {newInstanceUuid})",
                    newResource.Spec.Id
                );
            }
        }

        Logger.LogDebug("Loading object storage instances from upc api completed (created: {createdCount})",
            created
        );
    }

    private static bool DoesInstanceExistInK8s(in IList<V1Alpha1ObjectStorage2> instancesFromK8s, in InstanceDetailsResponse instancesFromApi)
    {
        for (var idx = 0; idx < instancesFromK8s.Count; idx++) {
            if (instancesFromK8s[idx].Status.Id == instancesFromApi.UUID) {
                return true;
            }
        }

        return false;
    }

    private V1Alpha1ObjectStorage2 CreateNewResource(in InstanceDetailsResponse newInstanceDetails) => new()
        {
            Spec       = CreateNewInstanceSpec(in newInstanceDetails),
            Status     = new (),
            Metadata   = new V1ObjectMeta {
                Name              = string.Format("object-storage-v2-{0}", newInstanceDetails.UUID),
                NamespaceProperty = Options.Namespace
            }
        };

    private static V1Alpha1ObjectStorage2.V1Alpha1ObjectStorageSpec CreateNewInstanceSpec(in InstanceDetailsResponse newInstanceDetails) => new()
        {
            Id               = newInstanceDetails.UUID,
            Region           = newInstanceDetails.Region,
            ConfiguredStatus = newInstanceDetails.ConfiguredStatus,
            Networks         = new (
                newInstanceDetails.Networks.Select(x => new V1Alpha1ObjectStorage2.Network {
                    Name   = x.Name,
                    Family = x.Family,
                    Type   = x.Type,
                    Id     = x.UUID
                }).ToList()
            ),
            Users            = new (
                newInstanceDetails.Users.Select(x => new V1Alpha1ObjectStorage2.User {
                    Username = x.Username
                }).ToList()
            ),
            Labels           = new (
                newInstanceDetails.Labels.Select(x => new V1Alpha1ObjectStorage2.Label {
                    Name  = x.Key,
                    Value = x.Value
                }).ToList()
            )
        };
}
