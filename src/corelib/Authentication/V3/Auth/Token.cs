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
    public class Token
    {
		/// <summary>
        /// 
        /// </summary>
		[JsonProperty("id")]
		public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("project")]
		public IDictionary<string, object> Project { get;  set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("domain")]
		public IDictionary<string, object> Domain { get;  set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("methods")]
		public string [] Methods { get;  set; }

		/// <summary>
        /// 
        /// </summary>
        [JsonProperty("expires_at")]
        public DateTimeOffset? ExpiresAt { get;  set; }

        /// <summary>
        ///
        /// </summary>
        [JsonProperty("issued_at")]
        public DateTimeOffset? IssuedAt { get;  set; }

		/// <summary>
        /// </summary>
		[JsonProperty("catalog")]
		public Catalog[] Catalog { get;  set; }

        /// <summary>
        /// 
        /// </summary>
		[JsonProperty("user")]
		public AuthUser User { get;  set; }

		/// <summary>
        /// 
        /// </summary>
        public bool IsExpired
        {
            get
            {
                return DateTime.Now > ExpiresAt;
            }
        }
        ///
        public void SetId(string id)
        {
            this.Id = id;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("token")]
        public Token Token;
    }
}
