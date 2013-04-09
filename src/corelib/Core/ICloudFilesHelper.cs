using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSIStudios.SimpleRESTServices.Client;

namespace net.openstack.Core
{
    public interface ICloudFilesHelper
    {
        void ValidateContainerName(string containerName);
        void ValidateObjectName(string objectName);
        Dictionary<string, Dictionary<string, string>> ProcessMetadata(IList<HttpHeader> httpHeaders);
    }
}
