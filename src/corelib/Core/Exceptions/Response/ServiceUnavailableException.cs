namespace net.openstack.Core.Exceptions.Response
{
    public class ServiceUnavailableException : ResponseException
    {
        public ServiceUnavailableException(SimpleRestServices.Client.Response response) : base("The service is currently unavailable. Please try again later.", response) {}

        public ServiceUnavailableException(string message, SimpleRestServices.Client.Response response) : base(message, response) {}
    }
}