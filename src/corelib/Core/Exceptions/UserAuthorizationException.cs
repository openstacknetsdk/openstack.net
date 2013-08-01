using System;
using System.Runtime.Serialization;

namespace net.openstack.Core.Exceptions
{
    [Serializable]
    public class UserAuthorizationException : Exception
    {
        public UserAuthorizationException(string message) : base(message){}

        protected UserAuthorizationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
