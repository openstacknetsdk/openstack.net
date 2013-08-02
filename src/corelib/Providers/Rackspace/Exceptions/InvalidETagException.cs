using System;
using System.Runtime.Serialization;

namespace net.openstack.Providers.Rackspace.Exceptions
{
    [Serializable]
    public class InvalidETagException : Exception
    {
        public InvalidETagException()
        {
        }

        protected InvalidETagException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
