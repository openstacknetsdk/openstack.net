using System;
using System.Runtime.Serialization;
using RestResponse = JSIStudios.SimpleRESTServices.Client.Response;

namespace net.openstack.Core.Exceptions.Response
{
    [Serializable]
    class MethodNotImplementedException : ResponseException
    {
        public MethodNotImplementedException(RestResponse response) : base("The requested method is not implemented at the service.", response) { }

        public MethodNotImplementedException(string message, RestResponse response) : base(message, response) { }

        protected MethodNotImplementedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
