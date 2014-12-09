namespace OpenStack.Services.Identity.V3
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class RoleData : ExtensibleJsonObject
    {
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleData"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected RoleData()
        {
        }

        public RoleData(string name)
        {
            _name = name;
        }

        public RoleData(string name, params JProperty[] extensionData)
            : base(extensionData)
        {
            _name = name;
        }

        public RoleData(string name, IDictionary<string, JToken> extensionData)
            : base(extensionData)
        {
            _name = name;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }
    }
}
