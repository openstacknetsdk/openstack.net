using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "security_group_rule")]
    public class SecurityGroupRule : IHaveExtraData, IServiceResource
    {
        /// <summary />
        [JsonProperty("id")]
        public Identifier Id { get; set; }

        /// <summary />
        [JsonProperty("ip_protocol")]
        public IPProtocol Protocol { get; set; }

        /// <summary />
        [JsonProperty("from_port")]
        public int FromPort { get; set; }

        /// <summary />
        [JsonProperty("to_port")]
        public int ToPort { get; set; }

        /// <summary />
        [JsonIgnore]
        public string CIDR
        {
            get { return _ipRange.CIDR; }
            set { _ipRange.CIDR = value; }
        }

        /// <summary />
        [JsonProperty("ip_range")]
        private CIDRWrapper _ipRange = new CIDRWrapper();

        /// <summary />
        [JsonProperty("parent_group_id")]
        public Identifier GroupId { get; set; }

        /// <summary />
        [JsonProperty("group_id")]
        public Identifier SourceGroupId { get; set; }

        /// <summary />
        [JsonExtensionData]
        IDictionary<string, JToken> IHaveExtraData.Data { get; set; } = new Dictionary<string, JToken>();

        object IServiceResource.Owner { get; set; }

        /// <inheritdoc cref="ComputeApiBuilder.DeleteSecurityGroupRuleAsync" />
        /// <exception cref="InvalidOperationException">When this instance was not constructed by the <see cref="ComputeService"/>, as it is missing the appropriate internal state to execute service calls.</exception>
        public async Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var owner = this.TryGetOwner<ComputeApiBuilder>();
            await owner.DeleteSecurityGroupRuleAsync(Id, cancellationToken);
        }

        private class CIDRWrapper
        {
            [JsonProperty("cidr")]
            public string CIDR { get; set; }
        }
    }
}