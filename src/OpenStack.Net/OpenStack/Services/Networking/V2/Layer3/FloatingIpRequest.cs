namespace OpenStack.Services.Networking.V2.Layer3
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class FloatingIpRequest : ExtensibleJsonObject
    {
        [JsonProperty("floatingip", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private FloatingIpData _floatingIp;

        /// <summary>
        /// Initializes a new instance of the <see cref="FloatingIpRequest"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected FloatingIpRequest()
        {
        }

        public FloatingIpRequest(FloatingIpData floatingIp)
        {
            _floatingIp = floatingIp;
        }

        public FloatingIpData FloatingIp
        {
            get
            {
                return _floatingIp;
            }
        }
    }
}
