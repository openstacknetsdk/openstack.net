namespace OpenStack.Services.Identity.V3
{
    using System;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class EndpointData : ExtensibleJsonObject
    {
        [JsonProperty("interface", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private EndpointInterface _interface;

        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _name;

        [JsonProperty("region", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _region;

        [JsonProperty("url", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _url;

        [JsonProperty("service_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ServiceId _serviceId;

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointData"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected EndpointData()
        {
        }

        public EndpointData(ServiceId serviceId, string name, string region, Uri uri, EndpointInterface @interface)
        {
            _serviceId = serviceId;
            _name = name;
            _region = region;
            _url = uri.AbsoluteUri;
            _interface = @interface;
        }

        public EndpointInterface Interface
        {
            get
            {
                return _interface;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string Region
        {
            get
            {
                return _region;
            }
        }

        public Uri Uri
        {
            get
            {
                if (_url == null)
                    return null;

                return new Uri(_url, UriKind.Absolute);
            }
        }

        public ServiceId ServiceId
        {
            get
            {
                return _serviceId;
            }
        }
    }
}
