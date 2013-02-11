namespace net.openstack.Core.Exceptions.Response
{
    public class ServiceLimitReachedException : ResponseException
    {
        public ServiceLimitReachedException(JSIStudios.SimpleRESTServices.Client.Response response) : base("The service rate limit has been reached.  Either request a service limit increase or wait until the limit resets.", response) { }

        public ServiceLimitReachedException(string message, JSIStudios.SimpleRESTServices.Client.Response response) : base(message, response) { }
    }
}