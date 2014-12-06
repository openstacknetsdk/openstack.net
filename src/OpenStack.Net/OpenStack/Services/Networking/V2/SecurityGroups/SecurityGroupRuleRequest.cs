namespace OpenStack.Services.Networking.V2.SecurityGroups
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class SecurityGroupRuleRequest : ExtensibleJsonObject
    {
        [JsonProperty("security_group_rule", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private SecurityGroupRuleData _securityGroupRule;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityGroupRuleRequest"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected SecurityGroupRuleRequest()
        {
        }

        public SecurityGroupRuleRequest(SecurityGroupRuleData securityGroupRule)
        {
            _securityGroupRule = securityGroupRule;
        }

        public SecurityGroupRuleData SecurityGroupRule
        {
            get
            {
                return _securityGroupRule;
            }
        }
    }
}
