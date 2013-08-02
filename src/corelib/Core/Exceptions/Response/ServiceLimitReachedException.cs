using System;
using System.Runtime.Serialization;
using RestResponse = JSIStudios.SimpleRESTServices.Client.Response;

namespace net.openstack.Core.Exceptions.Response
{
    [Serializable]
    public class ServiceLimitReachedException : ResponseException
    {
        public ServiceLimitReachedException(RestResponse response) : base("The service rate limit has been reached.  Either request a service limit increase or wait until the limit resets.", response) { }

        public ServiceLimitReachedException(string message, RestResponse response) : base(message, response) { }

        protected ServiceLimitReachedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
