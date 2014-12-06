namespace OpenStack.Services.Networking.V2.Metering
{
    using Newtonsoft.Json;
    using OpenStack.Services.Identity;

    [JsonObject(MemberSerialization.OptIn)]
    public class MeteringLabel : MeteringLabelData
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private MeteringLabelId _id;

        [JsonProperty("tenant_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ProjectId _projectId;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteringLabel"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected MeteringLabel()
        {
        }

        public MeteringLabelId Id
        {
            get
            {
                return _id;
            }
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
