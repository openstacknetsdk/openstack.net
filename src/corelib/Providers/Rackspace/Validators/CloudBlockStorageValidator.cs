using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.openstack.Core;

namespace net.openstack.Providers.Rackspace.Validators
{
    public class CloudBlockStorageValidator : ICloudBlockStorageValidator
    {
        public void ValidateVolumeSize(int size)
        {   
            if (size < 100 || size > 1000)
                throw new ArgumentException("ERROR: The volume size value must be between 100 and 1000");
        }
    }
}
