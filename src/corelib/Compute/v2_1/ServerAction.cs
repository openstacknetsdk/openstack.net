using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "instanceAction")]
    public class ServerAction : ServerActionReference
    {
        /// <summary />
        public ServerAction()
        {
            Events = new List<ServerEvent>();
        }

        /// <summary />
        [JsonProperty("events")]
        public IList<ServerEvent> Events { get; set; } 
    }

    /// <summary />
    public class ServerEvent : IHaveExtraData
    {
        /// <summary />
        [JsonProperty("event")]
        public string Name { get; set; }

        /// <summary />
        [JsonProperty("result")]
        public ServerEventStatus Result { get; set; }

        /// <summary />
        [JsonProperty("start_time")]
        public DateTimeOffset Started { get; set; }

        /// <summary />
        [JsonProperty("finish_time")]
        public DateTimeOffset? Finished { get; set; }

        /// <summary />
        [JsonProperty("traceback")]
        public string StackTrace { get; set; }

        /// <summary />
        [JsonExtensionData]
        IDictionary<string, JToken> IHaveExtraData.Data { get; set; } = new Dictionary<string, JToken>();
    }
}