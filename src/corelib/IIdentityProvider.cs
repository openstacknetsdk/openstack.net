using net.openstack.corelib.Providers.Rackspace.Objects;

namespace net.openstack.corelib
{
    public interface IIdentityProvider
    {
        Role[] GetAllRoles(CloudIdentity identity);
        Role[] GetRolesByUser(string userId, CloudIdentity identity);
        User GetUserByName(string name, CloudIdentity identity);
        bool AddRoleToUser(string userId, string roleId, CloudIdentity identity);
        string GetUserImpersonationToken(string userName, CloudIdentity identity);
        string GetToken(CloudIdentity identity);
        bool DeleteRoleFromUser(string userId, string roleId, CloudIdentity identity);
        IdentityToken GetTokenInfo(CloudIdentity identity);
    }

    public class User
    {
        public string id { get; set; }

        public string username { get; set; }

        public string email { get; set; }

        public bool enabled { get; set; }
    }
}
