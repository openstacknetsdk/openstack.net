namespace OpenStack.Services.Networking.V2.Metering
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class MeteringLabelResponse : ExtensibleJsonObject
    {
        [JsonProperty("metering_label", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private MeteringLabel _meteringLabel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteringLabelResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected MeteringLabelResponse()
        {
        }

        public MeteringLabel MeteringLabel
        {
            get
            {
                return _meteringLabel;
            }
        }
    }
}
