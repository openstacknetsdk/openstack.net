using System;
using System.Runtime.Serialization;
using RestResponse = JSIStudios.SimpleRESTServices.Client.Response;

namespace net.openstack.Core.Exceptions.Response
{
    [Serializable]
    public class ServiceUnavailableException : ResponseException
    {
        public ServiceUnavailableException(RestResponse response) : base("The service is currently unavailable. Please try again later.", response) { }

        public ServiceUnavailableException(string message, RestResponse response) : base(message, response) { }

        protected ServiceUnavailableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
