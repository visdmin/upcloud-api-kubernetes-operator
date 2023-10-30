using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

using UpcloudApiKubernetesOperator.Entities;

namespace UpcloudApiKubernetesOperator.UpCloudApi.ObjectStorageV2.Models.Requests;
public readonly struct InstanceDetails
{
    [JsonPropertyName("region")]
    public readonly string Region;

    [JsonPropertyName("configured_status")]
    public readonly string ConfiguredStatus;

    [JsonPropertyName("networks")]
    public readonly IReadOnlyCollection<Network> Networks;

    [JsonPropertyName("users")]
    public readonly IReadOnlyCollection<User> Users;

    [JsonPropertyName("labels")]
    public readonly IReadOnlyCollection<Label>? Labels;

    [JsonConstructor]
    public InstanceDetails(
        string region,
        string configuredStatus,
        IReadOnlyCollection<Network> networks,
        IReadOnlyCollection<User> users,
        IReadOnlyCollection<Label>? labels
    ) => (Region, ConfiguredStatus, Networks, Users, Labels) = (region, configuredStatus, networks, users, labels);

    public readonly struct Network
    {
        [JsonPropertyName("name")]
        public readonly string Name;

        [JsonPropertyName("uuid")]
        public readonly string? UUID;

        [JsonPropertyName("type")]
        public readonly string Type;

        [JsonPropertyName("family")]
        public readonly string Family;

        [JsonConstructor]
        public Network(string name, string? uuid, string type, string family) => (Name, UUID, Type, Family) = (name, uuid, type, family);
    }

    public readonly struct User
    {
        [JsonPropertyName("username")]
        public readonly string Username;

        [JsonConstructor]
        public User(string username) => Username = username;
    }

    public readonly struct Label
    {
        [JsonPropertyName("key")]
        public readonly string Key;

        [JsonPropertyName("value")]
        public readonly string Value;

        [JsonConstructor]
        public Label(string key, string value) => (Key, Value) = (key, value);
    }

    public static InstanceDetails FromKubernetesSpec(V1Alpha1ObjectStorage2.V1Alpha1ObjectStorageSpec spec) =>
        new (
            region:           spec.Region,
            configuredStatus: spec.ConfiguredStatus,
            networks:         new ReadOnlyCollection<Network>(
                spec.Networks.Select(x => new Network(
                    name:   x.Name,
                    uuid:   string.IsNullOrEmpty(x.Id) ? null : x.Id,
                    type:   x.Type,
                    family: x.Family
                )).ToArray()
            ),
            users:           new ReadOnlyCollection<User>(
                spec.Users.Select(
                    x => new User(username: x.Username)
                ).ToArray()
            ),
            labels:          spec.Labels.Count switch {
                0 => null,
                _ => new ReadOnlyCollection<Label>(
                    spec.Labels.Select(x => new Label(
                        key:   x.Name,
                        value: x.Value
                    )).ToArray()
                )
            }
        );
}
