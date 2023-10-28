using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using k8s;
using k8s.Models;
using KubeOps.Operator.Entities;
using KubeOps.Operator.Entities.Annotations;

namespace UpcloudApiKubernetesOperator.Entities;

[KubernetesEntity(Group = "storage.upcloud.com", ApiVersion = "v1alpha1", Kind = "Storage")]
public class V1Alpha1Storage : CustomKubernetesEntity<V1Alpha1Storage.V1Alpha1StorageSpec, V1Alpha1Storage.V1Alpha1StorageStatus>
{
    [Description("Specification for UpCloud Storage entity: https://developers.upcloud.com/1.3/9-storages/#create-storage")]
    public class V1Alpha1StorageSpec
    {
        [Required]
        [Length(MinLength = 0, MaxLength = 255)]
        [Description("A short, informational description.")]
        public string Title                          { get; set; } = string.Empty;

        [Required]
        [RangeMinimum(Minimum = 1)]
        [RangeMaximum(Maximum = 4096)]
        [Description("The size of the storage in gigabytes.")]
        public int Size                              { get; set; } = default;

        [Required]
        [Length(MinLength = 1)]
        [Pattern("hdd|maxiops")]
        [Description("The storage tier to use. See Storage tiers: https://developers.upcloud.com/1.3/9-storages/#storage-tiers")]
        public string Tier                           { get; set; } = "hdd";

        [Required]
        [Length(MinLength = 1)]
        [Description("The zone in which the storage will be created, e.g. fi-hel1. See: https://developers.upcloud.com/1.3/5-zones/")]
        public string Zone                           { get; set; } = string.Empty;

        [Description("The backup_rule block defines when the storage device is backed up automatically.")]
        public BackupRule? BackupRule                { get; set; } = null;

        [Items(MinItems = -1, MaxItems = -1)]
        [Description("Labels describing the storage, objects with both 'key' and 'value' defined.")]
        public Collection<Label> Labels              { get; set; } = new ();
    }

    [Description("Backup rule for UpCloud storage entity")]
    public class BackupRule
    {
        [Required]
        [Description("The weekday when the backup is created. If daily is selected, backups are made every day at the same time. (daily / mon / tue / wed / thu / fri / sat / sun)")]
        public string Interval { get; set; }  = string.Empty;

        [Required]
        [Description("The time of day when the backup is created. (0000-2359)")]
        [Pattern("^[0-9]*$")]
        public string Time     { get; set; }  = string.Empty;

        [Required]
        [Description("The number of days before a backup is automatically deleted. The maximum retention period is three years (1095 days).")]
        [RangeMaximum(Maximum = 1095)]
        [RangeMinimum(Minimum = 1)]
        public int Retention   { get; set; }  = default;
    }

    [Description("Label for UpCloud storage entity")]
    public class Label
    {
        [Required]
        public string Name { get; set; }  = string.Empty;
        [Required]
        public string Value { get; set; } = string.Empty;
    }

    public class V1Alpha1StorageStatus
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
