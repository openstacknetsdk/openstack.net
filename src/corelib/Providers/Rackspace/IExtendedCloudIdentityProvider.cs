using net.openstack.Core;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace
{
    public interface IExtendedCloudIdentityProvider : ICloudIdentityProvider
    {
        Role[] ListRoles(CloudIdentity identity = null);
        Role AddRole(Role role, CloudIdentity identity = null);
        Role GetRole(string roleId, CloudIdentity identity = null);
        bool AddRoleToUser(string userId, string roleId, CloudIdentity identity = null);
        bool DeleteRoleFromUser(string userId, string roleId, CloudIdentity identity = null);

        UserCredential UpdateUserCredentials(string userId, string apiKey, CloudIdentity identity = null);
        bool DeleteUserCredentials(string userId, CloudIdentity identity = null);
        bool SetUserPassword(string userId, string password, CloudIdentity identity = null);
        bool SetUserPassword(User user, string password, CloudIdentity identity = null);
        bool SetUserPassword(string userId, string username, string password, CloudIdentity identity = null);
        UserCredential UpdateUserCredentials(User user, string apiKey, CloudIdentity identity = null);
        UserCredential UpdateUserCredentials(string userId, string username, string apiKey, CloudIdentity identity = null);
    }
}
