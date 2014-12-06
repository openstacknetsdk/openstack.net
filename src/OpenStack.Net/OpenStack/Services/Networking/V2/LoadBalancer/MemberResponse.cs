namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class MemberResponse : ExtensibleJsonObject
    {
        [JsonProperty("member", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Member _member;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected MemberResponse()
        {
        }

        public Member Member
        {
            get
            {
                return _member;
            }
        }
    }
}
