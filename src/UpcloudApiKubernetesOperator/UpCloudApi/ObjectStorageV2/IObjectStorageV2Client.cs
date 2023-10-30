using UpcloudApiKubernetesOperator.UpCloudApi.ObjectStorageV2.Models.Responses;
using UpcloudApiKubernetesOperator.UpCloudApi.ObjectStorageV2.Models.Requests;

namespace UpcloudApiKubernetesOperator.UpCloudApi.ObjectStorageV2;

public interface IObjectStorageV2Client
{
    Task<IReadOnlyCollection<InstanceDetailsResponse>> ListInstances(CancellationToken cancellationToken = default);
    Task<InstanceDetailsResponse?> GetDetails(string instanceUuid, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<BucketMetric>> GetBucketMetrics(string instanceUuid, CancellationToken cancellationToken = default);
    Task<CreateInstanceResponse> CreateInstance(InstanceDetails instanceDetails, CancellationToken cancellationToken = default);
    Task<ReplaceInstanceResponse> ReplaceInstance(string instanceUuid, InstanceDetails instanceDetails, CancellationToken cancellationToken = default);
    Task<DeleteInstanceResponse> DeleteInstance(string instanceUuid, CancellationToken cancellationToken = default);
    Task<CreateAccessKeyResponse> CreateAccessKey(string instanceUuid, string username, AccessKeyDetails details, CancellationToken cancellationToken = default);
}
