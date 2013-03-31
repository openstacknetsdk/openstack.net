using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.openstack.Core.Domain
{
    public enum ObjectStore
    {
        Unknown,
        ContainerCreated,
        ContainerExists,
        ContainerDeleted,
        ContainerNotEmpty,
        ContainerNotFound,
        ObjectDeleted,
        ObjectCreated
    }
}
