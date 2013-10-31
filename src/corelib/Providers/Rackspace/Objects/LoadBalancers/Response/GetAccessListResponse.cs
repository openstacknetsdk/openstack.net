namespace net.openstack.Providers.Rackspace.Objects.LoadBalancers.Response
{
    using System.Collections.ObjectModel;
    using Newtonsoft.Json;

    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    internal class GetAccessListResponse
    {
#pragma warning disable 649 // Field 'fieldName' is never assigned to, and will always have its default value
        [JsonProperty("accessList")]
        private NetworkItem[] _accessList;
#pragma warning restore 649

        public ReadOnlyCollection<NetworkItem> AccessList
        {
            get
            {
                if (_accessList == null)
                    return null;

                return new ReadOnlyCollection<NetworkItem>(_accessList);
            }
        }
    }
}
