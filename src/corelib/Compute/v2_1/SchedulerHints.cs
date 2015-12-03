using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    public class SchedulerHints : IHaveExtraData
    {
        /// <summary />
        public SchedulerHints()
        {
            DifferentHost = new List<string>();
            SameHost = new List<string>();
        }

        /// <summary />
        [JsonProperty("query")]
        public string Query { get; set; }

        /// <summary />
        [JsonProperty("build_near_host_ip")]
        public string BuildNearHostIP { get; set; }

        /// <summary />
        [JsonProperty("cidr")]
        public string CIDR { get; set; }

        /// <summary />
        [JsonProperty("different_host")]
        public IList<string> DifferentHost { get; set; }

        /// <summary />
        [JsonProperty("same_host")]
        public IList<string> SameHost { get; set; }

        /// <summary />
        [JsonExtensionData]
        public IDictionary<string, JToken> Data { get; set; } = new Dictionary<string, JToken>();
    }
}