using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenStack.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class Project
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("description")]
        public string Description;
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("domain_id")]
        public string DomainId;
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("enabled")]
        public bool Enabled;
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("id")]
        public string Id;
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("admin")]
        public string Name;
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("parent_id")]
        public string parentId;
    }
}
