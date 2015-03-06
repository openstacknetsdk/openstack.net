namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class PoolRequest : ExtensibleJsonObject
    {
        [JsonProperty("pool", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private PoolData _pool;

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolRequest"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected PoolRequest()
        {
        }

        public PoolRequest(PoolData pool)
        {
            _pool = pool;
        }

        public PoolData Pool
        {
            get
            {
                return _pool;
            }
        }
    }
}
