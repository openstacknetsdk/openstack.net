using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.openstack.corelib.Providers.Rackspace.Objects
{
    public class TokenResponse
    {
        public Access access { get; set; }
    }

    public class Access
    {
        public Token token { get; set; }

        public TokenUser user { get; set; }

        public ServiceCatalog[] ServiceCatalog { get; set; }
    }

    public class ServiceCatalog
    {
        public Endpoint[] endpoints { get; set; }

        public string name { get; set; }

        public string type { get; set; }
    }

    public class Endpoint
    {
        public string publicURL { get; set; }

        public string region { get; set; }

        public string tenantId { get; set; }

        public string versionId { get; set; }

        public string versionInfo { get; set; }

        public string versionList { get; set; }
    }

    public class TokenUser
    {
        public string id { get; set; }

        public string name { get; set; }

        public Role[] roles { get; set; }
    }

    public class Role
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class Token
    {
        public string expires { get; set; }

        public string id { get; set; }
    }
}
