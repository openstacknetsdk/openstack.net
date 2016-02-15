using Newtonsoft.Json;

namespace OpenStack.Compute.v2_1
{
    /// <summary>
    /// Resumes a suspended server and changes its status to ACTIVE.
    /// </summary>
    public class ResumeServerRequest
    {
        /// <summary />
        [JsonProperty("resume", DefaultValueHandling = DefaultValueHandling.Include, NullValueHandling = NullValueHandling.Include)]
        public string Action { get; set; }
    }
}