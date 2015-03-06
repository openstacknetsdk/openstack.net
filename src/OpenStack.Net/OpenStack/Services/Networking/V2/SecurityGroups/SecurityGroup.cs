namespace OpenStack.Services.Networking.V2.SecurityGroups
{
    using System.Collections.Immutable;
    using Newtonsoft.Json;
    using OpenStack.Services.Identity;

    [JsonObject(MemberSerialization.OptIn)]
    public class SecurityGroup : SecurityGroupData
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private SecurityGroupId _id;

        [JsonProperty("security_group_rules", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<SecurityGroupRule> _rules;

        [JsonProperty("tenant_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ProjectId _projectId;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityGroup"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected SecurityGroup()
        {
        }

        public SecurityGroupId Id
        {
            get
            {
                return _id;
            }
        }

        public ProjectId ProjectId
        {
            get
            {
                return _projectId;
            }
        }

        public ImmutableArray<SecurityGroupRule> Rules
        {
            get
            {
                return _rules;
            }
        }
    }
}
