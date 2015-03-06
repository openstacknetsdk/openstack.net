namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class PoolResponse : ExtensibleJsonObject
    {
        [JsonProperty("pool", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Pool _pool;

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected PoolResponse()
        {
        }

        public Pool Pool
        {
            get
            {
                return _pool;
            }
        }
    }
}
