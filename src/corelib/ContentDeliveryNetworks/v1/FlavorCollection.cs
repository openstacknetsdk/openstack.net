using System.Collections.Generic;
using Newtonsoft.Json;

namespace OpenStack.ContentDeliveryNetworks.v1
{
    /// <summary>
    /// Represents a collection of flavor resources of the <see cref="IContentDeliveryNetworkService"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class FlavorCollection : ResourceCollection<Flavor>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlavorCollection"/> class.
        /// </summary>
        /// <param name="flavors">The availabe flavors.</param>
        public FlavorCollection(IEnumerable<Flavor> flavors)
            : base(flavors)
        {
        }

        /// <summary>
        /// The available flavors.
        /// </summary>
        [JsonProperty("flavors")]
        protected IEnumerable<Flavor> Flavors
        {
            get { return Items; }
        }
    }
}