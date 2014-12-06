namespace OpenStack.Services.Networking.V2
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class SubnetResponse : ExtensibleJsonObject
    {
        [JsonProperty("subnet", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Subnet _subnet;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubnetResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected SubnetResponse()
        {
        }

        public Subnet Subnet
        {
            get
            {
                return _subnet;
            }
        }
    }
}
