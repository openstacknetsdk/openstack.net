namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class HealthMonitorResponse : ExtensibleJsonObject
    {
        [JsonProperty("health_monitor", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private HealthMonitor _healthMonitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthMonitorResponse"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected HealthMonitorResponse()
        {
        }

        public HealthMonitor HealthMonitor
        {
            get
            {
                return _healthMonitor;
            }
        }
    }
}
