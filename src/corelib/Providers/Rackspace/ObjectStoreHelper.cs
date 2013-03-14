using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using JSIStudios.SimpleRESTServices.Client;
using net.openstack.Core;
using net.openstack.Core.Exceptions;

namespace net.openstack.Providers.Rackspace
{
    public class ObjectStoreHelper : IObjectStoreHelper
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
        //        if (header.Key.ToLower().Contains(ObjectStoreConstants.AccountMetaDataPrefix))
        //        {
        //            metadata.Add(header.Key.Remove(0, 15), header.Value);
        //        }
        //        else if (header.Key.ToLower().Contains(ObjectStoreConstants.ContainerMetaDataPrefix))
        //        {
        //            metadata.Add(header.Key.Remove(0, 17), header.Value);
        //        }
        //        else if (header.Key.ToLower().Contains(ObjectStoreConstants.ObjectMetaDataPrefix))
        //        {
        //            metadata.Add(header.Key.Remove(0, 14), header.Value);
        //        }
        //        else
        //        {
        //            pheaders.Add(header.Key.ToLower(), header.Value);
        //        }
        //    }
        //    var processed_headers = new Dictionary<string, Dictionary<string, string>>();
        //    processed_headers[ObjectStoreConstants.ProcessedHeadersHeaderKey] = pheaders;
        //    processed_headers[ObjectStoreConstants.ProcessedHeadersMetadataKey] = metadata;
        //    return processed_headers;
        //}

        public Dictionary<string, Dictionary<string, string>> ProcessMetadata(IList<HttpHeader> httpHeaders)
        {
            var pheaders = new Dictionary<string, string>();
            var metadata = new Dictionary<string, string>();
            foreach (var header in httpHeaders)
            {
                if (header.Key.ToLower().Contains(ObjectStoreConstants.AccountMetaDataPrefix))
                {
                    metadata.Add(header.Key.Remove(0, 15), header.Value);
                }
                else if (header.Key.ToLower().Contains(ObjectStoreConstants.ContainerMetaDataPrefix))
                {
                    metadata.Add(header.Key.Remove(0, 17), header.Value);
                }
                else if (header.Key.ToLower().Contains(ObjectStoreConstants.ObjectMetaDataPrefix))
                {
                    metadata.Add(header.Key.Remove(0, 14), header.Value);
                }
                else
                {
                    pheaders.Add(header.Key.ToLower(), header.Value);
                }
            }
            var processed_headers = new Dictionary<string, Dictionary<string, string>>();
            processed_headers[ObjectStoreConstants.ProcessedHeadersHeaderKey] = pheaders;
            processed_headers[ObjectStoreConstants.ProcessedHeadersMetadataKey] = metadata;
            return processed_headers;
        }
    }
}
