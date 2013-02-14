using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.openstack.Core
{
    public interface IObjectStoreValidator
    {
        void ValidateContainerName(string containerName);
    }
}
