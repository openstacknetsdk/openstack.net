using System;

namespace net.openstack.corelib.Providers.Rackspace
{
    public class IdentityProviderFactory : IProviderFactory<IIdentityProvider>
    {
        public IIdentityProvider Get(string geo)
        {
            switch (geo.ToLower())
            {
                case "dfw":
                case "ord":
                case "us":
                    return new GeographicalIdentityProvider(new Uri(Settings.USIdentityUrlBase));
                case "lon":
                    return new GeographicalIdentityProvider(new Uri(Settings.LONIdentityUrlBase));
                default:
                    throw new UnknownGeographyException(geo);
            }
        }
    }

    public class UnknownGeographyException : Exception
    {
        public string Geo { get; private set; }

        public UnknownGeographyException(string geo) : base(string.Format("Unknown Geography: {0}", geo))
        {
            Geo = geo;
        }
    }

    public interface IProviderFactory<T>
    {
        T Get(string geo);
    }
}
