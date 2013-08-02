using System;
using System.Collections.Generic;
using JSIStudios.SimpleRESTServices.Client;
using net.openstack.Core;

namespace net.openstack.Providers.Rackspace
{
    internal class CloudFilesMetadataProcessor : IObjectStorageMetadataProcessor
    {
        /// <summary>
        /// A default instance of <see cref="CloudFilesMetadataProcessor"/>.
        /// </summary>
        private static readonly CloudFilesMetadataProcessor _default = new CloudFilesMetadataProcessor();

        /// <summary>
        /// Gets a default instance of <see cref="CloudFilesMetadataProcessor"/>.
        /// </summary>
        public static CloudFilesMetadataProcessor Default
        {
            get
            {
                return _default;
            }
        }

        /// <summary>
        /// Extracts metadata information from a collection of HTTP headers.
        /// </summary>
        /// <remarks>
        /// The returned collection has two keys: <see cref="CloudFilesProvider.ProcessedHeadersHeaderKey"/>
        /// and <see cref="CloudFilesProvider.ProcessedHeadersMetadataKey"/>.
        ///
        /// <para>The value for
        /// <see cref="CloudFilesProvider.ProcessedHeadersMetadataKey"/> contains the processed Cloud Files
        /// metadata included in HTTP headers such as <strong>X-Account-Meta-*</strong>,
        /// <strong>X-Container-Meta-*</strong>, and <strong>X-Object-Meta-*</strong>. The metadata prefix
        /// has been removed from the keys stored in this value.</para>
        ///
        /// <para>The value for <see cref="CloudFilesProvider.ProcessedHeadersHeaderKey"/> contains the
        /// HTTP headers which were not in the form of a known Cloud Files metadata prefix.</para>
        /// </remarks>
        /// <param name="httpHeaders">The collection of HTTP headers.</param>
        /// <returns>The metadata.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="httpHeaders"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="httpHeaders"/> contains two headers with equivalent values for <see cref="HttpHeader.Key"/> (case-insensitive).</exception>
        public virtual Dictionary<string, Dictionary<string, string>> ProcessMetadata(IList<HttpHeader> httpHeaders)
        {
            if (httpHeaders == null)
                throw new ArgumentNullException("httpHeaders");

            var pheaders = new Dictionary<string, string>();
            var metadata = new Dictionary<string, string>();
            foreach (var header in httpHeaders)
            {
                if (header.Key.ToLower().Contains(CloudFilesProvider.AccountMetaDataPrefix))
                {
                    metadata.Add(header.Key.Remove(0, 15), header.Value);
                }
                else if (header.Key.ToLower().Contains(CloudFilesProvider.ContainerMetaDataPrefix))
                {
                    metadata.Add(header.Key.Remove(0, 17), header.Value);
                }
                else if (header.Key.ToLower().Contains(CloudFilesProvider.ObjectMetaDataPrefix))
                {
                    metadata.Add(header.Key.Remove(0, 14), header.Value);
                }
                else
                {
                    pheaders.Add(header.Key.ToLower(), header.Value);
                }
            }
            var processedHeaders = new Dictionary<string, Dictionary<string, string>>()
                {
                    {CloudFilesProvider.ProcessedHeadersHeaderKey, pheaders},
                    {CloudFilesProvider.ProcessedHeadersMetadataKey, metadata}
                };

            return processedHeaders;
        }
    }
}
