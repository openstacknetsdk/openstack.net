using System;
using System.Runtime.Serialization;

namespace net.openstack.Core.Exceptions.Response
{
    [Serializable]
    public class ServiceConflictException : ResponseException
    {
        public ServiceConflictException(JSIStudios.SimpleRESTServices.Client.Response response) : base("There was a conflict with the service.  Entity may already exist.", response) { }

        public ServiceConflictException(string message, JSIStudios.SimpleRESTServices.Client.Response response) : base(message, response) { }

        protected ServiceConflictException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
