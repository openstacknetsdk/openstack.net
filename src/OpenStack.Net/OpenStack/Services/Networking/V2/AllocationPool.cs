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
    public class AllocationPool : ExtensibleJsonObject
    {
        [JsonProperty("end", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(IPAddressSimpleConverter))]
        private IPAddress _start;

        [JsonProperty("end", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(IPAddressSimpleConverter))]
        private IPAddress _end;

        /// <summary>
        /// Initializes a new instance of the <see cref="AllocationPool"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected AllocationPool()
        {
        }

        public AllocationPool(IPAddress start, IPAddress end)
        {
            _start = start;
            _end = end;
        }

        public IPAddress Start
        {
            get
            {
                return _start;
            }
        }

        public IPAddress End
        {
            get
            {
                return _end;
            }
        }
    }
}
