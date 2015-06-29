using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using net.openstack.Core.Domain;
using Newtonsoft.Json;

namespace OpenStack.ContentDeliveryNetworks.v1
{
    /// <summary>
    /// Represents a collection of service resources of the <see cref="IContentDeliveryNetworkService"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class ServiceCollection : Page<Service>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCollection"/> class.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="links">Any API navigation links.</param>
        public ServiceCollection(IEnumerable<Service> services)
            : base(services)
        {
        }

        /// <summary>
        /// The services.
        /// </summary>
        [JsonProperty("services")]
        protected IEnumerable<Service> Services
        {
            get { return Items; }
        }

        /// <summary>
        /// Any API navigation links.
        /// </summary>
        [JsonProperty("links")]
        public IEnumerable<Link> Links { get; set; }

        internal GetNextPageCallback NextPageHandler { get; set; }

        /// <inheritdoc />
        public override bool HasNextPage
        {
            // TODO: Extract "next" link logic into a separate generic class that we can use via composition        
            get { return Links != null && Links.Any(link => link.Rel == "next"); }
        }

        /// <inheritdoc />
        public override async Task<IPage<Service>> GetNextPageAsync(CancellationToken cancellationToken)
        {
            if (!HasNextPage)
                return Empty();

            var nextPageLink = Links.First(link => link.Rel == "next").Href;
            return await NextPageHandler(new Url(nextPageLink), cancellationToken);
        }
    }
}