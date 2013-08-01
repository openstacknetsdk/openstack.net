using System;
using System.Runtime.Serialization;

namespace net.openstack.Core.Exceptions
{
    [Serializable]
    public class UserAuthenticationException : Exception
    {
        public UserAuthenticationException(string message) : base(message) {}

        protected UserAuthenticationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
