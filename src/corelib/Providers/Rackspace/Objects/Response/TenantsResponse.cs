using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects.Response
{
    internal class TenantsResponse
    {
        public Tenant[] Tenants { get; private set; }
    }
}
