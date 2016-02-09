using Newtonsoft.Json;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    public class ImageSummary : ImageReference
    {
        /// <summary />
        [JsonProperty("name")]
        public string Name { get; set; }

    }
}