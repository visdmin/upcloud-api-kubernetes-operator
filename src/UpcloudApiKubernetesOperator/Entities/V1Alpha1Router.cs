using System.Collections.ObjectModel;

using k8s.Models;
using KubeOps.Operator.Entities;
using KubeOps.Operator.Entities.Annotations;

namespace UpcloudApiKubernetesOperator.Entities;

[KubernetesEntity(Group = "network.upcloud.com", ApiVersion = "v1alpha1", Kind = "Router")]
public class V1Alpha1Router : CustomKubernetesEntity<V1Alpha1Router.V1Alpha1RouterSpec, V1Alpha1Router.V1Alpha1RouterStatus>
{
    [Description("Specification for UpCloud router entity: https://developers.upcloud.com/1.3/13-networks/#create-router")]
    public class V1Alpha1RouterSpec
    {
        [Required]
        [Length(MinLength = 1, MaxLength = 255)]
        [Description("Non-empty string")]
        public string Name { get; set; } = string.Empty;

        [Items(MaxItems = 50, MinItems = -1)]
        [Description("Contains blocks that define static routes for this router.")]
        public Collection<StaticRoute> StaticRoutes { get; set; } = new ();

        [Items(MaxItems = -1, MinItems = -1)]
        [Description("Contains blocks that define static routes for this router.")]
        public Collection<Label> Labels { get; set; } = new ();
    }

    [Description("Backup rule for UpCloud storage entity, see: https://developers.upcloud.com/1.3/13-networks/#create-router")]
    public class StaticRoute
    {
        [Length(MinLength = 1, MaxLength = 64)]
        [Pattern("^[a-zA-Z0-9_-]+$")]
        [Description("Name or description of the route.")]
        public string Name { get; set; }  = string.Empty;

        [Required]
        [Description("Destination prefix of the route, Valid IPv4 or IPv6 prefix")]
        [Pattern("^[0-9]*$")]
        public string Route     { get; set; }  = string.Empty;
        [Required]
        [Description("Next hop address, Valid IPv4 or IPv6 prefix")]
        [Pattern("^[0-9]*$")]
        public string NextHop     { get; set; }  = string.Empty;

    }

    [Description("Label for UpCloud storage entity")]
    public class Label
    {
        [Required]
        public string Name { get; set; }  = string.Empty;
        [Required]
        public string Value { get; set; } = string.Empty;
    }

    public class V1Alpha1RouterStatus
    {
        [Description("Router uuid")]
        public string Id { get; set; } = string.Empty;

        [Description("Router type")]
        public string Type { get; set; } = string.Empty;

        [Description("Attached networks")]
        public Collection<V1Alpha1SDN> AttachedNetworks { get; init; } = new ();

        [Description("Contains blocks that define static routes for this router.")]
        public Collection<Label> Labels { get; set; } = new ();

        [Description("Contains blocks that define static routes for this router.")]
        public Collection<StaticRoute> StaticRoutes { get; set; } = new ();
    }
}
