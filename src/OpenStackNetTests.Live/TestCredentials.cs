namespace OpenStackNetTests.Live
{
    using System;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;
    using OpenStack.Services.Identity.V2;

    [JsonObject(MemberSerialization.OptIn)]
    internal class TestCredentials : ExtensibleJsonObject
    {
#pragma warning disable 649 // Field 'fieldName' is never assigned to, and will always have its default value {value}
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _name;

        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _description;

        [JsonProperty("vendor", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _vendor;

        [JsonProperty("baseAddress", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _baseAddress;

        [JsonProperty("defaultRegion", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _defaultRegion;

        [JsonProperty("authRequest", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private AuthenticationRequest _authRequest;

        [JsonProperty("proxy", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private TestProxy _proxy;
#pragma warning restore 649

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCredentials"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected TestCredentials()
        {
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public string Vendor
        {
            get
            {
                return _vendor;
            }
        }

        public Uri BaseAddress
        {
            get
            {
                if (_baseAddress == null)
                    return null;

                return new Uri(_baseAddress);
            }
        }

        public string DefaultRegion
        {
            get
            {
                return _defaultRegion;
            }
        }

        public AuthenticationRequest AuthenticationRequest
        {
            get
            {
                return _authRequest;
            }
        }

        public TestProxy Proxy
        {
            get
            {
                return _proxy;
            }
        }
    }
}
