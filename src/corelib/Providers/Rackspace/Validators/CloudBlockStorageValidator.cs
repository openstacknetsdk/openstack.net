using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.openstack.Core;
using net.openstack.Core.Validators;
using net.openstack.Providers.Rackspace.Exceptions;

namespace net.openstack.Providers.Rackspace.Validators
{
    public class CloudBlockStorageValidator : IBlockStorageValidator
    {
        public void ValidateVolumeSize(int size)
        {
            if (size < 100 || size > 1000)
                throw new InvalidVolumeSizeException(size);
        }
    }
}
