namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    [JsonObject(MemberSerialization.OptIn)]
    public class HealthMonitorRequest : ExtensibleJsonObject
    {
        [JsonProperty("health_monitor", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private HealthMonitorData _healthMonitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthMonitorRequest"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected HealthMonitorRequest()
        {
        }

        public HealthMonitorRequest(HealthMonitorData healthMonitor)
        {
            _healthMonitor = healthMonitor;
        }

        public HealthMonitorData HealthMonitor
        {
            get
            {
                return _healthMonitor;
            }
        }
    }
}
