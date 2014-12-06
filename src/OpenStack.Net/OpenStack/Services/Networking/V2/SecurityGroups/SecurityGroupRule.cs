namespace OpenStack.Services.Networking.V2.SecurityGroups
{
    using Newtonsoft.Json;
    using OpenStack.Services.Identity;

    [JsonObject(MemberSerialization.OptIn)]
    public class SecurityGroupRule : SecurityGroupRuleData
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private SecurityGroupRuleId _id;

        [JsonProperty("tenant_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ProjectId _projectId;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityGroupRule"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected SecurityGroupRule()
        {
        }

        public SecurityGroupRuleId Id
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
    }
}
