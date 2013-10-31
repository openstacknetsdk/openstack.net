namespace net.openstack.Providers.Rackspace.Objects.LoadBalancers.Response
{
    using Newtonsoft.Json;

    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    internal class GetLoadBalancerResponse
    {
#pragma warning disable 649 // Field 'fieldName' is never assigned to, and will always have its default value
        [JsonProperty("loadBalancer")]
        private LoadBalancer _loadBalancer;
#pragma warning restore 649

        public LoadBalancer LoadBalancer
        {
            get
            {
                return _loadBalancer;
            }
        }
    }
}
