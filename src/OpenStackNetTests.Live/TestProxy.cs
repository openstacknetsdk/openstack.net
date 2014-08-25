namespace OpenStackNetTests.Live
{
    using System;
    using System.Net;
    using System.Net.Http;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;
    using OpenStack.Services;

    [JsonObject(MemberSerialization.OptIn)]
    public class TestProxy : ExtensibleJsonObject
    {
#pragma warning disable 649 // Field 'fieldName' is never assigned to, and will always have its default value {value}
        [JsonProperty("address", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _address;

        [JsonProperty("host", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _host;

        [JsonProperty("port", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private int? _port;
#pragma warning restore 649

        /// <summary>
        /// Initializes a new instance of the <see cref="TestProxy"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected TestProxy()
        {
        }

        public Uri Address
        {
            get
            {
                if (_address == null)
                    return null;

                return new Uri(_address);
            }
        }

        public string Host
        {
            get
            {
                return _host;
            }
        }

        public int? Port
        {
            get
            {
                return _port;
            }
        }

        public static void ConfigureService(ServiceClient service, TestProxy proxy)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            if (proxy != null)
            {
                HttpClientHandler handler = new HttpClientHandler();
                if (proxy.Address != null)
                    handler.Proxy = new WebProxy(proxy.Address);
                else if (!string.IsNullOrEmpty(proxy.Host) && proxy.Port.HasValue)
                    handler.Proxy = new WebProxy(proxy.Host, proxy.Port.Value);
                else
                    throw new InvalidOperationException();

                service.HttpClient = new HttpClient(handler);
            }
        }
    }
}