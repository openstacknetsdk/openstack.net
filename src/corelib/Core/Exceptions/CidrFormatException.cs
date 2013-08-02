using System;
using System.Runtime.Serialization;

namespace net.openstack.Core.Exceptions
{
    [Serializable]
    public class CidrFormatException : Exception
    {
        public CidrFormatException() { }

        public CidrFormatException(string message) : base(message) { }

        protected CidrFormatException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
