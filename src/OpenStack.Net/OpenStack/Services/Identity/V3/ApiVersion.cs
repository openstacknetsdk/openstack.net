namespace OpenStack.Services.Identity.V3
{
    using System;
    using System.Collections.ObjectModel;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class ApiVersion : ExtensibleJsonObject
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ApiVersionId _id;

        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ApiStatus _status;

        [JsonProperty("updated", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private DateTimeOffset? _updated;

        [JsonProperty("media-types", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private MediaType[] _mediaTypes;

        [JsonProperty("links", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Link[] _links;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiVersion"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected ApiVersion()
        {
        }

        public ApiVersionId Id
        {
            get
            {
                return _id;
            }
        }

        public ApiStatus Status
        {
            get
            {
                return _status;
            }
        }

        public DateTimeOffset? LastModified
        {
            get
            {
                return _updated;
            }
        }

        public ReadOnlyCollection<MediaType> MediaTypes
        {
            get
            {
                if (_mediaTypes == null)
                    return null;

                return new ReadOnlyCollection<MediaType>(_mediaTypes);
            }
        }

        public ReadOnlyCollection<Link> Links
        {
            get
            {
                if (_links == null)
                    return null;

                return new ReadOnlyCollection<Link>(_links);
            }
        }
    }
}
