namespace OpenStack.Services.Identity.V3
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
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

        public DomainRequest(DomainData domain, params JProperty[] extensionData)
            : base(extensionData)
        {
            _domain = domain;
        }

        public DomainRequest(DomainData domain, IDictionary<string, JToken> extensionData)
            : base(extensionData)
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
