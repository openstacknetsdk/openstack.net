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
        /// <summary>
        /// The requested services.
        /// </summary>
        [JsonProperty("services")]
        public IList<Service> Services
        {
            get { return Items; }
        }
        
        /// <summary>
        /// The paging navigation links.
        /// </summary>
        [JsonProperty("links")]
        public IList<Link> ServiceLinks
        {
            get { return Links; }
        }
    }
}