using System;
using System.Runtime.Serialization;

namespace net.openstack.Core.Exceptions.Response
{
    [Serializable]
    class MethodNotImplementedException : ResponseException
    {
        public MethodNotImplementedException(JSIStudios.SimpleRESTServices.Client.Response response) : base("The requested method is not implemented at the service.", response) { }

        public MethodNotImplementedException(string message, JSIStudios.SimpleRESTServices.Client.Response response) : base(message, response) { }

        protected MethodNotImplementedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
