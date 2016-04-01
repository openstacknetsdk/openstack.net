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
    public class Credential
    {
        /// <summary>
        /// 
        /// </summary>

        [JsonProperty("scope")]
        public ProjectScope projectScope { get;  set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("identity")]
        public PasswordIdentity passwordIdentity { get; set;  }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="password"></param>
        public Credential(string userid, string password)
        {
            this.passwordIdentity = new PasswordIdentity(userid, password);
        }
    }
}
