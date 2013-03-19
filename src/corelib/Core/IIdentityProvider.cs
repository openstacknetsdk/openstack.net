using net.openstack.Core.Domain;

namespace net.openstack.Core
{
    public interface IIdentityProvider
    {
        UserAccess Authenticate(CloudIdentity identity = null);
        string GetToken(CloudIdentity identity = null, bool forceCacheRefresh = false);
        IdentityToken GetTokenInfo(CloudIdentity identity = null, bool forceCacheRefresh = false);

        Role[] GetRolesByUser(string userId, CloudIdentity identity = null);

        User[] ListUsers(CloudIdentity identity = null);
        User GetUserByName(string name, CloudIdentity identity = null);
        User GetUser(string id, CloudIdentity identity = null);
        NewUser AddUser(NewUser user, CloudIdentity identity = null);
        User UpdateUser(User user, CloudIdentity identity = null);
        bool DeleteUser(string userId, CloudIdentity identity = null);

        UserCredential[] ListUserCredentials(string userId, CloudIdentity identity = null);

        Tenant[] ListTenants(CloudIdentity identity = null);
        UserAccess GetUserAccess(CloudIdentity identity = null, bool forceCacheRefresh = false);
        UserCredential GetUserCredential(string userId, string credentialKey, CloudIdentity identity = null);
    }
}
