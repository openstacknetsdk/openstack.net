using net.openstack.Core.Validators;
using net.openstack.Providers.Rackspace.Exceptions;

namespace net.openstack.Providers.Rackspace.Validators
{
    public class CloudBlockStorageValidator : IBlockStorageValidator
    {
        /// <summary>
        /// A default instance of <see cref="CloudBlockStorageValidator"/>.
        /// </summary>
        private static readonly CloudBlockStorageValidator _default = new CloudBlockStorageValidator();

        /// <summary>
        /// Gets a default implementation of <see cref="CloudBlockStorageValidator"/>.
        /// </summary>
        public static CloudBlockStorageValidator Default
        {
            get
            {
                return _default;
            }
        }

        public void ValidateVolumeSize(int size)
        {
            if (size < 100 || size > 1000)
                throw new InvalidVolumeSizeException(size);
        }
    }
}
