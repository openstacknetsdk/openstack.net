using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenStack.Networking.v2;
using OpenStack.Serialization;

namespace OpenStack.Networking.v2.Layer3
{
    /// <summary>
    ///Regpresents the security group of the <see cref="NetworkingService"/> 
    /// </summary>
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "securitygroup")]
    public class SecurityGroup
    {
        /// <summary>
        ///the security group description
        /// </summary>
        [JsonProperty("description")]
        public string Description;

        /// <summary>
        ///the UUID of security group
        /// </summary>
        [JsonProperty("id")]
        public Identifier Id;

        /// <summary>
        /// the security group name
        /// </summary>
        [JsonProperty("name")]
        public string Name;

        /// <summary>
        ///A list of <see cref="SecurityGroup"/> objects.
        /// </summary>
        [JsonProperty("security_group_rules")]
        public IList<SecurityGroupRule> SecurityGroupRules;

        /// <summary>
        ///The UUId of tenant who owns the scurity group
        /// </summary>
        [JsonProperty("tenant_id")]
        public string TenantId;
    }
}
