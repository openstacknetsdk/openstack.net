using net.openstack.Core;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace
{
    public interface IExtendedIdentityProvider : IIdentityProvider
    {
        Role[] ListRoles(CloudIdentity identity);
        Role AddRole(CloudIdentity identity, Role role);
        Role GetRole(CloudIdentity identity, string roleId);
        bool AddRoleToUser(CloudIdentity identity, string userId, string roleId);
        bool DeleteRoleFromUser(CloudIdentity identity, string userId, string roleId);

        UserCredential UpdateUserCredentials(CloudIdentity identity, string userId, string apiKey);
        bool DeleteUserCredentials(CloudIdentity identity, string userId);
        bool SetUserPassword(CloudIdentity identity, string userId, string password);
    }
}
