using System.Collections.ObjectModel;

using k8s.Models;

using KubeOps.Operator.Entities;
using KubeOps.Operator.Entities.Annotations;

namespace UpcloudApiKubernetesOperator.Entities;

[KubernetesEntity(Group = "network.upcloud.com", ApiVersion = "v1alpha1", Kind = "SDN")]
public class V1Alpha1SDN : CustomKubernetesEntity<V1Alpha1SDN.V1Alpha1SDNSpec, V1Alpha1SDN.V1Alpha1SDNStatus>
{
    [Description("Specification for UpCloud Storage entity: https://developers.upcloud.com/1.3/9-storages/#create-storage")]
    public class V1Alpha1SDNSpec
    {
        [Required]
        [Length(MinLength = 0, MaxLength = 255)]
        [Description("A valid name for the network")]
        public string Name                          { get; set; } = string.Empty;

        [Required]
        [Length(MinLength = 1)]
        [Description("The zone in which the storage will be created, e.g. fi-hel1. See: https://developers.upcloud.com/1.3/5-zones/")]
        public string Zone                           { get; set; } = string.Empty;


        [Length(MinLength = 1)]
        [Description("An existing router UUID, empty string or null")]
        public string Router                         { get; set; } = string.Empty;

        [Length(MinLength = 1)]
        [Description("Sets address space for the network")]
        public string Address                        { get; set; } = string.Empty;

        [Required]
        [Description("Toggles DHCP service for the network")]
        public bool Dhcp                             { get; set; } = default;

        [Description("Defines if the gateway should be given as default route by DHCP. Defaults to yes on public networks, and no on other ones")]
        public bool DhcpDefaultRoute                 { get; set; } = default;

        [Description("Defined DNS server addresses given by DHCP. Not implemented on SDN private networks.")]
        public string DhcpDns                        { get; set; } = string.Empty;

        [Description("Defines additional DHCP classless static routes to be delivered if the DHCP is enabled. Nexthop will be IP network gateway address.")]
        public string DhcpRoutes                     { get; set; } = string.Empty;

        [Description("TFTP URL to boot file given by DHCP. For example tftp://172.16.0.253/pxelinux.0")]
        public string DhcpBootFileUrl                { get; set; } = string.Empty;

        [Required]
        [Pattern("IPv4")]
        [Description("IP address family. Currently only IPv4 networks are supported.")]
        public string Family                         { get; set; } = string.Empty;

        [Description("Gateway address given by the DHCP service. Defaults to first address of the network if not given.")]
        public string Gateway                         { get; set; } = string.Empty;


        [Items(MinItems = -1, MaxItems = -1)]
        [Description("Labels describing the storage, objects with both 'key' and 'value' defined.")]
        public Collection<Label> Labels              { get; set; } = new ();
    }


    [Description("Label for UpCloud storage entity")]
    public class Label
    {
        [Required]
        [Length(MaxLength = 32, MinLength = 1)]
        [Description("Key representing the label, 1-32 upper and lower case letters, numbers, - & _. Cannot start with _")]
        public string Name { get; set; }  = string.Empty;

        [Required]
        [Length(MaxLength = 255, MinLength = 0)]
        [Description("Key representing the value, 0-255 characters")]
        public string Value { get; set; } = string.Empty;
    }

    public class V1Alpha1SDNStatus
    {
        [Description("Network uuid")]
        public string Id                           { get; init; } = string.Empty;
        public string Name                         { get; init; } = string.Empty;
        public string Type                         { get; init; } = string.Empty;
        public string Zone                         { get; init; } = string.Empty;
        [Description("Attached router")]
        public V1Alpha1Router? Router              { get; init; } = null;
        public Collection<Label> Labels            { get; init; } = new ();
        public Collection<Peering> Peerings        { get; init; } = new ();
        public Collection<IpNetwork> IpNetworks    { get; init; } = new ();
        public Collection<Server> Servers          { get; init; } = new ();
    }

    public class IpNetwork
    {
        public string Address             { get; init; } = string.Empty;
        public string Dhcp                { get; init; } = string.Empty;
        public string DhcpDefaultRoute    { get; init; } = string.Empty;
        public Collection<string> DhcpDns { get; init; } = new ();
        public string Family              { get; init; } = string.Empty;
        public string Gateway             { get; init; } = string.Empty;
    }

    public class Server
    {
        public string Title { get; init; } = string.Empty;
        public string Id    { get; init; } = string.Empty;
    }

    public class Peering
    {
        public string Name  { get; init; } = string.Empty;
        public string State { get; init; } = string.Empty;
        public string Id    { get; init; } = string.Empty;
    }
}
