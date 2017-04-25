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
    public class Catalog
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("type")]
        public string Type { get;  set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("id")]
        public string Id { get;  set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("name")]
        public string  Name{ get;  set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("endpoints")]
        public Endpoint[] Endpoints { get;  set; }

    }
}
