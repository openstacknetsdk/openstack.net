using System;
using net.openstack.Core;
using net.openstack.Providers.Rackspace.Exceptions;

namespace net.openstack.Providers.Rackspace
{
    internal class ComputeProviderFactory : IProviderFactory<IComputeProvider>
    {
        public IComputeProvider Get(string geo)
        {
            switch (geo.ToLower())
            {
                case "dfw":
                    return new RegionalComputeProvider(new Uri(Settings.DFWComputeUrlBase));
                case "ord":
                    return new RegionalComputeProvider(new Uri(Settings.ORDComputeUrlBase));
                case "lon":
                    return new RegionalComputeProvider(new Uri(Settings.LONComputeUrlBase));
                default:
                    throw new UnknownGeographyException(geo);
            }
        }
    }
}
