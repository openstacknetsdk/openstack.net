using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace OpenStack
{
    /// <summary>
    /// Represents a collection of resources.
    /// </summary>
    /// <typeparam name="T">The resource type.</typeparam>
    [JsonObject] // Using JsonObject to force the entire object to be serialized, ignoring the IEnumerable interface
    public abstract class ResourceCollection<T> : IEnumerable<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceCollection{T}"/> class.
        /// </summary>
        /// <param name="items">The requested items.</param>
        protected ResourceCollection(IEnumerable<T> items)
        {
            Items = items ?? Enumerable.Empty<T>();
        }

        /// <summary>
        /// The requested items.
        /// </summary>
        protected IEnumerable<T> Items { get; private set; }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}