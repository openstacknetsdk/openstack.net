using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "volumeAttachment")]
    public class ServerVolumeDefinition
    {
    /// <summary />
        public ServerVolumeDefinition(Identifier volumeId)
        {
            VolumeId = volumeId;
        }

        /// <summary />
        [JsonProperty("device")]
        public string DeviceName { get; set; }

        /// <summary />
        [JsonProperty("volumeId")]
        public Identifier VolumeId { get; set; }
    }
}
