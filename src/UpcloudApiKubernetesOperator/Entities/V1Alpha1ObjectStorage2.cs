using System.Collections.ObjectModel;

using k8s.Models;
using KubeOps.Operator.Entities;
using KubeOps.Operator.Entities.Annotations;

namespace UpcloudApiKubernetesOperator.Entities;

[KubernetesEntity(Group = "api.upcloud.com", ApiVersion = "v1alpha1", Kind = "ObjectStorage2")]
public class V1Alpha1ObjectStorage2 : CustomKubernetesEntity<V1Alpha1ObjectStorage2.V1Alpha1ObjectStorageSpec, V1Alpha1ObjectStorage2.V1Alpha1ObjectStorageStatus>
{
    public const string KubeApiVersion = "api.upcloud.com/v1alpha1";
    public const string KubeKind = "ObjectStorage2";

    public V1Alpha1ObjectStorage2()
    {
        ApiVersion = KubeApiVersion;
        Kind       = KubeKind;
    }

    [Description("Specification for UpCloud router entity: https://developers.upcloud.com/1.3/13-networks/#create-router")]
    public class V1Alpha1ObjectStorageSpec
    {
        [Length(MaxLength = 36, MinLength = -1)]
        [Description("UUID of already existing object storage instance")]
        public string Id { get; set; } = string.Empty;

        [Required]
        [Length(MinLength = 1, MaxLength = 255)]
        [Description("Region, see: https://developers.upcloud.com/1.3/21-managed-object-storage/#list-regions")]
        public string Region { get; set; } = string.Empty;

        [Required]
        [Pattern("stopped|started")]
        [Description("Service status, see: https://developers.upcloud.com/1.3/21-managed-object-storage/#service-configured-status")]
        public string ConfiguredStatus { get; set; } = string.Empty;

        [Items(MaxItems = 8, MinItems = 0)]
        [Description("Contains blocks that define static routes for this router.")]
        public Collection<Network> Networks { get; set; } = new ();

        [Required]
        [Items(MaxItems = 50, MinItems = 0)]
        [Description("List of cloud Users allowed to use object storage, see: https://developers.upcloud.com/1.3/21-managed-object-storage/#list-users")]
        public Collection<User> Users { get; set; } = new ();

        [Items(MaxItems = 255, MinItems = -1)]
        [Description("Labels used for service filtering, see labels usage https://developers.upcloud.com/1.3/21-managed-object-storage/#labels-usage-examples")]
        public Collection<Label> Labels { get; set; } = new ();
    }

    [Description("Network that can be used to access the object storage instance")]
    public class Network
    {
        [Required]
        [Length(MinLength = 1, MaxLength = 255)]
        [Description("Valid network name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Pattern("IPv4|IPv6")]
        [Description("Network family name")]
        public string Family { get; set; } = string.Empty;

        [Required]
        [Pattern("private|public")]
        [Description("Network type")]
        public string Type { get; set; } = string.Empty;

        [Length(MaxLength = 36, MinLength = -1)]
        [Description("Network uuid")]
        public string? Id { get; set; } = null;
    }

    [Description("Cloud API user that has access to the object storage instance")]
    public class User
    {
        [Required]
        [Length(MaxLength = -1, MinLength = 1)]
        [Description("Valid cloud user username")]
        public string Username { get; set; }  = string.Empty;
    }

    [Description("Label used for service filtering, see: https://developers.upcloud.com/1.3/21-managed-object-storage/#list-labels")]
    public class Label
    {
        [Required]
        public string Name { get; set; }  = string.Empty;
        [Required]
        public string Value { get; set; } = string.Empty;
    }

    public class V1Alpha1ObjectStorageStatus
    {
        [AdditionalPrinterColumn]
        [Description("Entity reconcile status")]
        public ReconcileState ReconcileState { get; set; } = ReconcileState.UPDATING;

        [AdditionalPrinterColumn]
        [Description("Object storage instance uuid")]
        public string Id { get; set; } = string.Empty;

        [AdditionalPrinterColumn]
        [Description("Configured service status, see: https://developers.upcloud.com/1.3/21-managed-object-storage/#service-configured-status")]
        public string ConfiguredStatus { get; set; } = string.Empty;

        [Description("Object storage instance creation time")]
        public string CreatedAt { get; set; } = string.Empty;

        [Description("List of endpoints that can be used to access the instance")]
        public Collection<Endpoint> Endpoints { get; set; } = new ();

        [Description("List of attached networks that can be used to access the instance")]
        public Collection<Network> Networks { get; set; } = new ();

        [AdditionalPrinterColumn]
        [Description("Current operational state of the object storage instance")]
        public string OperationalState { get; set; } = string.Empty;

        [AdditionalPrinterColumn]
        [Description("Region where the object storage instance is running")]
        public string Region { get; set; } = string.Empty;

        [AdditionalPrinterColumn]
        [Description("Object storage instance last updated time")]
        public string UpdatedAt { get; set; } = string.Empty;

        [Description("List of cloud API users configured to have access to the object storage instance")]
        public Collection<ConnectedUser> Users { get; set; } = new ();
    }

    public enum ReconcileState
    {
        PAUSED     = 0,
        UP_TO_DATE = 1,
        UPDATING   = 2,
        FAILURE    = 3,
    }

    [Description("Object storage instance endpoint")]
    public class Endpoint
    {
        [Description("Endpoint domain name")]
        public string DomainName { get; set; } = string.Empty;
        [Description("Endpoint type")]
        public string Type { get; set; } = string.Empty;
    }

    [Description("Object storage instance endpoint")]
    public class ConnectedUser
    {
        public string Username { get; set; } = string.Empty;
        public string OperationalState { get; set; } = string.Empty;
        public string UpdatedAt { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public Collection<AccessKey> AccessKeys { get; set; } = new ();
    }

    [Description("Object storage access key")]
    public class AccessKey
    {
        public string Name { get; set; } = string.Empty;
        public string Enabled { get; set; } = string.Empty;
        public string AccessKeyId { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public string UpdatedAt { get; set; } = string.Empty;
        public string LastUsedAt { get; set; } = string.Empty;
    }
}
