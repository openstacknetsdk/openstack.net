namespace OpenStack.Services.Networking.V2
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;
    using IPAddressSimpleConverter = OpenStack.ObjectModel.Converters.IPAddressSimpleConverter;

#if PORTABLE
    using IPAddress = System.String;
#else
    using IPAddress = System.Net.IPAddress;
#endif

    [JsonObject(MemberSerialization.OptIn)]
    public class FixedIp : ExtensibleJsonObject
    {
        [JsonProperty("subnet_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private SubnetId _subnetId;

        [JsonProperty("ip_address", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(IPAddressSimpleConverter))]
        private IPAddress _ipAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedIp"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected FixedIp()
        {
        }

        public FixedIp(SubnetId subnetId, IPAddress ipAddress)
        {
            _subnetId = subnetId;
            _ipAddress = ipAddress;
        }

        public SubnetId SubnetId
        {
            get
            {
                return _subnetId;
            }
        }

        public IPAddress IPAddress
        {
            get
            {
                return _ipAddress;
            }
        }
    }
}
