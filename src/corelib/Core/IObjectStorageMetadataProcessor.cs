using System;
using System.Collections.Generic;
using JSIStudios.SimpleRESTServices.Client;

namespace net.openstack.Core
{
    /// <summary>
    /// This interface represents an object that can extract metadata information from a
    /// collection of HTTP headers returned from a REST request.
    /// </summary>
    public interface IObjectStorageMetadataProcessor
    {
        /// <summary>
        /// Extracts metadata information from a collection of HTTP headers. The returned collection
        /// may include multiple types of metadata information. The keys of the collection represent
        /// the type of metadata, and the values are key-value collections of the corresponding
        /// metadata.
        /// </summary>
        /// <param name="httpHeaders">The collection of HTTP headers.</param>
        /// <returns>The metadata.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="httpHeaders"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="httpHeaders"/> contains two headers with equivalent values for <see cref="HttpHeader.Key"/> (case-insensitive).</exception>
        Dictionary<string, Dictionary<string, string>> ProcessMetadata(IList<HttpHeader> httpHeaders);
    }
}
