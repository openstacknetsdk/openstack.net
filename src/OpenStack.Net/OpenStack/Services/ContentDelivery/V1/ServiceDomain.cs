namespace OpenStack.Services.ContentDelivery.V1
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class ServiceDomain : ExtensibleJsonObject
    {
        /// <summary>
        /// This is the backing field for the <see cref="Domain"/> property.
        /// </summary>
        [JsonProperty("domain", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _domain;

        /// <summary>
        /// This is the backing field for the <see cref="Protocol"/> property.
        /// </summary>
        [JsonProperty("protocol", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ServiceProtocol _protocol;

        public ServiceDomain(string domain, ServiceProtocol protocol)
        {
            _domain = domain;
            _protocol = protocol;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDomain"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected ServiceDomain()
        {
        }

        /// <summary>
        /// Gets the domain used to access the assets on their website, which will be CNAMEd to the CDN provider.
        /// </summary>
        /// <value>
        /// <para>The domain used to access the assets on their website, which will be CNAMEd to the CDN provider.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public string Domain
        {
            get
            {
                return _domain;
            }
        }

        /// <summary>
        /// Gets the protocol used to access the assets on this domain.
        /// </summary>
        /// <value>
        /// <para>The protocol used to access the assets on this domain.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        public ServiceProtocol Protocol
        {
            get
            {
                return _protocol;
            }
        }
    }
}
