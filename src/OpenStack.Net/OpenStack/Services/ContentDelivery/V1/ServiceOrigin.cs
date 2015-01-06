namespace OpenStack.Services.ContentDelivery.V1
{
    using System.Collections.Immutable;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class ServiceOrigin : ExtensibleJsonObject
    {
        /// <summary>
        /// This is the backing field for the <see cref="Origin"/> property.
        /// </summary>
        [JsonProperty("origin", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _origin;

        /// <summary>
        /// This is the backing field for the <see cref="Port"/> property.
        /// </summary>
        [JsonProperty("port", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private int? _port;

        /// <summary>
        /// This is the backing field for the <see cref="Ssl"/> property.
        /// </summary>
        [JsonProperty("ssl", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private bool? _ssl;

        /// <summary>
        /// This is the backing field for the <see cref="Rules"/> property.
        /// </summary>
        [JsonProperty("rules", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<ServiceOriginRule> _rules;

        public ServiceOrigin(string origin, int? port, bool? ssl, ImmutableArray<ServiceOriginRule> rules)
        {
            _origin = origin;
            _port = port;
            _ssl = ssl;
            _rules = rules;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceOrigin"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected ServiceOrigin()
        {
        }

        public string Origin
        {
            get
            {
                return _origin;
            }
        }

        public int? Port
        {
            get
            {
                return _port;
            }
        }

        public bool? Ssl
        {
            get
            {
                return _ssl;
            }
        }

        public ImmutableArray<ServiceOriginRule> Rules
        {
            get
            {
                return _rules;
            }
        }
    }
}
