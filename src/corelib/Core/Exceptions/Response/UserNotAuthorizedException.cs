using System;
using System.Runtime.Serialization;
using RestResponse = JSIStudios.SimpleRESTServices.Client.Response;

namespace net.openstack.Core.Exceptions.Response
{
    [Serializable]
    public class UserNotAuthorizedException : ResponseException
    {
        public UserNotAuthorizedException(RestResponse response) : base("Unable to authenticate user and retrieve authorized service endpoints", response) { }

        public UserNotAuthorizedException(string message, RestResponse response) : base(message, response) { }

        protected UserNotAuthorizedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
