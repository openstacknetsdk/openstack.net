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
    }
}