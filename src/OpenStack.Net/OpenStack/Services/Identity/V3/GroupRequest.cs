namespace OpenStack.Services.Identity.V3
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class GroupRequest : ExtensibleJsonObject
    {
        [JsonProperty("group", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private GroupData _group;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupRequest"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected GroupRequest()
        {
        }

        public GroupRequest(GroupData group)
        {
        }

        public GroupRequest(GroupData group, params JProperty[] extensionData)
            : base(extensionData)
        {
        }

        public GroupRequest(GroupData group, IDictionary<string, JToken> extensionData)
            : base(extensionData)
        {
        }

        public GroupData Group
        {
            get
            {
                return _group;
            }
        }
    }
}
