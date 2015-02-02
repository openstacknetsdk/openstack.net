namespace OpenStack.Services.Identity.V3
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class DomainRequest : ExtensibleJsonObject
    {
        [JsonProperty("domain", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private DomainData _domain;

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainRequest"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected DomainRequest()
        {
        }

        public DomainRequest(DomainData domain)
        {
            _domain = domain;
        }

        public DomainData Domain
        {
            get
            {
                return _domain;
            }
        }
    }
}
