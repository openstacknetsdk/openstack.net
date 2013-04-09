using System;
using System.Collections.Generic;
using System.Web;
using JSIStudios.SimpleRESTServices.Client;
using net.openstack.Core;
using net.openstack.Core.Exceptions;

namespace net.openstack.Providers.Rackspace
{
    public class CloudFilesHelper : ICloudFilesHelper
    {
        public void ValidateContainerName(string containerName)
        {
            var containerNameString = string.Format("Container Name:[{0}]", containerName);
            if (string.IsNullOrWhiteSpace(containerName))
                throw new ArgumentNullException("ContainerName", "ERROR: Container Name cannot be Null.");
            if (HttpUtility.UrlEncode(containerName).Length > 256)
                throw new ContainerNameException(string.Format("ERROR: encoded URL Length greater than 256 char's. {0}", containerNameString));
            if (containerName.Contains("/"))
                throw new ContainerNameException(string.Format("ERROR: Container Name contains a /. {0}", containerNameString));
        }

        public void ValidateObjectName(string objectName)
        {
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentNullException();
            if (HttpUtility.UrlEncode(objectName).Length > 1024)
                throw new ObjectNameException("ERROR: Url Encoded Object Name exceeds 1024 char's");
        }

        //public Dictionary<string, Dictionary<string, string>> ProcessMetadata(Dictionary<string, string> headers)
        //{
        //    var pheaders = new Dictionary<string, string>();
        //    var metadata = new Dictionary<string, string>();
        //    foreach (var header in headers)
        //    {
        //        if (header.Key.ToLower().Contains(CloudFilesConstants.AccountMetaDataPrefix))
        //        {
        //            metadata.Add(header.Key.Remove(0, 15), header.Value);
        //        }
        //        else if (header.Key.ToLower().Contains(CloudFilesConstants.ContainerMetaDataPrefix))
        //        {
        //            metadata.Add(header.Key.Remove(0, 17), header.Value);
        //        }
        //        else if (header.Key.ToLower().Contains(CloudFilesConstants.ObjectMetaDataPrefix))
        //        {
        //            metadata.Add(header.Key.Remove(0, 14), header.Value);
        //        }
        //        else
        //        {
        //            pheaders.Add(header.Key.ToLower(), header.Value);
        //        }
        //    }
        //    var processed_headers = new Dictionary<string, Dictionary<string, string>>();
        //    processed_headers[CloudFilesConstants.ProcessedHeadersHeaderKey] = pheaders;
        //    processed_headers[CloudFilesConstants.ProcessedHeadersMetadataKey] = metadata;
        //    return processed_headers;
        //}

        public Dictionary<string, Dictionary<string, string>> ProcessMetadata(IList<HttpHeader> httpHeaders)
        {
            var pheaders = new Dictionary<string, string>();
            var metadata = new Dictionary<string, string>();
            foreach (var header in httpHeaders)
            {
                if (header.Key.ToLower().Contains(CloudFilesConstants.AccountMetaDataPrefix))
                {
                    metadata.Add(header.Key.Remove(0, 15), header.Value);
                }
                else if (header.Key.ToLower().Contains(CloudFilesConstants.ContainerMetaDataPrefix))
                {
                    metadata.Add(header.Key.Remove(0, 17), header.Value);
                }
                else if (header.Key.ToLower().Contains(CloudFilesConstants.ObjectMetaDataPrefix))
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
                    {CloudFilesConstants.ProcessedHeadersHeaderKey, pheaders},
                    {CloudFilesConstants.ProcessedHeadersMetadataKey, metadata}
                };

            return processedHeaders;
        }
    }
}
