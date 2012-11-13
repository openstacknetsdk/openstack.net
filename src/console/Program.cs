using System;
using System.Threading;
using net.openstack.Core.Domain;

namespace net.openstack.console
{
    class Program
    {
        static void Main(string[] args)
        {
            var identity = new CloudIdentity()
                               {
                                   APIKey = "",
                                   Password = "",
                                   Region = "",
                                   Username = ""
                               };

            Console.WriteLine("Building new server" + Environment.NewLine);
            var provider = new Providers.Rackspace.ComputeProvider();
            var server = provider.CreateServer("test-server", "My Test Server", "d531a2dd-7ae9-4407-bb5a-e5ea03303d98", "2", identity);

            if (server == null)
            {
                Console.WriteLine("No server returned. Check log for details");
            }
            else
            {
                Console.WriteLine(string.Format("Id: {0}", server.Id));
                Console.WriteLine(string.Format("Disk Config: {0}", server.DiskConfig));
                Console.WriteLine(string.Format("Admin Pass: {0}", server.AdminPassword));
                Array.ForEach(server.Links, s =>
                                                {
                                                    Console.WriteLine("Link:");
                                                    Console.WriteLine(string.Format("\tRel: {0}", s.Rel));
                                                    Console.WriteLine(string.Format("\tHref: {0}", s.Href));
                                                });

                Console.WriteLine(Environment.NewLine + "Waiting for server to become active...");
                var details = provider.GetDetails(server.Id, identity);
                while (!details.Status.Equals("ACTIVE") && !details.Status.Equals("ERROR") && !details.Status.Equals("UNKNOWN") && !details.Status.Equals("SUSPENDED"))
                {
                    Thread.Sleep(1000);
                    details = provider.GetDetails(server.Id, identity);
                }
                Console.WriteLine("Server is active.");
                Console.WriteLine(Environment.NewLine + "Deleting server..");
                provider.DeleteServer(server.Id, identity);
                Console.WriteLine("Waiting for server to be deleted...");
                details = provider.GetDetails(server.Id, identity);
                while ( details != null && (!details.Status.Equals("DELETED") && !details.Status.Equals("ERROR") && !details.Status.Equals("UNKNOWN") && !details.Status.Equals("SUSPENDED")))
                {
                    Thread.Sleep(1000);
                    details = provider.GetDetails(server.Id, identity);
                }
                Console.WriteLine("Server deleted.");
            }
            Console.ReadLine();
        }
    }
}
