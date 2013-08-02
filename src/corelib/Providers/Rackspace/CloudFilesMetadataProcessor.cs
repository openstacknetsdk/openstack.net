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

        public virtual Dictionary<string, Dictionary<string, string>> ProcessMetadata(IList<HttpHeader> httpHeaders)
        {
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
