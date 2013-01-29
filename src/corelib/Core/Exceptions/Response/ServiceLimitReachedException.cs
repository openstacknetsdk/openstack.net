namespace net.openstack.Core.Exceptions.Response
{
    public class ServiceLimitReachedException : ResponseException
    {
        public ServiceLimitReachedException(SimpleRestServices.Client.Response response) : base("The service rate limit has been reached.  Either request a service limit increase or wait until the limit resets.", response) {}

        public ServiceLimitReachedException(string message, SimpleRestServices.Client.Response response) : base(message, response) {}
    }
}