using System;
using Newtonsoft.Json;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    public class KeyPairDetails : KeyPair
    {
        /// <summary />
        [JsonProperty("id")]
        public Identifier Id { get; set; }

        /// <summary />
        [JsonProperty("created_at")]
        public DateTimeOffset Created { get; set; }
    }
}