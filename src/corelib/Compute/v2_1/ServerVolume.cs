using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "volumeAttachment")]
    public class ServerVolume : ServerVolumeReference
    {
        private Identifier _serverId;

        /// <summary />
        [JsonProperty("device")]
        public string DeviceName { get; set; }

        /// <summary />
        [JsonProperty("serverId")]
        public Identifier ServerId
        {
            get { return _serverId; }
            set
            {
                _serverId = value;
                ((IChildResource)this).SetParent(_serverId);
            }
        }

        /// <summary />
        [JsonProperty("volumeId")]
        public Identifier VolumeId { get; set; }
    }
}
