using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using net.openstack.Core.Domain;
using Newtonsoft.Json;

namespace OpenStack.Serialization
{
    /// <inheritdoc cref="IPage{T}" />
    [JsonObject(MemberSerialization.OptIn)]
    public class Page<T> : ResourceCollection<T>, IPage<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Page{T}"/> class.
        /// </summary>
        public Page()
        {
            Links = new List<Link>();    
        }

        /// <summary>
        /// Callback method used when retrieving the next page of items.
        /// </summary>
        /// <param name="url">The URL which represents a request for the next page.</param>
        /// <param name="cancellation">The cancellation.</param>
        /// <returns>
        /// The next page of items.
        /// </returns>
        public delegate Task<IPage<T>> GetNextPageCallback(Url url, CancellationToken cancellation);

        /// <inheritdoc />
        public bool HasNextPage
        {
            get { return GetNextLink() != null; }
        }

        /// <inheritdoc />
        public GetNextPageCallback NextPageHandler { get; set; }

        /// <inheritdoc />
        public async Task<IPage<T>> GetNextPageAsync(CancellationToken cancellationToken)
        {
            var nextPageLink = GetNextLink();
            if (nextPageLink == null)
                return Empty();

            return await NextPageHandler(new Url(nextPageLink.Href), cancellationToken);
        }

        /// <summary>
        /// Returns an empty page
        /// </summary>
        public static IPage<T> Empty()
        {
            return EmptyPage.Instance;
        }

        /// <summary>
        /// The paging navigation links.
        /// </summary>
        public IList<Link> Links { get; set; }

        /// <summary>
        /// Finds the next link.
        /// </summary>
        protected virtual Link GetNextLink()
        {
            return Links.FirstOrDefault(x => x.Rel == "next");
        }

        private sealed class EmptyPage : Page<T>
        {
            public static readonly EmptyPage Instance = new EmptyPage();

            private EmptyPage() { }
        }
    }
}