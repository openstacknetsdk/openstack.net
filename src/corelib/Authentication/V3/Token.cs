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
    public class Token
    {
		/// <summary>
        /// 
        /// </summary>
		public string Id { get; set; }

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
		public User User { get;  set; }

		/// <summary>
        /// 
        /// </summary>
        public bool IsExpired
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void SetIdentity(string id)
        {
            this.Id = id;
        }
    }
}
