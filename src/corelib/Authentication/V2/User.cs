using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenStack.Authentication.V3
{
    /// <summary>
    /// represent unique user in openstack identity
    /// </summary>
    public class User
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
        /// <param name="UserId"></param>
        /// <param name="Password"></param>
        public User(string UserId, string Password)
        {
            this.Id = UserId;
            this.Password = Password;
        }
    }
}
