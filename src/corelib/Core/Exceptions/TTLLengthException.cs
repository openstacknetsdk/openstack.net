using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.openstack.Core.Exceptions
{
    public class TTLLengthException : Exception
    {
        public TTLLengthException() { }
        public TTLLengthException(string message) : base(message) { }
    }
}
