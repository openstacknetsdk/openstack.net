using System;
using net.openstack.Core.Domain;
using net.openstack.Providers.Rackspace;

namespace net.openstack.console
{
    class Program
    {
        static void Main(string[] args)
        {
            var provider = new Providers.Rackspace.ComputeProvider();
            var results = provider.ListImagesWithDetails(new RackspaceCloudIdentity {Username = "Cloud2", Password = "Hybr1d99"});

            Console.WriteLine(results);
            Console.ReadLine();
        }
    }
}
