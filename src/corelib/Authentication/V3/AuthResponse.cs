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
    public class AuthResponse
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("token")]
        public Token Token { get;  set; }
    }
}
