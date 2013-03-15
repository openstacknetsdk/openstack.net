using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.openstack.Core.Exceptions
{
    public class ObjectNameException : Exception
    {
        public ObjectNameException() { }
        public ObjectNameException(string message) : base(message) { }
    }
}
