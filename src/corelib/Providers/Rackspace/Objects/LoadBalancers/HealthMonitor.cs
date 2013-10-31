namespace net.openstack.Providers.Rackspace.Objects.LoadBalancers
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// This is the base class for load balancer health monitoring configurations.
    /// </summary>
    /// <see cref="ILoadBalancerService.GetHealthMonitorAsync"/>
    /// <see cref="ILoadBalancerService.SetHealthMonitorAsync"/>
    /// <see cref="ILoadBalancerService.RemoveHealthMonitorAsync"/>
    /// <seealso href="http://docs.rackspace.com/loadbalancers/api/v1.0/clb-devguide/content/Monitor_Health-d1e3434.html">Monitor Health (Rackspace Cloud Load Balancers Developer Guide - API v1.0)</seealso>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    [JsonConverter(typeof(HealthMonitor.Converter))]
    public abstract class HealthMonitor
    {
        /// <summary>
        /// This is the backing field for the <see cref="Type"/> property.
        /// </summary>
        [JsonProperty("type")]
        private HealthMonitorType _type;

        /// <summary>
        /// This is the backing field for the <see cref="Timeout"/> property.
        /// </summary>
        [JsonProperty("timeout")]
        private int? _timeout;

        /// <summary>
        /// This is the backing field for the <see cref="Delay"/> property.
        /// </summary>
        [JsonProperty("delay")]
        private int? _delay;

        /// <summary>
        /// This is the backing field for the <see cref="AttemptsBeforeDeactivation"/> property.
        /// </summary>
        [JsonProperty("attemptsBeforeDeactivation")]
        private int? _attemptsBeforeDeactivation;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthMonitor"/> class
        /// during JSON deserialization.
        /// </summary>
        protected HealthMonitor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthMonitor"/> class
        /// using the specified values.
        /// </summary>
        /// <param name="type">The type of health monitor.</param>
        /// <param name="attemptsBeforeDeactivation">The number of permissible monitor failures before removing a node from rotation.</param>
        /// <param name="timeout">The maximum number of seconds to wait for a connection to be established before timing out.</param>
        /// <param name="delay">The minimum time to wait before executing the health monitor.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="attemptsBeforeDeactivation"/> is less than or equal to 0.
        /// <para>-or-</para>
        /// <para>If <paramref name="timeout"/> is negative or <see cref="TimeSpan.Zero"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="delay"/> is negative or <see cref="TimeSpan.Zero"/>.</para>
        /// </exception>
        protected HealthMonitor(HealthMonitorType type, int attemptsBeforeDeactivation, TimeSpan timeout, TimeSpan delay)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (attemptsBeforeDeactivation <= 0)
                throw new ArgumentOutOfRangeException("attemptsBeforeDeactivation");
            if (timeout <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("timeout");
            if (delay <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("delay");

            _type = type;
            _attemptsBeforeDeactivation = attemptsBeforeDeactivation;
            _timeout = (int)timeout.TotalSeconds;
            _delay = (int)delay.TotalSeconds;
        }

        /// <summary>
        /// Gets the type of health monitor.
        /// </summary>
        public HealthMonitorType Type
        {
            get
            {
                return _type;
            }
        }

        /// <summary>
        /// Gets the maximum number of seconds to wait for a connection to be established before timing out.
        /// </summary>
        public TimeSpan? Timeout
        {
            get
            {
                if (_timeout == null)
                    return null;

                return TimeSpan.FromSeconds(_timeout.Value);
            }
        }

        /// <summary>
        /// Gets the minimum time to wait before executing the health monitor.
        /// </summary>
        public TimeSpan? Delay
        {
            get
            {
                if (_delay == null)
                    return null;

                return TimeSpan.FromSeconds(_delay.Value);
            }
        }

        /// <summary>
        /// Gets the number of permissible monitor failures before removing a node from rotation.
        /// </summary>
        public int? AttemptsBeforeDeactivation
        {
            get
            {
                return _attemptsBeforeDeactivation;
            }
        }

        /// <inheritdoc/>
        protected class Converter : JsonConverter
        {
            /// <inheritdoc/>
            public override bool CanConvert(Type objectType)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}
