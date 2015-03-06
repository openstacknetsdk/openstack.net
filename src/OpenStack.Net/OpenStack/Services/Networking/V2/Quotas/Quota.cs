namespace OpenStack.Services.Networking.V2.Quotas
{
    using Newtonsoft.Json;
    using OpenStack.Services.Identity;

    [JsonObject(MemberSerialization.OptIn)]
    public class Quota : QuotaData
    {
        [JsonProperty("tenant_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ProjectId _projectId;

        /// <summary>
        /// Initializes a new instance of the <see cref="Quota"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected Quota()
        {
        }

        public ProjectId ProjectId
        {
            get
            {
                return _projectId;
            }
        }
    }
}
