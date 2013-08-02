using System;
using System.Runtime.Serialization;

namespace net.openstack.Core.Exceptions
{
    [Serializable]
    public class TTLLengthException : Exception
    {
        public TTLLengthException() { }

        public TTLLengthException(string message) : base(message) { }

        protected TTLLengthException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
