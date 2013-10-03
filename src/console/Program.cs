using System;
using System.Linq;
using System.Threading;
using Net.OpenStack.Testing.Integration;
using net.openstack.Core.Domain.Rackspace;
using net.openstack.Core.Providers.Rackspace;
using net.openstack.Providers.Rackspace;

namespace net.openstack.console
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Initialize();

            ICloudLoadBalancerProvider provider = new CloudLoadBalancerProvider(Bootstrapper.Settings.TestIdentity);

            var loadBalancers = provider.ListLoadBalancers();
            
            Console.WriteLine("Found {0} loadBalancers.", loadBalancers.Count());

            foreach (var simpleLoadBalancer in loadBalancers)
            {
                var loadBalancer = provider.GetLoadBalancer(simpleLoadBalancer.Id);
                Console.WriteLine("   - {0} ({1})", simpleLoadBalancer.Name, simpleLoadBalancer.Id);
                var nodes = provider.ListLoadBalancerNodes(simpleLoadBalancer.Id);
                Console.WriteLine("        - Found {0} Nodes:", nodes.Count());
                foreach (var node in nodes)
                {
                    Console.WriteLine("          - {0}:{1}:{2}:{3}", node.IPAddress, node.Port, node.Status, node.Condition);
                }
            }

            Console.WriteLine();
            Console.WriteLine("Creating new Load Balancer");
            provider.CreateLoadBalancer("My Test Load Balancer 1", LoadBalancerProtocol.HTTP,
                                        new[] {VirtualIPType.Public},
                                        new[]
                                            {
                                                new LoadBalancerNode
                                                    {
                                                        IPAddress = "10.182.33.87",
                                                        Port = 80,
                                                        Type = LoadBalancerNodeType.Primary,
                                                        Condition = LoadBalancerNodeCondition.Enabled
                                                    }
                                            });

            Thread.Sleep(2000);

            loadBalancers = provider.ListLoadBalancers();

            Console.WriteLine("Found {0} loadBalancers.", loadBalancers.Count());

            foreach (var simpleLoadBalancer in loadBalancers)
            {
                var loadBalancer = provider.GetLoadBalancer(simpleLoadBalancer.Id);
                Console.WriteLine("   - {0} ({1})", simpleLoadBalancer.Name, simpleLoadBalancer.Id);
                var nodes = provider.ListLoadBalancerNodes(simpleLoadBalancer.Id);
                Console.WriteLine("        - Found {0} Nodes:", nodes.Count());
                foreach (var node in nodes)
                {
                    Console.WriteLine("          - {0}:{1}:{2}:{3}", node.IPAddress, node.Port, node.Status, node.Condition);
                }
            }

            Console.WriteLine();
            Console.WriteLine("Waiting for My Test Load Balancer 1 to be ACTIVE");

            var testLoadBalancer = loadBalancers.First(lb => lb.Name.Equals("My Test Load Balancer 1", StringComparison.OrdinalIgnoreCase));
            while (testLoadBalancer.Status != LoadBalancerState.Active)
            {
                Thread.Sleep(2000);
                testLoadBalancer = provider.GetLoadBalancer(testLoadBalancer.Id);
            }

            Console.WriteLine();
            Console.WriteLine("Adding node to: My Test Load Balancer 1");

            provider.AddLoadBalancerNode(testLoadBalancer.Id, "10.181.20.180", LoadBalancerNodeCondition.Enabled, 80, LoadBalancerNodeType.Primary);

            var newNodes = provider.ListLoadBalancerNodes(testLoadBalancer.Id);
            Console.WriteLine("        - Found {0} Nodes:", newNodes.Count());
            foreach (var node in newNodes)
            {
                Console.WriteLine("          - {0}:{1}:{2}:{3}", node.IPAddress, node.Port, node.Status, node.Condition);
            }

            Console.WriteLine();
            Console.WriteLine("Waiting for My Test Load Balancer 1 to be ACTIVE");

            testLoadBalancer = provider.GetLoadBalancer(testLoadBalancer.Id);
            while (testLoadBalancer.Status != LoadBalancerState.Active)
            {
                Thread.Sleep(2000);
                testLoadBalancer = provider.GetLoadBalancer(testLoadBalancer.Id);
            }

            Console.WriteLine();
            Console.WriteLine("Removing test load balancer: My Test Load Balancer 1");

            provider.RemoveLoadBalancer(testLoadBalancer.Id);

            Console.WriteLine("Done!");
            Console.Read();
        }
    }
}
