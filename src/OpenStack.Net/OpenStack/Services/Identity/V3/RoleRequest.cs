namespace OpenStack.Services.Identity.V3
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class RoleRequest : ExtensibleJsonObject
    {
        [JsonProperty("role", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private RoleData _role;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleRequest"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected RoleRequest()
        {
        }

        public RoleRequest(RoleData role)
        {
            _role = role;
        }

        public RoleRequest(RoleData role, params JProperty[] extensionData)
            : base(extensionData)
        {
            _role = role;
        }

        public RoleRequest(RoleData role, IDictionary<string, JToken> extensionData)
            : base(extensionData)
        {
            _role = role;
        }

        public RoleData Role
        {
            get
            {
                return _role;
            }
        }
    }
}
