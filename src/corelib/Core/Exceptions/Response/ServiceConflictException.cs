namespace net.openstack.Core.Exceptions.Response
{
    public class ServiceConflictException : ResponseException
    {
        public ServiceConflictException(JSIStudios.SimpleRESTServices.Client.Response response) : base("There was a conflict with the service.  Entity may already exist.", response) { }

        public ServiceConflictException(string message, JSIStudios.SimpleRESTServices.Client.Response response) : base(message, response) { }
    }
}