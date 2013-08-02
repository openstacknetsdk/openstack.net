using System;
using System.Runtime.Serialization;

namespace net.openstack.Core.Exceptions
{
    [Serializable]
    public class NoDefaultRegionSetException : Exception
    {
        public NoDefaultRegionSetException(string message) : base(message)
        {
        }

        protected NoDefaultRegionSetException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
