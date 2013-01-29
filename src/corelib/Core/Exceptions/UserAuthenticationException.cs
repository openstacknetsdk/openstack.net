using System;

namespace net.openstack.Core.Exceptions
{
    public class UserAuthenticationException : Exception
    {
        public UserAuthenticationException(string message) : base(message){}
    }
}