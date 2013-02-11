namespace net.openstack.Core.Exceptions.Response
{
    public class ServiceFaultException : ResponseException
    {
        public ServiceFaultException(JSIStudios.SimpleRESTServices.Client.Response response) : base("There was an unhandled error at the service endpoint.  Please try again later.", response) { }

        public ServiceFaultException(string message, JSIStudios.SimpleRESTServices.Client.Response response) : base(message, response) { }
    }
}