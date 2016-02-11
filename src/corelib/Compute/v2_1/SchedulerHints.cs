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

        /// <summary>
        /// Classless Inter-Domain Routing (CIDR). A method for allocating IP addresses and routing Internet Protocol packets.
        /// </summary>
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

        /// <summary>
        /// Adds a custom scheduling hint.
        /// </summary>
        /// <param name="hint">The hint key.</param>
        /// <param name="value">The hint value.</param>
        public void Add(string hint, object value)
        {
            Data.Add(hint, JToken.FromObject(value));
        }
    }
}