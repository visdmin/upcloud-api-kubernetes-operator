using System.Text.Json.Serialization;

namespace UpcloudApiKubernetesOperator.UpCloudApi.ObjectStorageV2.Models.Responses;
public readonly struct Label
{
    [JsonPropertyName("key")]
    public readonly string Key;

    [JsonPropertyName("value")]
    public readonly string Value;

    [JsonConstructor]
    public Label(string key, string value) => (Key, Value) = (key, value);
}
