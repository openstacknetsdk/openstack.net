namespace System.Collections.Generic
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Provides extension methods to support consistent <see cref="List{T}"/> API operations
    /// across multiple versions of the .NET framework.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public static class OpenStackListExtensions
    {
        /// <summary>
        /// Returns a read-only <see cref="IList{T}"/> wrapper for a specified collection.
        /// </summary>
        /// <typeparam name="T">The type of element stored in the list.</typeparam>
        /// <param name="list">The list to provide a read-only wrapper for.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> that acts as a read-only wrapper around the specified <paramref name="list"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="list"/> is <see langword="null"/>.</exception>
        public static ReadOnlyCollection<T> AsReadOnly<T>(this List<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            return new ReadOnlyCollection<T>(list);
        }
    }
}
