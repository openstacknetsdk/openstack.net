namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class Member : MemberData
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private MemberId _id;

        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private MemberStatus _status;

        /// <summary>
        /// Initializes a new instance of the <see cref="Member"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected Member()
        {
        }

        public MemberId Id
        {
            get
            {
                return _id;
            }
        }

        public MemberStatus Status
        {
            get
            {
                return _status;
            }
        }
    }
}
