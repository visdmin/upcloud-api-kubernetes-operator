using System.Text.Json.Serialization;

namespace UpcloudApiKubernetesOperator.UpCloudApi.ObjectStorageV2.Models.Responses;
public readonly struct NewAccessKeyDetails
{
    [JsonPropertyName("access_key_id")]
    public readonly string AccessKeyId;

    [JsonPropertyName("created_at")]
    public readonly DateTime CreatedAt;

    [JsonPropertyName("enabled")]
    public readonly bool Enabled;

    [JsonPropertyName("last_used_at")]
    public readonly DateTime LastUsedAt;

    [JsonPropertyName("name")]
    public readonly string Name;

    [JsonPropertyName("secret_access_key")]
    public readonly string Secret;

    [JsonPropertyName("updated_at")]
    public readonly DateTime UpdatedAt;

    [JsonConstructor]
    public NewAccessKeyDetails(
        string accessKeyId,
        DateTime createdAt,
        bool enabled,
        DateTime lastUsedAt,
        string name,
        string secret,
        DateTime updatedAt
    ) =>
        (AccessKeyId, CreatedAt, Enabled, LastUsedAt, Name, Secret, UpdatedAt) =
        (accessKeyId, createdAt, enabled, lastUsedAt, name, secret, updatedAt);

}
