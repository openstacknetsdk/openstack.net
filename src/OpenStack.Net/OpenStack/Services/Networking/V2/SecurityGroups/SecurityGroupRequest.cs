namespace OpenStack.Services.Networking.V2.SecurityGroups
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class SecurityGroupRequest : ExtensibleJsonObject
    {
        [JsonProperty("security_group", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private SecurityGroupData _securityGroup;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityGroupRequest"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected SecurityGroupRequest()
        {
        }

        public SecurityGroupRequest(SecurityGroupData securityGroup)
        {
            _securityGroup = securityGroup;
        }

        public SecurityGroupData SecurityGroup
        {
            get
            {
                return _securityGroup;
            }
        }
    }
}
