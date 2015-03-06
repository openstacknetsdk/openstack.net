namespace OpenStack.Services.Networking.V2.LoadBalancer
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class HealthMonitor : HealthMonitorData
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private HealthMonitorId _id;

        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private HealthMonitorStatus _status;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthMonitor"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected HealthMonitor()
        {
        }

        public HealthMonitorId Id
        {
            get
            {
                return _id;
            }
        }

        public HealthMonitorStatus Status
        {
            get
            {
                return _status;
            }
        }
    }
}
