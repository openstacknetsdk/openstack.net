namespace net.openstack.Core.Exceptions.Response
{
    public class BadServiceRequestException : ResponseException
    {
        public BadServiceRequestException(JSIStudios.SimpleRESTServices.Client.Response response) : base("Unable to process the service request.  Please try again later.", response) { }

        public BadServiceRequestException(string message, JSIStudios.SimpleRESTServices.Client.Response response) : base(message, response) { }
    }
}