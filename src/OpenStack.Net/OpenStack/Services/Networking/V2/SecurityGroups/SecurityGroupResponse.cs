namespace OpenStack.Services.Networking.V2.SecurityGroups
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class SecurityGroupResponse : ExtensibleJsonObject
    {
        [JsonProperty("security_group", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private SecurityGroup _securityGroup;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityGroupResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected SecurityGroupResponse()
        {
        }

        public SecurityGroup SecurityGroup
        {
            get
            {
                return _securityGroup;
            }
        }
    }
}
