using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using net.openstack.corelib.Providers.Rackspace.Objects;
using net.openstack.corelib.Web;
using net.openstack.corelib.Web.Json;

namespace net.openstack.corelib.Providers.Rackspace
{
    public class GeographicalIdentityProvider : IdentityProviderBase, IIdentityProvider
    {
        public GeographicalIdentityProvider(Uri urlBase)
            : this(urlBase, new JsonRestServices(), new IdentityTokenCache())
        {
        }
        public GeographicalIdentityProvider(Uri urlBase, IRestService restService, ICache<IdentityToken> tokenCache) 
            : base(urlBase, restService, tokenCache)
        {
        }

        public Role[] GetAllRoles(CloudIdentity identity)
        {
            const string urlPath = Settings.GetAllRolesUrlFormat;
            var response = ExecuteRESTRequest<RolesResponse>(urlPath, HttpMethod.GET, null, identity);
            if (response == null || response.Data == null)
                return null;

            return response.Data.roles;
        }

        public Role[] GetRolesByUser(string userId, CloudIdentity identity)
        {
            const string urlFormat = Settings.GetRolesByUserUrlFormat;
            var urlPath = urlFormat.Format(new Dictionary<string, string>() { { "userID", userId } });
            var response = ExecuteRESTRequest<RolesResponse>(urlPath, HttpMethod.GET, null, identity);

            if (response == null || response.Data == null)
                return null;

            return response.Data.roles;
        }

        public User GetUserByName(string name, CloudIdentity identity)
        {
            const string urlFormat = Settings.GetUserByNameUrlFormat;
            var urlPath = urlFormat.Format(new Dictionary<string, string>() { { "name", name } });
            var response = ExecuteRESTRequest<UserResponse>(urlPath, HttpMethod.GET, null, identity);

            if (response == null || response.Data == null)
                return null;

            return response.Data.user;
        }

        public bool AddRoleToUser(string userId, string roleId, CloudIdentity identity)
        {
            const string urlFormat = Settings.AddRoleToUserUrlFormat;
            var urlPath = urlFormat.Format(new Dictionary<string, string>() { { "userID", userId }, { "roleID", roleId } });
            var response = ExecuteRESTRequest<object>(urlPath, HttpMethod.PUT, null, identity);

            // If the response status code is 409, that mean the user is already apart of the role, so we want to return true;
            if (response == null || (response.StatusCode >= 400 && response.StatusCode != 409))
                return false;

            return true;
        }

        public string GetUserImpersonationToken(string userName, CloudIdentity identity)
        {
            const string urlPath = Settings.GetUserImpersonationTokenUrlFormat;
            var request = BuildImpersonationRequestJson(urlPath, userName, 600);
            var response = ExecuteRESTRequest<UserImpersonationResponse>(urlPath, HttpMethod.POST, request, identity);

            if (response == null || response.Data == null || response.Data.access == null || response.Data.access.token == null)
                return null;

            return response.Data.access.token.id;
        }

        public bool DeleteRoleFromUser(string userId, string roleId, CloudIdentity identity)
        {
            const string urlFormat = Settings.DeleteRoleFromUserUrlFormat;
            var urlPath = urlFormat.Format(new Dictionary<string, string>() { { "userID", userId }, { "roleID", roleId } });
            var response = ExecuteRESTRequest<object>(urlPath, HttpMethod.DELETE, null, identity);

            if (response != null && response.StatusCode == 204)
                return true;
            
            return false;
        }

        #region

        private JObject BuildImpersonationRequestJson(string path, string userName, int expirationInSeconds)
        {
            var request = new JObject();
            var impInfo = new JObject();
            var user = new JObject { { "username", userName }, { "expire-in-seconds", expirationInSeconds } };
            impInfo.Add("user", user);
            var parts = path.Split('/');
            request.Add(string.Format("{0}:impersonation", parts[1]), impInfo);

            return request;
        }

        #endregion
    }

    public class UserResponse
    {
        public User user { get; set; }
    }

    public class RolesResponse
    {
        public Role[] roles { get; set; }
        public string[] roles_links { get; set; }
    }

    public class UserImpersonationResponse
    {
        public Access access { get; set; }
    }
}
