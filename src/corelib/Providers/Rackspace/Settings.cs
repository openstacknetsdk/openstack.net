using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.openstack.corelib.Providers.Rackspace
{
    public class Settings
    {
        public const string USIdentityUrlBase = "https://identity.api.rackspacecloud.com";
        public const string LONIdentityUrlBase = "https://lon.identity.api.rackspacecloud.com";
        public const string GetAllRolesUrlFormat = "v2.0/OS-KSADM/roles";
        public const string GetRolesByUserUrlFormat = "v2.0/users/{userID}/roles";
        public const string GetUserByNameUrlFormat = "v2.0/users/?name={name}";
        public const string AddRoleToUserUrlFormat = "v2.0/users/{userID}/roles/OS-KSADM/{roleID}";
        public const string GetUserImpersonationTokenUrlFormat = "v2.0/RAX-AUTH/impersonation-tokens";
        public const string DeleteRoleFromUserUrlFormat = "v2.0/users/{userID}/roles/OS-KSADM/{roleID}";
    }
}
