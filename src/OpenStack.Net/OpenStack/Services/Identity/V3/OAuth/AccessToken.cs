namespace OpenStack.Services.Identity.V3.OAuth
{
    using System;
    using System.Collections.Immutable;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class AccessToken : ExtensibleJsonObject
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private AccessTokenId _id;

        [JsonProperty("consumer_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ConsumerId _consumerId;

        [JsonProperty("project_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ProjectId _projectId;

        [JsonProperty("authorizing_user_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private UserId _authorizingUserId;

        [JsonProperty("expires_at", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private DateTimeOffset? _expiresAt;

        [JsonProperty("links", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableDictionary<string, string> _links;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessToken"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected AccessToken()
        {
        }

        public AccessTokenId Id
        {
            get
            {
                return _id;
            }
        }

        public ConsumerId ConsumerId
        {
            get
            {
                return _consumerId;
            }
        }

        public ProjectId ProjectId
        {
            get
            {
                return _projectId;
            }
        }

        public UserId AuthorizingUserId
        {
            get
            {
                return _authorizingUserId;
            }
        }

        public DateTimeOffset? ExpiresAt
        {
            get
            {
                return _expiresAt;
            }
        }

        public ImmutableDictionary<string, string> Links
        {
            get
            {
                return _links;
            }
        }
    }
}
