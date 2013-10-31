namespace net.openstack.Providers.Rackspace.Objects.LoadBalancers.Response
{
    using Newtonsoft.Json;

    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    internal class GetLoadBalancerSslConfigurationResponse
    {
#pragma warning disable 649 // Field 'fieldName' is never assigned to, and will always have its default value {value}
        [JsonProperty("sslTermination")]
        private LoadBalancerSslConfiguration _sslConfiguration;
#pragma warning restore 649

        public LoadBalancerSslConfiguration SslConfiguration
        {
            get
            {
                return _sslConfiguration;
            }
        }
    }
}
