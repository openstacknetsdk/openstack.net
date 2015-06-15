namespace OpenStack.Services.Identity.V3
{
    using System.Collections.Immutable;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class IdentityData : ExtensibleJsonObject
    {
        [JsonProperty("methods", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<AuthenticationMethod> _methods;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityData"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected IdentityData()
        {
        }

        public IdentityData(ImmutableArray<AuthenticationMethod> methods)
        {
            _methods = methods;
        }

        public ImmutableArray<AuthenticationMethod> Methods
        {
            get
            {
                return _methods;
            }
        }
    }
}
