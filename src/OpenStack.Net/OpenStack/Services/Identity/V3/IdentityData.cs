namespace OpenStack.Services.Identity.V3
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class IdentityData : ExtensibleJsonObject
    {
        [JsonProperty("methods", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private AuthenticationMethod[] _methods;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityData"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected IdentityData()
        {
        }

        public IdentityData(IEnumerable<AuthenticationMethod> methods)
        {
            if (methods != null)
                _methods = methods.ToArray();
        }

        public IdentityData(IEnumerable<AuthenticationMethod> methods, params JProperty[] extensionData)
            : base(extensionData)
        {
            if (methods != null)
                _methods = methods.ToArray();
        }

        public IdentityData(IEnumerable<AuthenticationMethod> methods, IDictionary<string, JToken> extensionData)
            : base(extensionData)
        {
            if (methods != null)
                _methods = methods.ToArray();
        }

        public ReadOnlyCollection<AuthenticationMethod> Methods
        {
            get
            {
                if (_methods == null)
                    return null;

                return new ReadOnlyCollection<AuthenticationMethod>(_methods);
            }
        }
    }
}
