using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenStack.Authentication.V3
{
    /// <summary>
    /// 
    /// </summary>
    public class PasswordIdentity
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("methods")]
        public IList<string> methods { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("password")]
        public IDictionary<string, User> password { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="password"></param>
        public PasswordIdentity(string userid, string password)
        {
            this.password = new Dictionary<string, User>() {
                { "user", new User(userid, password) }
            };
            this.methods = new List<string>{ "password"};
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        public PasswordIdentity(User user)
        {
            this.password = new Dictionary<string, User>() {
                { "user", user }
            };
            this.methods = new List<string>{ "password"};
        }
    }
}
