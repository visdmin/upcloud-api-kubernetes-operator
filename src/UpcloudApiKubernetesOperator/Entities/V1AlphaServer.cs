using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using k8s;
using k8s.Models;
using KubeOps.Operator.Entities;
using KubeOps.Operator.Entities.Annotations;

namespace UpcloudApiKubernetesOperator.Entities;

[KubernetesEntity(Group = "storage.upcloud.com", ApiVersion = "v1alpha1", Kind = "Storage")]
public class V1Alpha1Server : CustomKubernetesEntity<V1Alpha1Server.V1Alpha1ServerSpec, V1Alpha1Server.V1Alpha1ServerStatus>
{
    [Description("Specification for UpCloud Storage entity: https://developers.upcloud.com/1.3/9-storages/#create-storage")]
    public class V1Alpha1ServerSpec
    {
        [Required]
        [Description("A short, informational description.")]
        [Length(MinLength = 0, MaxLength = 255)]
        public string Title                          { get; set; } = string.Empty;

        [Required]
        [Description("The zone in which the storage will be created, e.g. fi-hel1. See: https://developers.upcloud.com/1.3/5-zones/")]
        [Length(MinLength = 1)]
        public string Zone                           { get; set; } = string.Empty;

        [Required]
        [Description("Hostname of the server to be created")]
        [Length(MinLength = 1)]
        public string Hostname                       { get; set; } = string.Empty;

        [Required]
        [Description("Server plan name, see: https://developers.upcloud.com/1.3/8-servers/#list-server-configurations")]
        [Length(MinLength = 1)]
        public string Plan                            { get; set; } = string.Empty;

        [Description("Labels describing the server, objects with both 'key' and 'value' defined.")]
        [Items(MinItems = -1, MaxItems = -1)]
        public Collection<Label> Labels               { get; set; } = new ();

        [Required]
        [Description("Storage device collection of the server")]
        public StorageDevices StorageDevices          { get; set; } = new ();
    }

    [Description("Storage devices entry for server")]
    public class StorageDevices
    {
        [Required]
        [Description("Storage device collection")]
        public Collection<StorageDevice> StorageDevice { get; set; } = new ();
    }

    [Description("Storage device entry")]
    public class StorageDevice
    {
        [Required]
        [Length(MinLength = 1)]
        [Description("Storage device creation action")]
        public string Action { get; set; }  = string.Empty;

        [Required]
        [Length(MinLength = 1)]
        [Description("Storage device title")]
        public string Title { get; set; }  = string.Empty;

        [Required]
        [Description("The size of the storage in gigabytes.")]
        [RangeMinimum(Minimum = 1)]
        [RangeMaximum(Maximum = 4096)]
        public int Size { get; set; } = default;

        [Required]
        [Length(MinLength = 1)]
        [Pattern("hdd|maxiops")]
        [Description("Storage device title")]
        public string Tier { get; set; }  = string.Empty;

        [Description("Labels describing the storage, objects with both 'key' and 'value' defined.")]
        [Items(MinItems = -1, MaxItems = -1)]
        public Collection<Label> Labels { get; set; } = new ();
    }

    [Description("Label for UpCloud storage entity")]
    public class Label
    {
        [Required]
        public string Name { get; set; }  = string.Empty;
        [Required]
        public string Value { get; set; } = string.Empty;
    }

    public class V1Alpha1ServerStatus
    {
        [Description("Storage uuid")]
        [JsonPropertyName("id")]
        public string Identifier { get; init; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; init; }      = string.Empty;

        [JsonPropertyName("state")]
        public string State { get; init; }      = string.Empty;

        [Description("The size of the storage in gigabytes")]
        [JsonPropertyName("size")]
        public int Size { get; init; }          = default;

        [JsonPropertyName("type")]
        public string Type { get; init; }       = string.Empty;

        [JsonPropertyName("tier")]
        public string Tier { get; init; }       = string.Empty;

        [JsonPropertyName("zone")]
        public string Zone { get; init; }       = string.Empty;

        [JsonPropertyName("access")]
        public string Access { get; init; }     = string.Empty;
    }
}
