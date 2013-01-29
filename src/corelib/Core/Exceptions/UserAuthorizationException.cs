using System;

namespace net.openstack.Core.Exceptions
{
    public class UserAuthorizationException : Exception
    {
        public UserAuthorizationException(string message) : base(message){}
    }
}