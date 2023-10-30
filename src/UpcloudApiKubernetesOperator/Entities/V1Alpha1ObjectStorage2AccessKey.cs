using System.Collections.ObjectModel;

using k8s.Models;
using KubeOps.Operator.Entities;
using KubeOps.Operator.Entities.Annotations;

namespace UpcloudApiKubernetesOperator.Entities;

[KubernetesEntity(Group = "api.upcloud.com", ApiVersion = "v1alpha1", Kind = "ObjectStorage2AccessKey")]
public class V1Alpha1ObjectStorage2AccessKey : CustomKubernetesEntity<V1Alpha1ObjectStorage2AccessKey.V1Alpha1ObjectStorageSpec, V1Alpha1ObjectStorage2AccessKey.V1Alpha1ObjectStorageStatus>
{
    public const string KubeApiVersion = "api.upcloud.com/v1alpha1";
    public const string KubeKind       = "ObjectStorage2AccessKey";

    public V1Alpha1ObjectStorage2AccessKey()
    {
        ApiVersion = KubeApiVersion;
        Kind       = KubeKind;
    }

    [Description("Specification for UpCloud object storage v2 access key entity: https://developers.upcloud.com/1.3/21-managed-object-storage/#create-access-key")]
    public class V1Alpha1ObjectStorageSpec
    {
        [Required]
        [Description("Reference to object storage 2 instance")]
        [EmbeddedResource]
        public ObjectStorage2Reference ObjectStorage2Reference { get; set; } = new ();

        [Required]
        [Length(MaxLength = 64, MinLength = 1)]
        [Pattern("^[a-zA-Z0-9_-]+$")]
        [Description("Access key name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Length(MaxLength = -1, MinLength = 1)]
        [Description("Valid cloud user username, that has access to the object storage 2 instance")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [Description("Access key state")]
        public bool Enabled { get; set; } = true;
    }

    [Description("Reference to object storage v2 entity")]
    public class ObjectStorage2Reference
    {
        [Required]
        [Length(MaxLength = 36, MinLength = -1)]
        [Description("UUID of existing object storage instance")]
        public string Id { get; set; } = string.Empty;

        [Required]
        [Description("Kubernetes namespace where the object storage v2 entity exists")]
        public string Namespace { get; set; } = string.Empty;
    }

    public class V1Alpha1ObjectStorageStatus
    {
        [AdditionalPrinterColumn]
        [Description("Entity reconcile status")]
        public ReconcileState ReconcileState { get; set; } = ReconcileState.UPDATING;

        [AdditionalPrinterColumn]
        [Description("Object storage 2 instance uuid")]
        public string ObjectStorage2Uuid { get; set; } = string.Empty;

        [AdditionalPrinterColumn]
        [Description("Access key state")]
        public bool Enabled { get; set; } = true;

        [AdditionalPrinterColumn]
        [Description("Access key name")]
        public string Name { get; set; } = string.Empty;

        [AdditionalPrinterColumn]
        [Description("Valid cloud user username")]
        public string Username { get; set; } = string.Empty;

        [AdditionalPrinterColumn]
        [Description("Access key id")]
        public string AccessKeyId { get; set; } = string.Empty;

        [AdditionalPrinterColumn]
        [Description("Name of the secret object holding the access key secret")]
        public string AccessKeySecretName { get; set; } = string.Empty;

        [Description("Creation time")]
        public string CreatedAt { get; set; } = string.Empty;

        [Description("Last updated time")]
        public string UpdatedAt { get; set; } = string.Empty;

        [Description("Last used time")]
        public string LastUsedAt { get; set; } = string.Empty;
    }

    public enum ReconcileState
    {
        PAUSED     = 0,
        UP_TO_DATE = 1,
        UPDATING   = 2,
        FAILURE    = 3,
    }
}
