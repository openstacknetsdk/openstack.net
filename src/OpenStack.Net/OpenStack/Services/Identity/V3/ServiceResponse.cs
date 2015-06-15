namespace OpenStack.Services.Identity.V3
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class ServiceResponse : ExtensibleJsonObject
    {
        [JsonProperty("service", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Service _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected ServiceResponse()
        {
        }

        public Service Service
        {
            get
            {
                return _service;
            }
        }
    }
}
