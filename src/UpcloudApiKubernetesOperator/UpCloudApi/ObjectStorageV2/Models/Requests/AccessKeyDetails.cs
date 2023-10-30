using System.Text.Json.Serialization;

namespace UpcloudApiKubernetesOperator.UpCloudApi.ObjectStorageV2.Models.Requests;
public readonly struct AccessKeyDetails
{
    [JsonPropertyName("name")]
    public readonly string Name;

    [JsonPropertyName("enabled")]
    public readonly bool Enabled;

    [JsonConstructor]
    public AccessKeyDetails(string name, bool enabled) => (Name, Enabled) = (name, enabled);
}
