using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "server")]
    public class ServerUpdateDefinition
    {
        private string _ipv4Address;
        private string _ipv6Address;

        /// <summary />
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary />
        [JsonProperty("accessIPv4")]
        public string IPv4Address
        {
            get { return _ipv4Address; }
            set
            {
                // Nova returns "" when the value isn't set, which is causing us to serialize this propety during updates, when we really shouldn't
                _ipv4Address = !string.IsNullOrEmpty(value) ? value : null;
            }
        }

        /// <summary />
        [JsonProperty("accessIPv6")]
        public string IPv6Address
        {
            get { return _ipv6Address; }
            set
            {
                // Nova returns "" when the value isn't set, which is causing us to serialize this propety during updates, when we really shouldn't
                _ipv6Address = !string.IsNullOrEmpty(value) ? value : null;
            }
        }
    }
}