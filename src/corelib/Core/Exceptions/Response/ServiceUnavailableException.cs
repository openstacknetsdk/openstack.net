namespace net.openstack.Core.Exceptions.Response
{
    public class ServiceUnavailableException : ResponseException
    {
        public ServiceUnavailableException(JSIStudios.SimpleRESTServices.Client.Response response) : base("The service is currently unavailable. Please try again later.", response) { }

        public ServiceUnavailableException(string message, JSIStudios.SimpleRESTServices.Client.Response response) : base(message, response) { }
    }
}