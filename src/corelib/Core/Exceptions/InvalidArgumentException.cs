using System;
using System.Runtime.Serialization;

namespace net.openstack.Core.Exceptions
{
    public class InvalidArgumentException : Exception
    {
        public InvalidArgumentException() 
        { }

        public InvalidArgumentException(string message) 
            : base(message) 
        { }

        protected InvalidArgumentException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
