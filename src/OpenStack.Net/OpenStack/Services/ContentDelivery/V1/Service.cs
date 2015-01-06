namespace OpenStack.Services.ContentDelivery.V1
{
    using System.Collections.Immutable;
    using Newtonsoft.Json;
    using OpenStack.Services.Identity;

    [JsonObject(MemberSerialization.OptIn)]
    public class Service : ServiceData
    {
#pragma warning disable 649 // Field 'fieldName' is never assigned to, and will always have its default value {value}
        /// <summary>
        /// This is the backing field for the <see cref="Id"/> property.
        /// </summary>
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ServiceId _id;

        /// <summary>
        /// This is the backing field for the <see cref="Status"/> property.
        /// </summary>
        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ServiceStatus _status;

        /// <summary>
        /// This is the backing field for the <see cref="Errors"/> property.
        /// </summary>
        [JsonProperty("errors", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<ServiceError> _errors;

        /// <summary>
        /// This is the backing field for the <see cref="Links"/> property.
        /// </summary>
        [JsonProperty("links", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<Link> _links;
#pragma warning restore 649

        /// <summary>
        /// Initializes a new instance of the <see cref="Service"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected Service()
        {
        }

        public ServiceId Id
        {
            get
            {
                return _id;
            }
        }

        public ServiceStatus Status
        {
            get
            {
                return _status;
            }
        }

        public ImmutableArray<ServiceError> Errors
        {
            get
            {
                return _errors;
            }
        }

        public ImmutableArray<Link> Links
        {
            get
            {
                return _links;
            }
        }
    }
}
