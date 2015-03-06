namespace OpenStack.Services.Networking.V2.PortsBinding
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class BindingCapabilities : ExtensibleJsonObject
    {
        [JsonProperty("port_filter", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private bool? _portFilter;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingCapabilities"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected BindingCapabilities()
        {
        }

        public BindingCapabilities(bool? portFilter)
        {
            _portFilter = portFilter;
        }

        public bool? PortFilter
        {
            get
            {
                return _portFilter;
            }
        }
    }
}
