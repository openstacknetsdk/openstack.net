using net.openstack.Core.Domain;

namespace net.openstack.Core
{
    public interface IIdentityProvider
    {
        Role[] ListRoles(CloudIdentity identity);
        //void AddRole(Role role, CloudIdentity identity);
        //void GetRole(string roleId, CloudIdentity identity);
        Role[] GetRolesByUser(string userId, CloudIdentity identity);
        bool AddRoleToUser(string userId, string roleId, CloudIdentity identity);
        bool DeleteRoleFromUser(string userId, string roleId, CloudIdentity identity);
        string GetToken(CloudIdentity identity);
        IdentityToken GetTokenInfo(CloudIdentity identity);
        
        //User[] ListUsers(CloudIdentity identity);
        User GetUserByName(string name, CloudIdentity identity);
        //User GetUser(string id, CloudIdentity identity);
        //void AddUser(User user, CloudIdentity identity);
        //void UpdateUser(User user, CloudIdentity identity);
        //void DeleteUser(string userId, CloudIdentity identity);
        UserAccess Authenticate(CloudIdentity identity);
    }
}
