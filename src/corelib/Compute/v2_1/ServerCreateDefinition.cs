using System.Collections.Generic;
using Newtonsoft.Json;
using OpenStack.Compute.v2_1.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    [JsonConverter(typeof(ServerCreateDefinitionConverter))]
    public class ServerCreateDefinition
    {
        /// <summary />
        public ServerCreateDefinition(string name, Identifier imageId, Identifier flavorId)
        {
            Name = name;
            ImageId = imageId;
            FlavorId = flavorId;
            SecurityGroups = new List<SecurityGroupReference>();
            Networks = new List<NetworkAttachDefinition>();
            Metadata = new Dictionary<string, string>();
            Personality = new List<Personality>();
            BlockDeviceMapping = new List<ServerBlockDeviceMapping>();
        }

        /// <summary />
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary />
        [JsonProperty("imageRef")]
        public Identifier ImageId { get; set; }

        /// <summary />
        [JsonProperty("flavorRef")]
        public Identifier FlavorId { get; set; }

        /// <summary />
        [JsonProperty("security_groups")]
        public IList<SecurityGroupReference> SecurityGroups { get; set; }

        /// <summary />
        [JsonProperty("availability_zone")]
        public string AvailabilityZone { get; set; }

        /// <summary /> // base 64 encoded string
        [JsonProperty("user_data")]
        public string UserData { get; set; }

        /// <summary />
        [JsonProperty("networks")]
        public IList<NetworkAttachDefinition> Networks { get; set; }

        /// <summary />
        [JsonProperty("metadata")]
        public IDictionary<string, string> Metadata { get; set; }

        /// <summary />
        [JsonProperty("personality")]
        public IList<Personality> Personality { get; set; }

        /// <summary />
        [JsonProperty("block_device_mapping_v2")]
        public IList<ServerBlockDeviceMapping> BlockDeviceMapping { get; set; }

        /// <summary /> // base 64 encoded string
        [JsonProperty("config_drive")]
        public bool ShouldUseConfigurationDrive { get; set; }

        /// <summary />
        [JsonProperty("key_name")]
        public string KeyPairName { get; set; }

        /// <summary />
        [JsonIgnore] // Serialized at the same level as "server"
        public SchedulerHints SchedulerHints { get; set; }

        /// <summary />
        [JsonProperty("OS-DCF:diskConfig")]
        public DiskConfiguration DiskConfig { get; set; }

        /// <summary />
        public void LoadUserDataFromFile(string path)
        {
            byte[] contents = System.IO.File.ReadAllBytes(path);
            UserData = System.Convert.ToBase64String(contents);
        }

        /// <summary>
        /// Configures the server to boot from an existing volume.
        /// </summary>
        /// <param name="volumeId">The volume identifier.</param>
        /// <param name="deleteVolumeWithServer">if set to <c>true</c> [delete volume with server].</param>
        public void ConfigureBootFromVolume(Identifier volumeId, bool deleteVolumeWithServer = false)
        {
            BlockDeviceMapping.Add(new ServerBlockDeviceMapping
            {
                SourceType = ServerBlockDeviceType.Volume,
                SourceId = volumeId,
                BootIndex = 0,
                DeleteWithServer = deleteVolumeWithServer
            });
        }

        /// <summary>
        /// Configures the server to boot from a copy of an existing volume.
        /// </summary>
        /// <param name="volumeId">The volume identifier.</param>
        /// <param name="volumeSize">Size of the volume.</param>
        /// <param name="deleteVolumeWithServer">if set to <c>true</c> [delete volume with server].</param>
        public void ConfigureBootFromNewVolume(Identifier volumeId, int volumeSize, bool deleteVolumeWithServer = false)
        {
            BlockDeviceMapping.Add(new ServerBlockDeviceMapping
            {
                SourceType = ServerBlockDeviceType.Volume,
                SourceId = volumeId,
                BootIndex = 0,
                DestinationType = ServerBlockDeviceType.Volume,
                DestinationVolumeSize = volumeSize,
                DeleteWithServer = deleteVolumeWithServer
            });
        }

        /// <summary>
        /// Configures the server to boot from a new volume, copied from the base server image.
        /// </summary>
        /// <param name="volumeSize">Size of the volume.</param>
        /// <param name="deleteVolumeWithServer">if set to <c>true</c> [delete volume with server].</param>
        public void ConfigureBootFromNewVolume(int volumeSize, bool deleteVolumeWithServer = false)
        {
            BlockDeviceMapping.Add(new ServerBlockDeviceMapping
            {
                SourceType = ServerBlockDeviceType.Image,
                SourceId = ImageId,
                BootIndex = 0,
                DestinationType = ServerBlockDeviceType.Volume,
                DestinationVolumeSize = volumeSize,
                DeleteWithServer = deleteVolumeWithServer
            });
        }
    }
}