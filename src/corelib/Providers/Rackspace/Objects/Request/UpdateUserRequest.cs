using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Request
{
    internal class UpdateUserRequest
    {
        public User User { get; set; }
    }
}
