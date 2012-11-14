using net.openstack.Core.Domain;

namespace net.openstack.Core
{
    public interface IIdentityProvider
    {
        Role[] ListRoles(CloudIdentity identity);
        //void AddRole(Role role, CloudIdentity identity);
        //void GetRole(string roleId, CloudIdentity identity);
        Role[] GetRolesByUser(CloudIdentity identity, string userId);
        bool AddRoleToUser(CloudIdentity identity, string userId, string roleId);
        bool DeleteRoleFromUser(CloudIdentity identity, string userId, string roleId);
        string GetToken(CloudIdentity identity);
        IdentityToken GetTokenInfo(CloudIdentity identity);
        
        //User[] ListUsers(CloudIdentity identity);
        User GetUserByName(CloudIdentity identity, string name);
        //User GetUser(string id, CloudIdentity identity);
        //void AddUser(User user, CloudIdentity identity);
        //void UpdateUser(User user, CloudIdentity identity);
        //void DeleteUser(string userId, CloudIdentity identity);
        UserAccess Authenticate(CloudIdentity identity);
    }
}
