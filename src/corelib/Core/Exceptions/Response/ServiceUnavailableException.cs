using System;
using System.Runtime.Serialization;

namespace net.openstack.Core.Exceptions.Response
{
    [Serializable]
    public class ServiceUnavailableException : ResponseException
    {
        public ServiceUnavailableException(JSIStudios.SimpleRESTServices.Client.Response response) : base("The service is currently unavailable. Please try again later.", response) { }

        public ServiceUnavailableException(string message, JSIStudios.SimpleRESTServices.Client.Response response) : base(message, response) { }

        protected ServiceUnavailableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
