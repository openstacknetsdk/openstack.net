using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using net.openstack.Core;
using net.openstack.Core.Exceptions;

namespace net.openstack.Providers.Rackspace
{
    public class ObjectStoreValidator : IObjectStoreValidator
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
    }
}
