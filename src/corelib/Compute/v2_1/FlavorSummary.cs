using Newtonsoft.Json;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    public class FlavorSummary : FlavorReference
    {
        /// <summary /> // In some cases, only the id is populated. Use GetFlavor if Name is null.
        [JsonProperty("name")]
        public string Name { get; set; }   
    }
}