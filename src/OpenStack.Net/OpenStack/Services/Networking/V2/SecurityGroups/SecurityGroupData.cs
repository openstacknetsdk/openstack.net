namespace OpenStack.Services.Networking.V2.SecurityGroups
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class SecurityGroupData : ExtensibleJsonObject
    {
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _name;

        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _description;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityGroupData"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected SecurityGroupData()
        {
        }

        public SecurityGroupData(string name)
            : this(name, null)
        {
        }

        public SecurityGroupData(string name, string description)
        {
            _name = name;
            _description = description;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }
    }
}
