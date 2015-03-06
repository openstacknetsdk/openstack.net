namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class MemberRequest : ExtensibleJsonObject
    {
        [JsonProperty("member", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private MemberData _member;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberRequest"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected MemberRequest()
        {
        }

        public MemberRequest(MemberData member)
        {
            _member = member;
        }

        public MemberData Member
        {
            get
            {
                return _member;
            }
        }
    }
}
