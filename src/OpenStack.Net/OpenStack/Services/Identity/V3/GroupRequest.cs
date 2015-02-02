namespace OpenStack.Services.Identity.V3
{
    using Newtonsoft.Json;
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

        public GroupData Group
        {
            get
            {
                return _group;
            }
        }
    }
}
