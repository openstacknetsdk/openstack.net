namespace OpenStack.Services.Identity.V3
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class DomainResponse : ExtensibleJsonObject
    {
        [JsonProperty("domain", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Domain _domain;

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected DomainResponse()
        {
        }

        public Domain Domain
        {
            get
            {
                return _domain;
            }
        }
    }
}
