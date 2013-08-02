using System;
using System.Runtime.Serialization;

namespace net.openstack.Core.Exceptions
{
    [Serializable]
    internal class InvalidCloudIdentityException : Exception
    {
        public InvalidCloudIdentityException(string message) : base(message) {}

        protected InvalidCloudIdentityException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
