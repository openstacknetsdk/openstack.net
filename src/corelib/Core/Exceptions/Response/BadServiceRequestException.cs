using System;
using System.Runtime.Serialization;
using RestResponse = JSIStudios.SimpleRESTServices.Client.Response;

namespace net.openstack.Core.Exceptions.Response
{
    [Serializable]
    public class BadServiceRequestException : ResponseException
    {
        public BadServiceRequestException(RestResponse response) : base("Unable to process the service request.  Please try again later.", response) { }

        public BadServiceRequestException(string message, RestResponse response) : base(message, response) { }

        protected BadServiceRequestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
