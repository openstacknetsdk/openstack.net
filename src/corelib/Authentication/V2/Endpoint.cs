using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenStack.Serialization;
namespace OpenStack.Authentication.V3
{
    /// <summary>
    /// represents endpoint entity for keystone v3
    ///</summary>
    ///<seealso href="http://developer.openstack.org/api-ref-identity-v3.html#service-catalog-v3"/>
    public class Endpoint
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("interface")]
        public string Interface{ get;  set; } 

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("region")]
        public string Region { get;  set; } 

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("region_id`")]
        public string RegionId { get;  set; } 

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("url")]
        public string  Url { get;  set; } 

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("enabled")]
        public string  Enabled { get;  set; } 

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("service_id")]
        public string ServiceID { get;  set; } 

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("service_name")]
        public string ServiceName { get;  set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("service_type")]
        public string ServiceType { get;  set; }


    }
}
