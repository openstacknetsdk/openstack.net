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
    public class AuthScopeType
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly string Type;

        /// <summary>
        /// 
        /// </summary>
        public static readonly AuthScopeType Project = new AuthScopeType("project");

        /// <summary>
        /// 
        /// </summary>
        public static readonly AuthScopeType Domain = new AuthScopeType("domain");
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public AuthScopeType(string type)
        {
            Type = type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool Equals(AuthScopeType left, AuthScopeType right)
        {
            return string.Equals(left.Type, right.Type);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class AuthScope
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("project")]
        public IDictionary<string, string> Project;

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("domain")]
        public IDictionary<string, string> Domain;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scopeIdentifier"></param>
        /// <param name="type"></param>
        public AuthScope(string scopeIdentifier, AuthScopeType type)
        {
            if (type != null && type != AuthScopeType.Domain && type != AuthScopeType.Project)
                throw new ArgumentException("scopeType must be 'project' or 'domaon'");

            if(type != null && scopeIdentifier == null)
            {
                throw new ArgumentNullException("scopeIdentifier");
            }
            
            if(type == AuthScopeType.Project) 
                this.Project = new Dictionary<string, string>() {
                    { "id", scopeIdentifier }
                };
            if(type == AuthScopeType.Domain)
                this.Domain = new Dictionary<string, string>() {
                    { "id", scopeIdentifier }
                };
        }
    }
}
