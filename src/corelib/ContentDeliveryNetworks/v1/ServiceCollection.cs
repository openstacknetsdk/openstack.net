using System.Collections.Generic;
using net.openstack.Core.Domain;
using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.ContentDeliveryNetworks.v1
{
    /// <summary>
    /// Represents a collection of service resources of the <see cref="IContentDeliveryNetworkService"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    public class ServiceCollection : Page<Service>
    {
        [JsonProperty("services")]
        internal IList<Service> Services
        {
            get { return Items; }
        }
        
        [JsonProperty("links")]
        internal IList<Link> ServiceLinks
        {
            get { return Links; }
        }
    }
}