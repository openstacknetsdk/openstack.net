using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace OpenStack.Networking.v2.Layer3
{
    /// <summary>
    /// 
    /// </summary>
    public class SecurityGroupRule
    {
        /// <summary>
        ///ngress or egress: the direction in which the security group rule is applied.
        ///For a compute instance, an ingress security group rule is applied to incoming (ingress) traffic for that instance. 
        ///An egress rule is applied to traffic leaving the instance.
        /// </summary>
        [JsonProperty("direction")]
        public string Direction;

        /// <summary>
        ///Must be IPv4 or IPv6, and addresses represented in CIDR must match the ingress or egress rules. 
        /// </summary>
        [JsonProperty("ethertype")]
        public string Ethertype;

        /// <summary>
        /// The UUID of the security group rule.
        /// </summary>
        [JsonProperty("id")]
        public Identifier Id;

        /// <summary>
        ///The maximum port number in the range that is matched by the security group rule. 
        ///The port_range_min attribute constrains the port_range_max attribute.
        ///If the protocol is ICMP, this value must be an ICMP type. 
        /// </summary>
        [JsonProperty("port_range_max")]
        public int PortRangeMax;

        ///<summary>
        ///The minimum port number in the range that is matched by the security group rule.
        ///If the protocol is TCP or UDP, this value must be less than or equal to the port_range_max attribute value.
        ///If the protocol is ICMP, this value must be an ICMP type.
        /// </summary>
        [JsonProperty("port_range_min")]
        public int PortRangeMin;

        /// <summary>
        ///The protocol that is matched by the security group rule. Value is null, icmp, icmpv6, tcp, or udp. 
        /// </summary>
        [JsonProperty("protocol")]
        public string Protocol;

        ///<summary>
        ///The remote group UUID to associate with this security group rule.
        ///You can specify either the remote_group_id or remote_ip_prefix attribute in the request body. 
        /// </summary>
        [JsonProperty("remote_group_id")]
        public string RemoteGroupId;

        /// <summary>
        ///The remote IP prefix to associate with this security group rule.
        ///You can specify either the remote_group_id or remote_ip_prefix attribute in the request body.
        ///This attribute value matches the IP prefix as the source IP address of the IP packet. 
        /// </summary>
        [JsonProperty("remote_ip_prefix")]
        public string RemoteIpPrefix;

        /// <summary>
        ///The UUId of security group 
        /// </summary>
        [JsonProperty("security_group_id")]
        public string SecurityGroupId;

        /// <summary>
        /// The UUID of the tenant who owns the security group rule. Only administrative users can specify a tenant UUID other than their own.
        /// </summary>
        [JsonProperty("tenant_id")]
        public string TenantId;
    }
}
