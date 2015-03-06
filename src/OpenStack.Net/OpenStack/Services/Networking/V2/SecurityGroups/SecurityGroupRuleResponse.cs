namespace OpenStack.Services.Networking.V2.SecurityGroups
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class SecurityGroupRuleResponse : ExtensibleJsonObject
    {
        [JsonProperty("security_group_rule", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private SecurityGroupRule _securityGroupRule;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityGroupRuleResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected SecurityGroupRuleResponse()
        {
        }

        public SecurityGroupRule SecurityGroupRule
        {
            get
            {
                return _securityGroupRule;
            }
        }
    }
}
