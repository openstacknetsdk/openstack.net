using Newtonsoft.Json;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    public class ServerReference
    {
        /// <summary />
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary />
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}