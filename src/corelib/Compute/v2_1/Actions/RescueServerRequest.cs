using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "rescue")]
    public class RescueServerRequest
    {
        /// <summary />
        [JsonProperty("adminPass")]
        public string AdminPassword { get; set; }

        /// <summary />
        [JsonProperty("rescue_image_ref")]
        public Identifier ImageId { get; set; }
    }
}