using System;
using System.Runtime.Serialization;

namespace net.openstack.Core.Exceptions
{
    [Serializable]
    public class ObjectNameException : Exception
    {
        public ObjectNameException() { }

        public ObjectNameException(string message) : base(message) { }

        protected ObjectNameException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
