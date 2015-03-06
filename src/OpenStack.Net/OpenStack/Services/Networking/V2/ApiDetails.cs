namespace OpenStack.Services.Networking.V2
{
    using System.Collections.Immutable;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class ApiDetails : ExtensibleJsonObject
    {
        [JsonProperty("resources", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<Resource> _resources;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiDetails"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected ApiDetails()
        {
        }

        public ImmutableArray<Resource> Resources
        {
            get
            {
                return _resources;
            }
        }
    }
}
