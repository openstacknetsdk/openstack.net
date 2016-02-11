using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "security_group_rule")]
    public class SecurityGroupRuleDefinition : IHaveExtraData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityGroupRuleDefinition"/> class.
        /// </summary>
        /// <param name="protocol">The IP protocol.</param>
        /// <param name="fromPort">The source port.</param>
        /// <param name="toPort">To destination port.</param>
        /// <param name="cidr">The CIDR.</param>
        public SecurityGroupRuleDefinition(IPProtocol protocol, int fromPort, int toPort, string cidr)
        {
            Protocol = protocol;
            FromPort = fromPort;
            ToPort = toPort;
            CIDR = cidr;
        }
        
        /// <summary />
        [JsonProperty("ip_protocol")]
        public IPProtocol Protocol { get; set; }

        /// <summary />
        [JsonProperty("from_port")]
        public int FromPort { get; set; }

        /// <summary />
        [JsonProperty("to_port")]
        public int ToPort { get; set; }

        /// <summary>
        /// Classless Inter-Domain Routing (CIDR). A method for allocating IP addresses and routing Internet Protocol packets.
        /// </summary>
        [JsonProperty("cidr")]
        public string CIDR { get; set; }

        /// <summary />
        [JsonProperty("parent_group_id")]
        public Identifier GroupId { get; set; }

        /// <summary />
        [JsonProperty("group_id")]
        public Identifier SourceGroupId { get; set; }

        /// <summary />
        [JsonExtensionData]
        IDictionary<string, JToken> IHaveExtraData.Data { get; set; } = new Dictionary<string, JToken>();
    }
}