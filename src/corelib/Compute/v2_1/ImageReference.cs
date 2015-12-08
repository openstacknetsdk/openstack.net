using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    public class ImageReference : IHaveExtraData
    {
        /// <summary />
        public Identifier Id { get; set; }

        /// <summary />
        public string Name { get; set; }

        // todo: implement
        //public Image GetImage();

        /// <summary />
        [JsonExtensionData]
        IDictionary<string, JToken> IHaveExtraData.Data { get; set; } = new Dictionary<string, JToken>();
    }
}