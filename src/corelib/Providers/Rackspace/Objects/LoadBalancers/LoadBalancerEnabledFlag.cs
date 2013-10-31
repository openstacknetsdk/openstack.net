namespace net.openstack.Providers.Rackspace.Objects.LoadBalancers
{
    using Newtonsoft.Json;

    /// <summary>
    /// This class models the JSON object used to represent an enabled/disabled
    /// flag for a configuration option.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    public class LoadBalancerEnabledFlag
    {
        /// <summary>
        /// This is the backing field for the <see cref="True"/> property.
        /// </summary>
        private static readonly LoadBalancerEnabledFlag _true = new LoadBalancerEnabledFlag(true);

        /// <summary>
        /// This is the backing field for the <see cref="False"/> property.
        /// </summary>
        private static readonly LoadBalancerEnabledFlag _false = new LoadBalancerEnabledFlag(false);

        /// <summary>
        /// This is the backing field for the <see cref="Enabled"/> property.
        /// </summary>
        [JsonProperty("enabled")]
        private bool _enabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadBalancerEnabledFlag"/>
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected LoadBalancerEnabledFlag()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadBalancerEnabledFlag"/>
        /// with the specified value.
        /// </summary>
        /// <param name="enabled"><c>true</c> if the option is enabled; otherwise, <c>false</c>.</param>
        public LoadBalancerEnabledFlag(bool enabled)
        {
            _enabled = enabled;
        }

        /// <summary>
        /// Gets a <see cref="LoadBalancerEnabledFlag"/> instance representing an enabled option.
        /// </summary>
        public static LoadBalancerEnabledFlag True
        {
            get
            {
                return _true;
            }
        }

        /// <summary>
        /// Gets a <see cref="LoadBalancerEnabledFlag"/> instance representing a disabled option.
        /// </summary>
        public static LoadBalancerEnabledFlag False
        {
            get
            {
                return _false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the option is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if the option is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled
        {
            get
            {
                return _enabled;
            }
        }
    }
}
