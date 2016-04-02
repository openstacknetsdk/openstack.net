using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenStack.Serialization;
namespace OpenStack.Authentication.V3.Auth
{
    /// <summary>
    /// 
    /// </summary>
    public class UserCredential
    {

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("scope")]
        public AuthScope Scope { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("identity")]
        public AuthMethod Identity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="password"></param>
        /// <param name="scopeIdentifier"></param>
        /// <param name="scopeType"></param>
        public UserCredential(string userid, string password, string scopeIdentifier, AuthScopeType scopeType)
        {
            var user = new AuthUser(userid, password);
            Identity = new AuthMethod(user);
            if(scopeIdentifier != null && scopeType !=null)
                Scope = new AuthScope(scopeIdentifier, scopeType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="scope"></param>
        public UserCredential(AuthUser user, AuthScope scope)
        {
            Identity = new AuthMethod(user);
            Scope = scope;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokenId"></param>
        /// <param name="scopeIdentifier"></param>
        /// <param name="scopeType"></param>
        public UserCredential(string tokenId, string scopeIdentifier, AuthScopeType scopeType)
        {
            Identity = new AuthMethod(tokenId);
            Scope = new AuthScope(scopeIdentifier, scopeType);
        }
    }
}
