using net.openstack.Core.Domain;

namespace net.openstack.Core
{
    public interface IIdentityProvider
    {
        UserAccess Authenticate(CloudIdentity identity, bool forceCacheRefresh = false);

        Role[] ListRoles(CloudIdentity identity);
        Role AddRole(CloudIdentity identity, Role role);
        Role GetRole(CloudIdentity identity, string roleId);
        Role[] GetRolesByUser(CloudIdentity identity, string userId);
        bool AddRoleToUser(CloudIdentity identity, string userId, string roleId);
        bool DeleteRoleFromUser(CloudIdentity identity, string userId, string roleId);
        string GetToken(CloudIdentity identity, bool forceCacheRefresh = false);
        IdentityToken GetTokenInfo(CloudIdentity identity, bool forceCacheRefresh = false);
        
        User[] ListUsers(CloudIdentity identity);
        User GetUserByName(CloudIdentity identity, string name);
        User GetUser(CloudIdentity identity, string id);
        User AddUser(CloudIdentity identity, User user);
        User UpdateUser(CloudIdentity identity, User user);
        bool DeleteUser(CloudIdentity identity, string userId);

        bool SetUserPassword(CloudIdentity identity, string userId, string password);

        UserCredential[] ListUserCredentials(CloudIdentity identity, string userId);
        UserCredential UpdateUserCredentials(CloudIdentity identity, string userId);
        bool DeleteUserCredentials(CloudIdentity identity, string userId);
    }
}
