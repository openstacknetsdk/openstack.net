namespace OpenStack.Services.Networking.V2.Quotas
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class QuotaResponse : ExtensibleJsonObject
    {
        [JsonProperty("quota", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Quota _quota;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuotaResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected QuotaResponse()
        {
        }

        public Quota Quota
        {
            get
            {
                return _quota;
            }
        }
    }
}
