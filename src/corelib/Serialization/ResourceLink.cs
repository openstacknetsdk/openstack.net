using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OpenStack.Serialization
{
    /// <summary />
    public class ResourceLink : IHaveExtraData
    {
        /// <summary />
        public ResourceLink(string relationship, string url)
        {
            Relationship = relationship;
            Url = url;
        }

        /// <summary />
        [JsonProperty("href")]
        public string Url { get; private set; }

        /// <summary />
        [JsonProperty("rel")]
        public string Relationship { get; private set; }

        /// <summary />
        [JsonExtensionData]
        IDictionary<string, JToken> IHaveExtraData.Data { get; set; } = new Dictionary<string, JToken>();
    }
}