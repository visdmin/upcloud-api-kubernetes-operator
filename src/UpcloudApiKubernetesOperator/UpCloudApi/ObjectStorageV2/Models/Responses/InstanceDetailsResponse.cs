using System.Text.Json.Serialization;

namespace UpcloudApiKubernetesOperator.UpCloudApi.ObjectStorageV2.Models.Responses;

public readonly struct InstanceDetailsResponse
{
    [JsonPropertyName("uuid")]
    public readonly string UUID;

    [JsonPropertyName("configured_status")]
    public readonly string ConfiguredStatus;

    [JsonPropertyName("operational_state")]
    public readonly string OperationalState;

    [JsonPropertyName("region")]
    public readonly string Region;

    [JsonPropertyName("created_at")]
    public readonly DateTime CreatedAt;

    [JsonPropertyName("updated_at")]
    public readonly DateTime UpdatedAt;

    [JsonPropertyName("endpoints")]
    public readonly IReadOnlyCollection<Endpoint> Endpoints;

    [JsonPropertyName("labels")]
    public readonly IReadOnlyCollection<Label> Labels;

    [JsonPropertyName("networks")]
    public readonly IReadOnlyCollection<Network> Networks;

    [JsonPropertyName("users")]
    public readonly IReadOnlyCollection<User> Users;

    [JsonConstructor]
    public InstanceDetailsResponse(
        string uuid,
        string configuredStatus,
        string operationalState,
        string region,
        DateTime createdAt,
        DateTime updatedAt,
        IReadOnlyCollection<Endpoint> endpoints,
        IReadOnlyCollection<Label> labels,
        IReadOnlyCollection<Network> networks,
        IReadOnlyCollection<User> users
    ) =>
    (UUID, ConfiguredStatus, OperationalState, Region, CreatedAt, UpdatedAt, Endpoints, Labels, Networks, Users) =
    (uuid, configuredStatus, operationalState, region, createdAt, updatedAt, endpoints, labels, networks, users);
}

public readonly struct Endpoint
{
    [JsonPropertyName("type")]
    public readonly string Type;

    [JsonPropertyName("domain_name")]
    public readonly string DomainName;

    [JsonConstructor]
    public Endpoint(string type, string domainName) => (Type, DomainName) = (type, domainName);
}

public readonly struct Network
{
    [JsonPropertyName("uuid")]
    public readonly string? UUID;

    [JsonPropertyName("name")]
    public readonly string Name;

    [JsonPropertyName("family")]
    public readonly string Family;

    [JsonPropertyName("type")]
    public readonly string Type;

    [JsonConstructor]
    public Network(string? uuid, string name, string family, string type) => (UUID, Name, Family, Type) = (uuid, name, family, type);
}

public readonly struct User
{
    [JsonPropertyName("username")]
    public readonly string Username;

    [JsonPropertyName("operational_state")]
    public readonly string OperationalState;

    [JsonPropertyName("created_at")]
    public readonly DateTime CreatedAt;

    [JsonPropertyName("updated_at")]
    public readonly DateTime UpdatedAt;

    [JsonPropertyName("access_keys")]
    public readonly IReadOnlyCollection<AccessKey> AccessKeys;

    [JsonConstructor]
    public User(
        string username,
        string operationalState,
        DateTime createdAt,
        DateTime updatedAt,
        IReadOnlyCollection<AccessKey> accessKeys
    ) => (Username, OperationalState, CreatedAt, UpdatedAt, AccessKeys) = (username, operationalState, createdAt, updatedAt, accessKeys);

}

public readonly struct AccessKey
{
    [JsonPropertyName("access_key_id")]
    public readonly string Id;

    [JsonPropertyName("created_at")]
    public readonly DateTime CreatedAt;

    [JsonPropertyName("enabled")]
    public readonly bool Enabled;

    [JsonPropertyName("last_used_at")]
    public readonly DateTime LastUsedAt;

    [JsonPropertyName("name")]
    public readonly string Name;

    [JsonPropertyName("updated_at")]
    public readonly DateTime UpdatedAt;

    [JsonConstructor]
    public AccessKey(
        string id,
        DateTime createdAt,
        bool enabled,
        DateTime lastUsedAt,
        string name,
        DateTime updatedAt
    ) => (Id, CreatedAt, Enabled, LastUsedAt, Name, UpdatedAt) = (id, createdAt, enabled, lastUsedAt, name, updatedAt);
}
