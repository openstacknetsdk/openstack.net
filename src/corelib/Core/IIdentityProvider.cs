using net.openstack.Core.Domain;

namespace net.openstack.Core
{
    public interface IIdentityProvider
    {
        UserAccess Authenticate(CloudIdentity identity);
        string GetToken(CloudIdentity identity, bool forceCacheRefresh = false);
        IdentityToken GetTokenInfo(CloudIdentity identity, bool forceCacheRefresh = false);

        Role[] GetRolesByUser(CloudIdentity identity, string userId);

        User[] ListUsers(CloudIdentity identity);
        User GetUserByName(CloudIdentity identity, string name);
        User GetUser(CloudIdentity identity, string id);
        NewUser AddUser(CloudIdentity identity, NewUser user);
        User UpdateUser(CloudIdentity identity, User user);
        bool DeleteUser(CloudIdentity identity, string userId);

        UserCredential[] ListUserCredentials(CloudIdentity identity, string userId);

        Tenant[] ListTenants(CloudIdentity identity);
        UserAccess GetUserAccess(CloudIdentity identity, bool forceCacheRefresh = false);
        UserCredential GetUserCredential(CloudIdentity identity, string userId, string credentialKey);
    }
}
