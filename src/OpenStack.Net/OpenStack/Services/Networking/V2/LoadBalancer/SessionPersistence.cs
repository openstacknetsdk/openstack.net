namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class SessionPersistence : ExtensibleJsonObject
    {
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private SessionPersistenceType _type;

        [JsonProperty("cookie_name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string _cookieName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionPersistence"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected SessionPersistence()
        {
        }

        public SessionPersistence(SessionPersistenceType type)
            : this(type, null)
        {
        }

        public SessionPersistence(SessionPersistenceType type, string cookieName)
        {
            _type = type;
            _cookieName = cookieName;
        }

        public SessionPersistenceType Type
        {
            get
            {
                return _type;
            }
        }

        public string CookieName
        {
            get
            {
                return _cookieName;
            }
        }
    }
}
