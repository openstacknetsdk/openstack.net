namespace OpenStack.Services.Identity.V3
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class PasswordIdentityData : IdentityData
    {
        [JsonProperty("user", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private UserData _user;

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordIdentityData"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected PasswordIdentityData()
        {
        }

        public PasswordIdentityData(UserData user)
            : base(new[] { AuthenticationMethod.Password })
        {
            _user = user;
        }

        public PasswordIdentityData(UserData user, params JProperty[] extensionData)
            : base(new[] { AuthenticationMethod.Password }, extensionData)
        {
            _user = user;
        }

        public PasswordIdentityData(UserData user, IDictionary<string, JToken> extensionData)
            : base(new[] { AuthenticationMethod.Password }, extensionData)
        {
            _user = user;
        }

        [JsonObject(MemberSerialization.OptIn)]
        public class UserData : ExtensibleJsonObject
        {
            [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
            private UserId _id;

            [JsonProperty("password", DefaultValueHandling = DefaultValueHandling.Ignore)]
            private string _password;

            [JsonProperty("domain", DefaultValueHandling = DefaultValueHandling.Ignore)]
            private DomainData _domain;

            /// <summary>
            /// Initializes a new instance of the <see cref="UserData"/> class
            /// during JSON deserialization.
            /// </summary>
            [JsonConstructor]
            protected UserData()
            {
            }

            public UserId Id
            {
                get
                {
                    return _id;
                }
            }

            public string Password
            {
                get
                {
                    return _password;
                }
            }

            public DomainData Domain
            {
                get
                {
                    return _domain;
                }
            }
        }

        [JsonObject(MemberSerialization.OptIn)]
        public class DomainData : ExtensibleJsonObject
        {
            [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
            private string _name;

            /// <summary>
            /// Initializes a new instance of the <see cref="DomainData"/> class
            /// during JSON deserialization.
            /// </summary>
            [JsonConstructor]
            protected DomainData()
            {
            }

            public DomainData(string name)
            {
                _name = name;
            }

            public DomainData(string name, params JProperty[] extensionData)
                : base(extensionData)
            {
                _name = name;
            }

            public DomainData(string name, IDictionary<string, JToken> extensionData)
                : base(extensionData)
            {
                _name = name;
            }

            public string Name
            {
                get
                {
                    return _name;
                }
            }
        }
    }
}
