namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using System.Collections.Immutable;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class Pool : PoolData
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private PoolId _id;

        [JsonProperty("vip_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private VirtualAddressId _virtualAddressId;

        [JsonProperty("members", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<MemberId> _members;

        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private PoolStatus _status;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pool"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected Pool()
        {
        }

        public PoolId Id
        {
            get
            {
                return _id;
            }
        }

        public VirtualAddressId VirtualAddressId
        {
            get
            {
                return _virtualAddressId;
            }
        }

        public ImmutableArray<MemberId> Members
        {
            get
            {
                return _members;
            }
        }

        public PoolStatus Status
        {
            get
            {
                return _status;
            }
        }
    }
}
