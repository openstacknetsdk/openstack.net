using System;
using System.Web;
using net.openstack.Core;
using net.openstack.Core.Exceptions;
using net.openstack.Core.Validators;

namespace net.openstack.Providers.Rackspace.Validators
{
    public class CloudFilesValidator : IObjectStorageValidator
    {
        /// <summary>
        /// A default instance of <see cref="CloudBlockStorageValidator"/>.
        /// </summary>
        private static readonly CloudFilesValidator _default = new CloudFilesValidator();

        /// <summary>
        /// Gets a default instance of <see cref="CloudBlockStorageValidator"/>.
        /// </summary>
        public static CloudFilesValidator Default
        {
            get
            {
                return _default;
            }
        }

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
    }
}