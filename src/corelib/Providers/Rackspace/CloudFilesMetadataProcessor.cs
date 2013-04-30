using System.Collections.Generic;
using JSIStudios.SimpleRESTServices.Client;
using net.openstack.Core;

namespace net.openstack.Providers.Rackspace
{
    internal class CloudFilesMetadataProcessor : IObjectStorageMetadataProcessor
    {
        public Dictionary<string, Dictionary<string, string>> ProcessMetadata(IList<HttpHeader> httpHeaders)
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
