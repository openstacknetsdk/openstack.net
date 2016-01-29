using System.Collections.Generic;
using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "volume_type")]
    public class VolumeType
    {
        /// <summary />
        [JsonProperty("id")]
        public Identifier Id { get; set; }

        /// <summary />
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary />
        [JsonProperty("extra_specs")]
        public IDictionary<string, string> ExtraSpecs { get; set; }
    }
}