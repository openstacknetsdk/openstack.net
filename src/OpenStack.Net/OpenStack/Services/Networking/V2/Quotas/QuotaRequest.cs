namespace OpenStack.Services.Networking.V2.Quotas
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class QuotaRequest : ExtensibleJsonObject
    {
        [JsonProperty("quota", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private QuotaData _quota;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuotaRequest"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected QuotaRequest()
        {
        }

        public QuotaRequest(QuotaData quota)
        {
            _quota = quota;
        }

        public QuotaData Quota
        {
            get
            {
                return _quota;
            }
        }
    }
}
