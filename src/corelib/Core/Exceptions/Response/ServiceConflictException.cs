using System;
using System.Runtime.Serialization;
using RestResponse = JSIStudios.SimpleRESTServices.Client.Response;

namespace net.openstack.Core.Exceptions.Response
{
    [Serializable]
    public class ServiceConflictException : ResponseException
    {
        public ServiceConflictException(RestResponse response) : base("There was a conflict with the service.  Entity may already exist.", response) { }

        public ServiceConflictException(string message, RestResponse response) : base(message, response) { }

        protected ServiceConflictException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
