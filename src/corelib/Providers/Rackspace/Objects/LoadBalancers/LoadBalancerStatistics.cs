namespace net.openstack.Providers.Rackspace.Objects.LoadBalancers
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This class represents the load balancer statistics returned from a
    /// call to <see cref="ILoadBalancerService.GetStatisticsAsync"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class LoadBalancerStatistics : Dictionary<string, long>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadBalancerStatistics"/> class.
        /// </summary>
        public LoadBalancerStatistics()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        /// <summary>
        /// Gets the number of connections closed by the load balancer because the <em>connect_timeout</em> interval was exceeded.
        /// </summary>
        public long? ConnectionTimedOut
        {
            get
            {
                long value;
                if (!TryGetValue("connectTimeOut", out value))
                    return null;

                return value;
            }
        }

        /// <summary>
        /// Gets the number of transaction or protocol errors in the load balancer.
        /// </summary>
        public long? ConnectionError
        {
            get
            {
                long value;
                if (!TryGetValue("connectError", out value))
                    return null;

                return value;
            }
        }

        /// <summary>
        /// Gets the number of connection failures in the load balancer.
        /// </summary>
        public long? ConnectionFailure
        {
            get
            {
                long value;
                if (!TryGetValue("connectFailure", out value))
                    return null;

                return value;
            }
        }

        /// <summary>
        /// Gets the number of connections closed by this load balancer because the <em>timeout</em> interval was exceeded.
        /// </summary>
        public long? DataTimedOut
        {
            get
            {
                long value;
                if (!TryGetValue("dataTimedOut", out value))
                    return null;

                return value;
            }
        }

        /// <summary>
        /// Gets the number of connections closed by this load balancer because the <em>keepalive_timeout</em> interval was exceeded.
        /// </summary>
        public long? KeepAliveTimedOut
        {
            get
            {
                long value;
                if (!TryGetValue("keepAliveTimedOut", out value))
                    return null;

                return value;
            }
        }

        /// <summary>
        /// Gets the maximum number of simultaneous TCP connections this load balancer has processed at any one time.
        /// </summary>
        public long? MaxConnections
        {
            get
            {
                long value;
                if (!TryGetValue("maxConn", out value))
                    return null;

                return value;
            }
        }
    }
}
