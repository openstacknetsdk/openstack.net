using System;
using System.Runtime.Serialization;

namespace net.openstack.Core.Exceptions.Response
{
    [Serializable]
    public class BadServiceRequestException : ResponseException
    {
        public BadServiceRequestException(JSIStudios.SimpleRESTServices.Client.Response response) : base("Unable to process the service request.  Please try again later.", response) { }

        public BadServiceRequestException(string message, JSIStudios.SimpleRESTServices.Client.Response response) : base(message, response) { }

        protected BadServiceRequestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
