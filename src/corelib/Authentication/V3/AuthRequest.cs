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
    public class AuthRequest
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("auth")]
        public Credential Auth { get;  set;  }

        /// <summary>
        /// 
        /// </summary>
        public AuthRequest(Credential credential)
        {
            this.Auth = credential;
        }
    }
}
