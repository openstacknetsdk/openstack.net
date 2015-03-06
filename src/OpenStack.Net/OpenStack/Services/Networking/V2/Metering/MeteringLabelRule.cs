namespace OpenStack.Services.Networking.V2.Metering
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class MeteringLabelRule : MeteringLabelRuleData
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private MeteringLabelRuleId _id;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteringLabelRule"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected MeteringLabelRule()
        {
        }

        public MeteringLabelRuleId Id
        {
            get
            {
                return _id;
            }
        }
    }
}
