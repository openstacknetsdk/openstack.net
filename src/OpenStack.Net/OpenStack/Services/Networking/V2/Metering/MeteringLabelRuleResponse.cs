namespace OpenStack.Services.Networking.V2.Metering
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class MeteringLabelRuleResponse : ExtensibleJsonObject
    {
        [JsonProperty("metering_label_rule", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private MeteringLabelRule _meteringLabelRule;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteringLabelRuleResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected MeteringLabelRuleResponse()
        {
        }

        public MeteringLabelRule MeteringLabelRule
        {
            get
            {
                return _meteringLabelRule;
            }
        }
    }
}
