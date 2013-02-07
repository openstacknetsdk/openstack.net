using System;

namespace net.openstack.Core.Exceptions.Response
{
    [Serializable]
    public class UserNotAuthorizedException : ResponseException
    {
        public UserNotAuthorizedException(SimpleRestServices.Client.Response response) : base("Unable to authenticate user and retrieve authorized service endpoints", response){}

        public UserNotAuthorizedException(string message, SimpleRestServices.Client.Response response) : base(message, response){}
    }
}