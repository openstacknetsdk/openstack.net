using System;
using System.Runtime.Serialization;

namespace net.openstack.Core.Exceptions
{
    [Serializable]
    public class ContainerNameException : Exception
    {
        public ContainerNameException() { }

        public ContainerNameException(string message) : base(message) { }

        protected ContainerNameException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
