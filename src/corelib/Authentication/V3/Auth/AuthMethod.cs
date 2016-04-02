using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace OpenStack.Authentication.V3.Auth
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthMethodType
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly string Type;

        /// <summary>
        /// 
        /// </summary>
        public readonly static AuthMethodType Password = new AuthMethodType("password");

        /// <summary>
        /// 
        /// </summary>
        public readonly static AuthMethodType Token = new AuthMethodType("token");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public AuthMethodType(string type)
        {
            Type = type;
        }

    }
    /// <summary>
    /// 
    /// </summary>
    public class AuthMethod
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("methods")]
        public IList<string> Methods;

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("password")]
        public IDictionary<string,  AuthUser> Password;

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("token")]
        public IDictionary<string,  string> Token;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        public AuthMethod(AuthUser user)
        {
            Methods = new List<string> { AuthMethodType.Password.Type};
            Password = new Dictionary<string, AuthUser>()
            {
                {"user", user }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokenId"></param>
        public AuthMethod(string tokenId)
        {
            Methods = new List<string> { AuthMethodType.Token.Type};
            Token = new Dictionary<string, string>()
            {
                {"id", tokenId  }
            };
        }


    }
}
