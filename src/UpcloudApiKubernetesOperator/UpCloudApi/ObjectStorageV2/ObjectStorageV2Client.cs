using System.Collections.ObjectModel;

using UpcloudApiKubernetesOperator.UpCloudApi.ObjectStorageV2.Models.Responses;
using UpcloudApiKubernetesOperator.UpCloudApi.ObjectStorageV2.Models.Requests;
using k8s.Models;

namespace UpcloudApiKubernetesOperator.UpCloudApi.ObjectStorageV2;

internal sealed class ObjectStorageV2Client : UpCloudApiClient, IObjectStorageV2Client
{
    public ObjectStorageV2Client(IHttpClientFactory httpClientFactory, ILogger<UpCloudApiClient> logger) : base(httpClientFactory, logger) { }

    public async Task<IReadOnlyCollection<InstanceDetailsResponse>> ListInstances(CancellationToken cancellationToken = default)
    {
        try {
            var response = await HttpClient.GetFromJsonAsync<InstanceDetailsResponse[]>(
                requestUri:        "/1.3/object-storage-2",
                options:           JsonSerializerOptions,
                cancellationToken: cancellationToken
            );

            return new ReadOnlyCollection<InstanceDetailsResponse>(response ?? Array.Empty<InstanceDetailsResponse>());
        }
        catch (Exception ex) {
            Logger.LogError(ex, "Requesting object storage v2 instance list from upc api failed");
            return new ReadOnlyCollection<InstanceDetailsResponse>(Array.Empty<InstanceDetailsResponse>());
        }
    }

    public async Task<InstanceDetailsResponse?> GetDetails(string instanceUuid, CancellationToken cancellationToken = default)
    {
        try {
            return await HttpClient.GetFromJsonAsync<InstanceDetailsResponse>(
                requestUri:        string.Format("/1.3/object-storage-2/{0}", instanceUuid),
                options:           JsonSerializerOptions,
                cancellationToken: cancellationToken
            );
        }
        catch (Exception ex) {
            Logger.LogError(ex, "Requesting object storage v2 details from upc api failed");
            return null;
        }
    }

    public async Task<IReadOnlyCollection<BucketMetric>> GetBucketMetrics(string instanceUuid, CancellationToken cancellationToken = default)
    {
        try {
            var response = await HttpClient.GetFromJsonAsync<BucketMetric[]>(
                requestUri:        string.Format("1.3/object-storage-2/{0}/metrics/buckets", instanceUuid),
                options:           JsonSerializerOptions,
                cancellationToken: cancellationToken
            );

            return new ReadOnlyCollection<BucketMetric>(response ?? Array.Empty<BucketMetric>());
        }
        catch (Exception ex) {
            Logger.LogError(ex, "Requesting object storage v2 bucket metrics from upc api failed");
            return new ReadOnlyCollection<BucketMetric>(Array.Empty<BucketMetric>());
        }
    }

    public async Task<CreateInstanceResponse> CreateInstance(InstanceDetails instanceDetails, CancellationToken cancellationToken = default)
    {
        try {
            var response = await HttpClient.PostAsJsonAsync<InstanceDetails>(
                requestUri:        "/1.3/object-storage-2",
                value:             instanceDetails,
                options:           JsonSerializerOptions,
                cancellationToken: cancellationToken
            );

            if (response.IsSuccessStatusCode) {
                return new (success: true, errorResponse: null);
            }

            return new (
                success:       true,
                errorResponse: await response.Content.ReadFromJsonAsync<ErrorResponse>(
                    options: JsonSerializerOptions,
                    cancellationToken: cancellationToken
                )
            );
        }
        catch (Exception ex) {
            Logger.LogError(ex, "Create new object storage 2 instance request failed");
            return new (success: false, errorResponse: null);
        }
    }

    public async Task<ReplaceInstanceResponse> ReplaceInstance(string instanceUuid, InstanceDetails instanceDetails, CancellationToken cancellationToken = default)
    {
        try {
            var response = await HttpClient.PutAsJsonAsync<InstanceDetails>(
                requestUri:        string.Format("/1.3/object-storage-2/{0}", instanceUuid),
                value:             instanceDetails,
                options:           JsonSerializerOptions,
                cancellationToken: cancellationToken
            );

            if (response.IsSuccessStatusCode) {
                return new (success: true, errorResponse: null);
            }

            return new (
                success:       true,
                errorResponse: await response.Content.ReadFromJsonAsync<ErrorResponse>(
                    options: JsonSerializerOptions,
                    cancellationToken: cancellationToken
                )
            );
        }
        catch (Exception ex) {
            Logger.LogError(ex, "Replace new object storage 2 instance request failed");
            return new (success: false, errorResponse: null);
        }
    }

    public async Task<DeleteInstanceResponse> DeleteInstance(string instanceUuid, CancellationToken cancellationToken = default)
    {
        try {
            var response = await HttpClient.DeleteAsync(
                requestUri:        string.Format("/1.3/object-storage-2/{0}", instanceUuid),
                cancellationToken: cancellationToken
            );

            if (response.IsSuccessStatusCode) {
                return new (success: true, errorResponse: null);
            }

            return new (
                success:       true,
                errorResponse: await response.Content.ReadFromJsonAsync<ErrorResponse>(
                    options: JsonSerializerOptions,
                    cancellationToken: cancellationToken
                )
            );
        }
        catch (Exception ex) {
            Logger.LogError(ex, "Delete new object storage 2 instance request failed");
            return new (success: false, errorResponse: null);
        }
    }
}
