using Newtonsoft.Json;

namespace OpenStack.Compute.v2_1
{
    /// <summary>
    /// Suspends a server and changes its status to SUSPENDED.
    /// </summary>
    public class SuspendServerRequest
    {
        /// <summary />
        [JsonProperty("suspend", DefaultValueHandling = DefaultValueHandling.Include, NullValueHandling = NullValueHandling.Include)]
        public string Action { get; set; }
    }
}