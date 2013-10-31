namespace net.openstack.Providers.Rackspace.Objects.LoadBalancers
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a load balancer in the <see cref="ILoadBalancerService"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    public class LoadBalancer : LoadBalancerConfiguration<Node>
    {
#pragma warning disable 649 // Field 'fieldName' is never assigned to, and will always have its default value
        /// <summary>
        /// This is the backing field for the <see cref="Id"/> property.
        /// </summary>
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private LoadBalancerId _id;

        /// <summary>
        /// This is the backing field for the <see cref="NodeCount"/> property.
        /// </summary>
        [JsonProperty("nodeCount", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private int? _nodeCount;

        /// <summary>
        /// This is the backing field for the <see cref="Cluster"/> property.
        /// </summary>
        [JsonProperty("cluster", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private LoadBalancerCluster _cluster;

        /// <summary>
        /// This is the backing field for the <see cref="Status"/> property.
        /// </summary>
        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private LoadBalancerStatus _status;

        /// <summary>
        /// This is the backing field for the <see cref="Created"/> property.
        /// </summary>
        [JsonProperty("created", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private LoadBalancerTimestamp _created;

        /// <summary>
        /// This is the backing field for the <see cref="Updated"/> property.
        /// </summary>
        [JsonProperty("updated", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private LoadBalancerTimestamp _updated;

        /// <summary>
        /// This is the backing field for the <see cref="SourceAddresses"/> property.
        /// </summary>
        [JsonProperty("sourceAddresses", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private Dictionary<string, string> _sourceAddresses;
#pragma warning restore 649

        /// <summary>
        /// Gets unique ID representing this load balancer within the load balancers service.
        /// </summary>
        /// <value>
        /// The unique ID for the load balancer, or <c>null</c> if the JSON response from the server
        /// did not include this property.
        /// </value>
        public LoadBalancerId Id
        {
            get
            {
                return _id;
            }
        }

        /// <summary>
        /// Gets the total number of nodes in the load balancer.
        /// </summary>
        /// <remarks>
        /// This property is typically populated by calls that do not provide the complete
        /// set of nodes in the <see cref="LoadBalancerConfiguration{TNodeConfiguration}.Nodes"/>
        /// property.
        /// </remarks>
        /// <value>
        /// The total number of nodes in the load balancer, or <c>null</c> if the JSON response
        /// from the server did not include this property.
        /// </value>
        public int? NodeCount
        {
            get
            {
                return _nodeCount;
            }
        }

        /// <summary>
        /// Gets the status of the load balancer.
        /// </summary>
        /// <value>
        /// The status of the load balancer, or <c>null</c> if the JSON response from the server
        /// did not include this property.
        /// </value>
        public LoadBalancerStatus Status
        {
            get
            {
                return _status;
            }
        }

        /// <summary>
        /// Gets the cluster the load balancer is located within.
        /// </summary>
        /// <value>
        /// A <see cref="LoadBalancerCluster"/> object describing the cluster where the load balancer
        /// is located, or <c>null</c> if the JSON response from the server did not include this
        /// property.
        /// </value>
        public LoadBalancerCluster Cluster
        {
            get
            {
                return _cluster;
            }
        }

        /// <summary>
        /// Gets the timestamp when the load balancer was created.
        /// </summary>
        /// <value>
        /// The creation timestamp of the load balancer, or <c>null</c> if the JSON response from the server
        /// did not include this property.
        /// </value>
        public DateTimeOffset? Created
        {
            get
            {
                if (_created == null)
                    return null;

                return _created.Time;
            }
        }

        /// <summary>
        /// Gets the timestamp when the load balancer was last updated.
        /// </summary>
        /// <value>
        /// The last-updated timestamp of the load balancer, or <c>null</c> if the JSON response from the server
        /// did not include this property.
        /// </value>
        public DateTimeOffset? Updated
        {
            get
            {
                if (_updated == null)
                    return null;

                return _updated.Time;
            }
        }

        /// <summary>
        /// Gets the source addresses for the load balancer. The keys of this dictionary
        /// are the names of the source network, and the values are an IP address or
        /// IP address range (in CIDR notation) of the addresses available to the load
        /// balancer on that network.
        /// </summary>
        /// <value>
        /// A dictionary mapping network names to source addresses for the load balancer,
        /// or <c>null</c> if the JSON response from the server did not include this property.
        /// </value>
        public Dictionary<string, string> SourceAddresses
        {
            get
            {
                return _sourceAddresses;
            }
        }
    }
}
