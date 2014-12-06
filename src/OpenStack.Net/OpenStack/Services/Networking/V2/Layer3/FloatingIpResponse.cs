namespace OpenStack.Services.Networking.V2.Layer3
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class FloatingIpResponse : ExtensibleJsonObject
    {
        [JsonProperty("floatingip", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public FloatingIp _floatingIp;

        /// <summary>
        /// Initializes a new instance of the <see cref="FloatingIpResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected FloatingIpResponse()
        {
        }

        public FloatingIp FloatingIp
        {
            get
            {
                return _floatingIp;
            }
        }
    }
}
