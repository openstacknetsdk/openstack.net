namespace OpenStack.Services.Identity.V3
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class GroupResponse : ExtensibleJsonObject
    {
        [JsonProperty("group", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Group _group;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected GroupResponse()
        {
        }

        public Group Group
        {
            get
            {
                return _group;
            }
        }
    }
}
