using System;
using System.Runtime.Serialization;
using RestResponse = JSIStudios.SimpleRESTServices.Client.Response;

namespace net.openstack.Core.Exceptions.Response
{
    [Serializable]
    public class ServiceFaultException : ResponseException
    {
        public ServiceFaultException(RestResponse response) : base("There was an unhandled error at the service endpoint.  Please try again later.", response) { }

        public ServiceFaultException(string message, RestResponse response) : base(message, response) { }

        protected ServiceFaultException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
