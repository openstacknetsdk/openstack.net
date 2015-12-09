using System.Collections.Generic;
using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    [JsonConverterWithConstructor(typeof (RootWrapperConverter), "createImage")]
    public class SnapshotRequest
    {
        /// <summary />
        public SnapshotRequest(string name)
        {
            Name = name;
            Metadata = new Dictionary<string, string>();
        }

        /// <summary />
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary />
        [JsonProperty("metadata")]
        public IDictionary<string, string> Metadata { get; set; }
    }
}
