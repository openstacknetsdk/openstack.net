namespace net.openstack.Providers.Rackspace.Objects.LoadBalancers.Response
{
    using Newtonsoft.Json;

    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    internal class GetLoadBalancerMetadataItemResponse
    {
#pragma warning disable 649 // Field 'fieldName' is never assigned to, and will always have its default value
        /// <summary>
        /// This is the backing field for the <see cref="MetadataItem"/> property.
        /// </summary>
        [JsonProperty("meta")]
        private LoadBalancerMetadataItem _metadataItem;
#pragma warning restore 649

        public LoadBalancerMetadataItem MetadataItem
        {
            get
            {
                return _metadataItem;
            }
        }
    }
}
