using System.Text.Json.Serialization;

namespace UpcloudApiKubernetesOperator.UpCloudApi.ObjectStorageV2.Models.Responses;
public readonly struct BucketMetric
{
    [JsonPropertyName("name")]
    public readonly string Name;

    [JsonPropertyName("total_objects")]
    public readonly int TotalObjects;

    [JsonPropertyName("total_size_bytes")]
    public readonly int TotalSizeBytes;

    [JsonConstructor]
    public BucketMetric(string name, int totalObjects, int totalSizeBytes) => (Name, TotalObjects, TotalSizeBytes) = (name, totalObjects, totalSizeBytes);
}
