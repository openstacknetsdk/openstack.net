using Newtonsoft.Json;

namespace OpenStack.Compute.v2_1
{
    /// <summary>
    /// Enables you to boot a server from a volume when you specify additional parameters.
    /// </summary>
    /// <remarks>
    /// If you specify the volume status, you must set it to available. In the OpenStack Block Storage database, the volume attach_status must be detached. 
    /// </remarks>
    public class ServerBlockDeviceMapping
    {
        /// <summary />
        [JsonProperty("boot_index")]
        public int BootIndex { get; set; }

        /// <summary />
        [JsonProperty("device_name")]
        public string DeviceName { get; set; }

        /// <summary />
        [JsonProperty("uuid")]
        public Identifier VolumeId { get; set; }

        /// <summary />
        [JsonProperty("source_type")]
        public VolumeType SourceType { get; set; }

        /// <summary />
        [JsonProperty("destination_type")]
        public VolumeType DestinationType { get; set; }

        /// <summary />
        [JsonProperty("delete_on_termination")]
        public bool DeleteWithServer { get; set; }

        /// <summary>
        /// Specifies how/if to format the device prior to attaching, and should be only used with blank local images.
        /// <para>Denotes a swap disk if the value is swap.</para>
        /// </summary>
        [JsonProperty("guest_format")]
        public string GuestFormat { get; set; }
    }
}