using System.Collections.Generic;
using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "server")]
    public class ServerCreateDefinition
    {
        /// <summary />
        public ServerCreateDefinition(string name, Identifier imageId, string flavorId)
        {
            Name = name;
            ImageId = imageId;
            FlavorId = flavorId;
        }

        /// <summary />
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary />
        [JsonProperty("imageRef")]
        public Identifier ImageId { get; set; }

        /// <summary />
        [JsonProperty("flavorRef")]
        public string FlavorId { get; set; }

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
        [JsonProperty("os:scheduler_hints")]
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
    }
}