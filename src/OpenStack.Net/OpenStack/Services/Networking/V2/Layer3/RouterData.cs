namespace OpenStack.Services.Networking.V2.Layer3
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class RouterData : ExtensibleJsonObject
    {
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _name;

        [JsonProperty("admin_state_up", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private bool? _adminStateUp;

        [JsonProperty("external_gateway_info", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ExternalGateway _externalGateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="RouterData"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected RouterData()
        {
        }

        public RouterData(string name, bool? adminStateUp)
        {
            _name = name;
            _adminStateUp = adminStateUp;
        }

        public RouterData(string name, bool? adminStateUp, ExternalGateway externalGateway)
        {
            _name = name;
            _adminStateUp = adminStateUp;
            _externalGateway = externalGateway;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public bool? AdminStateUp
        {
            get
            {
                return _adminStateUp;
            }
        }

        public ExternalGateway ExternalGateway
        {
            get
            {
                return _externalGateway;
            }
        }
    }
}
