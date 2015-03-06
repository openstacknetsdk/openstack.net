namespace OpenStack.Services.Networking.V2
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class Subnet : SubnetData
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private SubnetId _subnetId;

        /// <summary>
        /// Initializes a new instance of the <see cref="Subnet"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected Subnet()
        {
        }

        public SubnetId Id
        {
            get
            {
                return _subnetId;
            }
        }
    }
}
