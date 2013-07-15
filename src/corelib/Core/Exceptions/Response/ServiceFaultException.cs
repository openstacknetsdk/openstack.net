using System;
using System.Runtime.Serialization;

namespace net.openstack.Core.Exceptions.Response
{
    [Serializable]
    public class ServiceFaultException : ResponseException
    {
        public ServiceFaultException(JSIStudios.SimpleRESTServices.Client.Response response) : base("There was an unhandled error at the service endpoint.  Please try again later.", response) { }

        public ServiceFaultException(string message, JSIStudios.SimpleRESTServices.Client.Response response) : base(message, response) { }

        protected ServiceFaultException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
