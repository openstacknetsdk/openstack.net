using Newtonsoft.Json;

namespace OpenStack.Compute.v2_1
{
    /// <summary>
    /// Stops a running server and changes its status to SHUTOFF.
    /// </summary>
    public class StopServerRequest
    {
        /// <summary />
        [JsonProperty("os-stop", DefaultValueHandling = DefaultValueHandling.Include, NullValueHandling = NullValueHandling.Include)]
        public string Action { get; set; }
    }
}