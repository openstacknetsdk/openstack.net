using System;
using System.Runtime.Serialization;

namespace net.openstack.Core.Exceptions.Response
{
    [Serializable]
    public class UserNotAuthorizedException : ResponseException
    {
        public UserNotAuthorizedException(JSIStudios.SimpleRESTServices.Client.Response response) : base("Unable to authenticate user and retrieve authorized service endpoints", response) { }

        public UserNotAuthorizedException(string message, JSIStudios.SimpleRESTServices.Client.Response response) : base(message, response) { }

        protected UserNotAuthorizedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
