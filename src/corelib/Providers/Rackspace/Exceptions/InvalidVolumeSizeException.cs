using System;
using System.Runtime.Serialization;

namespace net.openstack.Providers.Rackspace.Exceptions
{
    [Serializable]
    public class InvalidVolumeSizeException : Exception
    {
        public int Size { get; private set; }

        public InvalidVolumeSizeException(int size)
            : base(string.Format("The volume size value must be between 100 and 1000.  The size requested was: {0}", size))
        {
            Size = size;
        }

        protected InvalidVolumeSizeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
