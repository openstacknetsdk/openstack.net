﻿using net.openstack.Core.Domain;

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
        string GetToken(CloudIdentity identity, bool forceCacheRefresh = false);
        IdentityToken GetTokenInfo(CloudIdentity identity, bool forceCacheRefresh = false);
        
        //User[] ListUsers(CloudIdentity identity);
        User GetUserByName(CloudIdentity identity, string name);
        User GetUser(CloudIdentity identity, string id);
        //void AddUser(User user, CloudIdentity identity);
        bool UpdateUser(CloudIdentity identity, User user);
        //void DeleteUser(string userId, CloudIdentity identity);
        UserAccess Authenticate(CloudIdentity identity, bool forceCacheRefresh = false);
    }
}
