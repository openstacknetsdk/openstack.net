using Newtonsoft.Json;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    public class StartServerRequest
    {
        /// <summary />
        [JsonProperty("os-start", DefaultValueHandling = DefaultValueHandling.Include, NullValueHandling = NullValueHandling.Include)]
        public object Action { get; set; }
    }
}