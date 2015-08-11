using System.Collections.Generic;
using Newtonsoft.Json;

namespace OpenStack.Serialization
{
    /// <summary>
    /// Represents a collection of resources.
    /// </summary>
    /// <typeparam name="T">The resource type.</typeparam>
    /// <exclude />
    [JsonObject(MemberSerialization.OptIn)] // Using JsonObject to force the entire object to be serialized, ignoring the IEnumerable interface
    public class ResourceCollection<T> : IEnumerable<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceCollection{T}"/> class.
        /// </summary>
        public ResourceCollection()
        {
            Items = new List<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceCollection{T}"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        public ResourceCollection(IEnumerable<T> items)
        {
            Items = items.ToNonNullList();
        } 

        /// <summary>
        /// The requested items.
        /// </summary>
        public IList<T> Items { get; set; }

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