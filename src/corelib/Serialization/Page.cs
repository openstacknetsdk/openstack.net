using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Newtonsoft.Json;

namespace OpenStack.Serialization
{
    /// <inheritdoc cref="IPage{T}" />
    /// <exclude />
    [JsonObject(MemberSerialization.OptIn)]
    public class Page<T> : ResourceCollection<T>, IPage<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Page{T}"/> class.
        /// </summary>
        public Page()
        {
            Links = new List<ResourceLink>();    
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
        [JsonIgnore]
        public bool HasNextPage => GetNextLink() != null;

        /// <inheritdoc />
        public GetNextPageCallback NextPageHandler { get; set; }

        /// <inheritdoc />
        public async Task<IPage<T>> GetNextPageAsync(CancellationToken cancellationToken)
        {
            var nextPageLink = GetNextLink();
            if (nextPageLink == null)
                return Empty();

            return await NextPageHandler(new Url(nextPageLink.Url), cancellationToken);
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
        public IList<ResourceLink> Links { get; set; }

        /// <summary>
        /// Finds the next link.
        /// </summary>
        protected virtual ResourceLink GetNextLink()
        {
            return Links.FirstOrDefault(x => x.Relationship == "next");
        }

        private sealed class EmptyPage : Page<T>
        {
            public static readonly EmptyPage Instance = new EmptyPage();

            private EmptyPage() { }
        }
    }
}