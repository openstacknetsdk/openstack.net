using Newtonsoft.Json;

namespace OpenStack.Compute.v2_1
{
    /// <summary>
    /// Starts a stopped server and changes its status to ACTIVE.
    /// </summary>
    public class StartServerRequest
    {
        /// <summary>
        /// The requested server action.
        /// </summary>
        [JsonProperty("os-start", DefaultValueHandling = DefaultValueHandling.Include, NullValueHandling = NullValueHandling.Include)]
        public object Action { get; set; }
    }
}