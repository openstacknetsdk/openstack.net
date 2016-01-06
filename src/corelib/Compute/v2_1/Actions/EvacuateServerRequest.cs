using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "evacuate")]
    public class EvacuateServerRequest
    {
        /// <summary />
        public EvacuateServerRequest(bool isServerOnSharedStorage)
        {
            IsServerOnSharedStorage = isServerOnSharedStorage;
        }

        /// <summary />
        [JsonProperty("host")]
        public Identifier DestinationHostId { get; set; }

        /// <summary />
        [JsonProperty("adminPass")] // NOTE: The doc says admin_password but the API clearly wants adminPass, see https://bugs.launchpad.net/keystone/+bug/1526446
        public string AdminPassword { get; set; }

        /// <summary />
        [JsonProperty("onSharedStorage", DefaultValueHandling = DefaultValueHandling.Include)] // NOTE: The doc says on_shared_storage but the API clearly wants onSharedStorage, see https://bugs.launchpad.net/keystone/+bug/1526446
        public bool IsServerOnSharedStorage { get; set; }
    }
}