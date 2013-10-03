namespace net.openstack.Core.Domain.Rackspace
{
    public class LoadBalancerStats
    {

        //connectTimeOut – Connections closed by this load balancer because the 'connect_timeout' interval was exceeded.
        public int ConnectionTimeOutCount { get; set; }

        //connectError – Number of transaction or protocol errors in this load balancer.
        public int ErrorCount { get; set; }

        //connectFailure – Number of connection failures in this load balancer.
        public int FailureCount { get; set; }

        //dataTimedOut – Connections closed by this load balancer because the 'timeout' interval was exceeded.
        public int TimeOutCount { get; set; }

        //keepAliveTimedOut – Connections closed by this load balancer because the 'keepalive_timeout' interval was exceeded.
        public int KeepAliveTimeOutCount { get; set; }

        //maxConn
        public int MaximumConnections { get; set; }
    }
}
