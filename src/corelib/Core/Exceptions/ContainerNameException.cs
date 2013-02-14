using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.openstack.Core.Exceptions
{
    public class ContainerNameException : Exception
    {
        public ContainerNameException() { }
        public ContainerNameException(string message) : base(message) { }
    }
}
