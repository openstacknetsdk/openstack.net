namespace net.openstack.Providers.Rackspace.Objects.LoadBalancers
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// This class represents the configuration for a new load balancer.
    /// </summary>
    /// <seealso cref="ILoadBalancerService.CreateLoadBalancerAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    public class LoadBalancerConfiguration : LoadBalancerConfiguration<NodeConfiguration>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadBalancerConfiguration"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor is used during JSON deserialization of derived types.
        /// </remarks>
        [JsonConstructor]
        protected LoadBalancerConfiguration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadBalancerConfiguration"/> class
        /// with the specified values.
        /// </summary>
        /// <param name="name">The name of the load balancer.</param>
        /// <param name="protocol">The load balancing protocol to use for this load balancer.</param>
        /// <param name="virtualAddresses">A collection of <see cref="LoadBalancerVirtualAddress"/> objects describing the virtual addresses to assign to the load balancer.</param>
        /// <param name="nodes">A collection of <see name="NodeConfiguration"/> objects describing the nodes in the load balancer. If this value is <c>null</c>, the load balancer will be created without any nodes.</param>
        /// <param name="halfClosed"><c>true</c> to enable half-closed support for the load balancer; otherwise, <c>false</c>. Half-Closed support provides the ability for one end of the connection to terminate its output, while still receiving data from the other end. Only applies to TCP/TCP_CLIENT_FIRST protocols.</param>
        /// <param name="accessList">A collection of <see cref="NetworkItem"/> objects describing the access list for the load balancer. If this value is <c>null</c>, the load balancer will be created without an access list configured.</param>
        /// <param name="algorithm">The load balancing algorithm that defines how traffic should be directed between back-end nodes. If this value is <c>null</c>, a provider-specific default algorithm will be used.</param>
        /// <param name="connectionLogging"><c>true</c> to enable connection logging; otherwise, <c>false</c>. If this value is <c>null</c>, a provider-specific default value will be used.</param>
        /// <param name="contentCaching"><c>true</c> to enable content caching; otherwise, <c>false</c>. If this value is <c>null</c>, a provider-specific default value will be used.</param>
        /// <param name="connectionThrottle">A <see cref="ConnectionThrottles"/> object defining the connection throttling configuration for the load balancer. If this value is <c>null</c>, a provider-specific default value will be used.</param>
        /// <param name="healthMonitor">A <see cref="HealthMonitor"/> object defining the health monitor to configure for the load balancer. If this value is <c>null</c>, the load balancer will be created with no health monitor configured.</param>
        /// <param name="metadata">A collection of <see cref="LoadBalancerMetadataItem"/> objects defining the initial metadata for the load balancer. If this value is <c>null</c>, the load balancer will be created without any initial custom metadata.</param>
        /// <param name="timeout">The timeout value for the load balancer and communications with its nodes. If this value is <c>null</c>, a provider-specific default value will be used.</param>
        /// <param name="sessionPersistence">A <see cref="SessionPersistence"/> object defining the session persistence configuration for the load balancer. If this value is <c>null</c>, the load balancer will be created with session persistence disabled.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="protocol"/> is <c>null</c>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="virtualAddresses"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="name"/> is empty.
        /// <para>-or-</para>
        /// <para>If <paramref name="nodes"/> contains any <c>null</c> values.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="virtualAddresses"/> is empty.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="virtualAddresses"/> contains any <c>null</c> values.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="accessList"/> contains any <c>null</c> values.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="metadata"/> contains any <c>null</c> values.</para>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="timeout"/> is negative or <see cref="TimeSpan.Zero"/>.
        /// </exception>
        public LoadBalancerConfiguration(string name, LoadBalancingProtocol protocol, IEnumerable<LoadBalancerVirtualAddress> virtualAddresses, IEnumerable<NodeConfiguration> nodes = null, bool? halfClosed = null, IEnumerable<NetworkItem> accessList = null, LoadBalancingAlgorithm algorithm = null, bool? connectionLogging = null, bool? contentCaching = null, ConnectionThrottles connectionThrottle = null, HealthMonitor healthMonitor = null, IEnumerable<LoadBalancerMetadataItem> metadata = null, TimeSpan? timeout = null, SessionPersistence sessionPersistence = null)
            : base(name, protocol, virtualAddresses, nodes, halfClosed, accessList, algorithm, connectionLogging, contentCaching, connectionThrottle, healthMonitor, metadata, timeout, sessionPersistence)
        {
        }
    }
}
