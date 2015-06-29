using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Newtonsoft.Json;

namespace OpenStack
{
    /// <summary>
    /// A page of <typeparamref name="T"/> instances.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    public interface IPage<T> : IEnumerable<T>
    {
        /// <summary>
        /// Specifies if another page can be retrieved with additional items
        /// </summary>
        bool HasNextPage { get; }

        /// <summary>
        /// Retrieves the next page of items, if one is available. If <see cref="HasNextPage"/> is <c>false</c>, an empty page is returned.
        /// </summary>
        Task<IPage<T>> GetNextPageAsync(CancellationToken cancellation = default(CancellationToken));
    }

    /// <inheritdoc />
    [JsonObject] // Using JsonObject to force the entire object to be serialized, ignoring the IEnumerable interface
    public abstract class Page<T> : ResourceCollection<T>, IPage<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Page{T}"/> class.
        /// </summary>
        /// <param name="items">The items on the page.</param>
        protected Page(IEnumerable<T> items) : base(items)
        {
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
        public abstract bool HasNextPage { get; }

        /// <inheritdoc />
        public abstract Task<IPage<T>> GetNextPageAsync(CancellationToken cancellation);

        /// <summary>
        /// Returns an empty page
        /// </summary>
        public static IPage<T> Empty()
        {
            return EmptyPage.Instance;
        }

        private sealed class EmptyPage : Page<T>
        {
            public static readonly EmptyPage Instance = new EmptyPage();

            private EmptyPage() : base(Enumerable.Empty<T>()) { }

            public override bool HasNextPage
            {
                get { return false; }
            }

            public override Task<IPage<T>> GetNextPageAsync(CancellationToken cancellation)
            {
                return Task.FromResult((IPage<T>)this);
            }
        }
    }
}