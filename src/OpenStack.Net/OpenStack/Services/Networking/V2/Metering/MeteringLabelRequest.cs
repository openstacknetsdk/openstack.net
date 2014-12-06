namespace OpenStack.Services.Networking.V2.Metering
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class MeteringLabelRequest : ExtensibleJsonObject
    {
        [JsonProperty("metering_label", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private MeteringLabelData _meteringLabel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteringLabelRequest"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected MeteringLabelRequest()
        {
        }

        public MeteringLabelRequest(MeteringLabelData meteringLabel)
        {
            _meteringLabel = meteringLabel;
        }

        public MeteringLabelData MeteringLabel
        {
            get
            {
                return _meteringLabel;
            }
        }
    }
}
