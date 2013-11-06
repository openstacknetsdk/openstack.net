using net.openstack.Core;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace
{
    public interface IExtendedIdentityProvider : IIdentityProvider
    {
        Role[] ListRoles(CloudIdentity identity, string serviceId = null, int? marker = null, int? limit = 10000);
        Role AddRole(CloudIdentity identity, Role role);
        Role GetRole(CloudIdentity identity, string roleId);
        bool AddRoleToUser(CloudIdentity identity, string userId, string roleId);
        bool DeleteRoleFromUser(CloudIdentity identity, string userId, string roleId);

        UserCredential UpdateUserCredentials(CloudIdentity identity, string userId, string apiKey);
        bool DeleteUserCredentials(CloudIdentity identity, string userId);
        bool SetUserPassword(CloudIdentity identity, string userId, string password);
        bool SetUserPassword(CloudIdentity identity, User user, string password);
        bool SetUserPassword(CloudIdentity identity, string userId, string username, string password);
        UserCredential UpdateUserCredentials(CloudIdentity identity, User user, string apiKey);
        UserCredential UpdateUserCredentials(CloudIdentity identity, string userId, string username, string apiKey);
    }
}
