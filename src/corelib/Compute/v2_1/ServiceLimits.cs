using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "limits")]
    public class ServiceLimits : IHaveExtraData
    {
        /// <summary />
        public ServiceLimits()
        {
            RateLimits = new List<RateLimits>();
        }

        /// <summary />
        [JsonProperty("absolute")]
        public ResourceLimits ResourceLimits { get; set; }

        /// <summary />
        [JsonProperty("rate")]
        public IList<RateLimits> RateLimits { get; set; }

        /// <summary />
        [JsonExtensionData]
        IDictionary<string, JToken> IHaveExtraData.Data { get; set; } = new Dictionary<string, JToken>();
    }

    /// <summary />
    public class ResourceLimits : IHaveExtraData
    {
        /// <summary />
        [JsonProperty("maxServerMeta")]
        public int? ServerMetadataMax { get; set; }

        /// <summary />
        [JsonProperty("maxPersonality")]
        public int? PersonalityMax { get; set; }

        /// <summary />
        [JsonProperty("maxImageMeta")]
        public int? ImageMetadataMax { get; set; }

        /// <summary />
        [JsonProperty("maxPersonalitySize")]
        public int? PersonalitySizeMax { get; set; }

        /// <summary />
        [JsonProperty("maxTotalKeypairs")]
        public int? KeypairsMax { get; set; }

        /// <summary />
        [JsonProperty("maxSecurityGroupRules")]
        public int? SecurityGroupRulesMax { get; set; }

        /// <summary />
        [JsonProperty("totalServerGroupsUsed")]
        public int? ServerGroupsUsed { get; set; }

        /// <summary />
        [JsonProperty("maxServerGroups")]
        public int? ServerGroupsMax { get; set; }

        /// <summary />
        [JsonProperty("totalCoresUsed")]
        public int? CoresUsed { get; set; }

        /// <summary />
        [JsonProperty("maxTotalCores")]
        public int? CoresMax { get; set; }

        /// <summary />
        [JsonProperty("totalRAMUsed")]
        public int? MemoryUsed { get; set; }

        /// <summary />
        [JsonProperty("maxTotalRAMSize")]
        public int? MemorySizeMax { get; set; }

        /// <summary />
        [JsonProperty("totalInstancesUsed")]
        public int? ServersUsed { get; set; }

        /// <summary />
        [JsonProperty("maxTotalInstances")]
        public int? ServersMax { get; set; }

        /// <summary />
        [JsonProperty("totalSecurityGroupsUsed")]
        public int? SecurityGroupUsed { get; set; }

        /// <summary />
        [JsonProperty("maxSecurityGroups")]
        public int? SecurityGroupMax { get; set; }

        /// <summary />
        [JsonProperty("totalFloatingIpsUsed")]
        public int? FloatingIPUsed { get; set; }

        /// <summary />
        [JsonProperty("maxTotalFloatingIps")]
        public int? FloatingIPMax { get; set; }

        /// <summary />
        [JsonProperty("maxServerGroupMembers")]
        public int? ServerGroupMemberMax { get; set; }
        
        /// <summary />
        [JsonExtensionData]
        public IDictionary<string, JToken> Data { get; set; } = new Dictionary<string, JToken>();
    }

    /// <summary />
    public class RateLimits
    {
        /// <summary />
        [JsonProperty("uri")]
        public string Name { get; set; }

        /// <summary />
        [JsonProperty("regex")]
        public Regex Regex { get; set; }

        /// <summary />
        [JsonProperty("limit")]
        public IList<RateLimit> Limits { get; set; }
    }

    /// <summary />
    public class RateLimit
    {
        /// <summary />
        [JsonProperty("verb")]
        public string HttpMethod { get; set; }

        /// <summary />
        [JsonProperty("value")]
        public int Maximum { get; set; }

        /// <summary />
        [JsonProperty("remaining")]
        public int Remaining { get; set; }

        /// <summary />
        [JsonProperty("unit")]
        public string UnitOfMeasurement { get; set; }

        /// <summary />
        [JsonProperty("next-available")]
        public DateTimeOffset NextAvailable { get; set; }
    }
}