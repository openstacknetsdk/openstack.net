using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenStack.Authentication.V3.Auth
{
    /// <summary>
    /// represent unique user in openstack identity
    /// </summary>
    public class AuthUser
    {
        /// <summary>
        /// UUId of a user that is unique in global scope 
        /// </summary>
        [JsonProperty("id")]
        public string Id { get;  set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("password")]
        public string Password { get;  set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("name")]
        public string Name { get;  set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("domain")]
        public IDictionary<string, string> Domain;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        public AuthUser(string userId, string password)
        {
            if (userId == null)
                throw new ArgumentNullException("userId");
            this.Id = userId;
            this.Password = password;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="domainId"></param>
        public  AuthUser(string userName, string password, string domainId)
        {
            if (userName == null)
                throw new ArgumentNullException("userName");
            if (domainId == null)
                throw new ArgumentNullException("domainId");

            this.Name = userName;
            this.Domain = new Dictionary<string, string>() {
                { "id", domainId }
            };
            this.Password = password;
        }
    }
}
