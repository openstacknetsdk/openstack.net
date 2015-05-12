namespace OpenStack.Services.ContentDelivery.V1
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    public class ServiceOriginRule : ExtensibleJsonObject
    {
        /// <summary>
        /// This is the backing field for the <see cref="Name"/> property.
        /// </summary>
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _name;

        /// <summary>
        /// This is the backing field for the <see cref="RequestUri"/> property.
        /// </summary>
        [JsonProperty("request_url", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _requestUrl;

        public ServiceOriginRule(string name, string requestUri)
        {
            _name = name;
            _requestUrl = requestUri;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceOriginRule"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected ServiceOriginRule()
        {
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string RequestUri
        {
            get
            {
                return _requestUrl;
            }
        }
    }
}
