using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "snapshot")]
    public class VolumeSnapshotDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VolumeSnapshotDefinition"/> class.
        /// </summary>
        public VolumeSnapshotDefinition()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VolumeSnapshotDefinition"/> class.
        /// </summary>
        /// <param name="volumeId">The volume identifier.</param>
        public VolumeSnapshotDefinition(Identifier volumeId)
        {
            VolumeId = volumeId;
        }

        /// <summary />
        [JsonProperty("volume_id")]
        public Identifier VolumeId { get; set; }

        /// <summary />
        [JsonProperty("display_name")]
        public string Name { get; set; }

        /// <summary />
        [JsonProperty("display_description")]
        public string Description { get; set; }

        /// <summary>
        /// Specifies whether to snapshot a volume even if it's attached to an instance
        /// </summary>
        [JsonProperty("force")]
        public bool? ShouldForce { get; set; }
    }
}