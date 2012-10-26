using System;

namespace net.openstack.Providers.Rackspace.Exceptions
{
    public class UnknownGeographyException : Exception
    {
        public string Geo { get; private set; }

        public UnknownGeographyException(string geo) : base(string.Format("Unknown Geography: {0}", geo))
        {
            Geo = geo;
        }
    }
}