using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.openstack.Providers.Rackspace.Exceptions
{
    public class InvalidVolumeSizeException : Exception
    {
        public int Size { get; private set; }

        public InvalidVolumeSizeException(int size)
            : base(string.Format("The volume size value must be between 100 and 1000.  The size requested was: {0}", size))
        {
            Size = size;
        }
    }
}
