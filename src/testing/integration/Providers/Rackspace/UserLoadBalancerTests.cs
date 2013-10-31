namespace Net.OpenStack.Testing.Integration.Providers.Rackspace
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using net.openstack.Core;
    using net.openstack.Providers.Rackspace;
    using net.openstack.Providers.Rackspace.Objects.LoadBalancers;
    using Newtonsoft.Json;
    using Security.Cryptography;
    using Security.Cryptography.X509Certificates;
    using CancellationToken = System.Threading.CancellationToken;
    using CancellationTokenSource = System.Threading.CancellationTokenSource;
    using Interlocked = System.Threading.Interlocked;
    using Path = System.IO.Path;
    using StringBuilder = System.Text.StringBuilder;

    /// <preliminary/>
    [TestClass]
    public class UserLoadBalancerTests
    {
        /// <summary>
        /// The prefix to use for names of load balancers created during integration testing.
        /// </summary>
        public static readonly string TestLoadBalancerPrefix = "UnitTestLB-";

        /// <summary>
        /// This method can be used to clean up load balancers created during integration testing.
        /// </summary>
        /// <remarks>
        /// The Cloud Load Balancer integration tests generally delete load balancers created
        /// during the tests, but test failures may lead to unused load balancers gathering
        /// on the system. This method searches for all load balancers matching the
        /// "integration testing" pattern (i.e., load balancers whose name starts with
        /// <see cref="TestLoadBalancerPrefix"/>), and attempts to delete them.
        /// <para>
        /// The deletion requests are sent in parallel, so a single deletion failure will not
        /// prevent the method from cleaning up other load balancers that can be successfully
        /// deleted. Note that the system does not increase the
        /// <see cref="ProviderBase{TProvider}.ConnectionLimit"/>, so the underlying REST
        /// requests may be pipelined if the number of load balancers to delete exceeds the
        /// default system connection limit.
        /// </para>
        /// </remarks>
        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        [TestCategory(TestCategories.Cleanup)]
        public async Task CleanupTestLoadBalancers()
        {
            ILoadBalancerService provider = CreateProvider();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(60)));
            string queueName = CreateRandomLoadBalancerName();

            LoadBalancer[] allLoadBalancers = ListAllLoadBalancers(provider, null, cancellationTokenSource.Token).Where(loadBalancer => loadBalancer.Name.StartsWith(TestLoadBalancerPrefix, StringComparison.OrdinalIgnoreCase)).ToArray();
            int blockSize = 10;
            for (int i = 0; i < allLoadBalancers.Length; i += blockSize)
            {
                await provider.RemoveLoadBalancerRangeAsync(
                    allLoadBalancers
                        .Skip(i)
                        .Take(blockSize)
                        .Select(loadBalancer =>
                            {
                                Console.WriteLine("Deleting load balancer: {0}", loadBalancer.Name);
                                return loadBalancer.Id;
                            })
                        .ToArray(), // included to ensure the Console.WriteLine is only executed once per load balancer
                    AsyncCompletionOption.RequestCompleted,
                    cancellationTokenSource.Token,
                    null);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public void TestListLoadBalancers()
        {
            ILoadBalancerService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(10))))
            {
                LoadBalancer[] loadBalancers = ListAllLoadBalancers(provider, null, cancellationTokenSource.Token).ToArray();
                if (!loadBalancers.Any())
                    Assert.Inconclusive("The account does not appear to contain any load balancers");

                foreach (LoadBalancer loadBalancer in loadBalancers)
                    Console.WriteLine("{0}: {1}", loadBalancer.Id, loadBalancer.Name);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public async Task TestGetLoadBalancer()
        {
            ILoadBalancerService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(10))))
            {
                foreach (LoadBalancer loadBalancer in ListAllLoadBalancers(provider, null, cancellationTokenSource.Token))
                {
                    Console.WriteLine("Basic information:");
                    Console.WriteLine(JsonConvert.SerializeObject(loadBalancer, Formatting.Indented));

                    LoadBalancer details = await provider.GetLoadBalancerAsync(loadBalancer.Id, cancellationTokenSource.Token);
                    Console.WriteLine("Detailed information:");
                    Console.WriteLine(JsonConvert.SerializeObject(details, Formatting.Indented));
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public async Task TestCreateLoadBalancer()
        {
            ILoadBalancerService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(60))))
            {
                IEnumerable<LoadBalancingProtocol> protocols = await provider.ListProtocolsAsync(cancellationTokenSource.Token);
                LoadBalancingProtocol httpProtocol = protocols.First(i => i.Name.Equals("HTTP", StringComparison.OrdinalIgnoreCase));

                string loadBalancerName = CreateRandomLoadBalancerName();

                LoadBalancerConfiguration configuration = new LoadBalancerConfiguration(
                    name: loadBalancerName,
                    nodes: null,
                    protocol: httpProtocol,
                    virtualAddresses: new[] { new LoadBalancerVirtualAddress(LoadBalancerVirtualAddressType.ServiceNet) },
                    halfClosed: null,
                    accessList: null,
                    algorithm: LoadBalancingAlgorithm.RoundRobin,
                    connectionLogging: null,
                    contentCaching: null,
                    connectionThrottle: null,
                    healthMonitor: null,
                    metadata: null,
                    timeout: null,
                    sessionPersistence: null);
                LoadBalancer tempLoadBalancer = await provider.CreateLoadBalancerAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                foreach (LoadBalancer loadBalancer in ListAllLoadBalancers(provider, null, cancellationTokenSource.Token))
                {
                    Console.WriteLine("{0}: {1}", loadBalancer.Id, loadBalancer.Name);
                }

                await provider.RemoveLoadBalancerAsync(tempLoadBalancer.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public async Task TestUpdateLoadBalancer()
        {
            ILoadBalancerService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(60))))
            {
                IEnumerable<LoadBalancingProtocol> protocols = await provider.ListProtocolsAsync(cancellationTokenSource.Token);
                LoadBalancingProtocol httpProtocol = protocols.First(i => i.Name.Equals("HTTP", StringComparison.OrdinalIgnoreCase));

                string loadBalancerName = CreateRandomLoadBalancerName();
                string loadBalancerRename = CreateRandomLoadBalancerName();

                LoadBalancerConfiguration configuration = new LoadBalancerConfiguration(
                    name: loadBalancerName,
                    nodes: null,
                    protocol: httpProtocol,
                    virtualAddresses: new[] { new LoadBalancerVirtualAddress(LoadBalancerVirtualAddressType.ServiceNet) },
                    halfClosed: null,
                    accessList: null,
                    algorithm: LoadBalancingAlgorithm.RoundRobin,
                    connectionLogging: null,
                    contentCaching: null,
                    connectionThrottle: null,
                    healthMonitor: null,
                    metadata: null,
                    timeout: null,
                    sessionPersistence: null);
                LoadBalancer tempLoadBalancer = await provider.CreateLoadBalancerAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                Assert.IsNotNull(tempLoadBalancer);
                Assert.AreEqual(loadBalancerName, tempLoadBalancer.Name);

                LoadBalancerUpdate updatedConfiguration = new LoadBalancerUpdate(name: loadBalancerRename);
                await provider.UpdateLoadBalancerAsync(tempLoadBalancer.Id, updatedConfiguration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                LoadBalancer renamed = await provider.GetLoadBalancerAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(renamed);
                Assert.AreEqual(tempLoadBalancer.Id, renamed.Id);
                Assert.AreEqual(loadBalancerRename, renamed.Name);

                await provider.RemoveLoadBalancerAsync(tempLoadBalancer.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public async Task TestRemoveLoadBalancerRange()
        {
            ILoadBalancerService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(30))))
            {
                IEnumerable<LoadBalancingProtocol> protocols = await provider.ListProtocolsAsync(cancellationTokenSource.Token);
                LoadBalancingProtocol httpProtocol = protocols.First(i => i.Name.Equals("HTTP", StringComparison.OrdinalIgnoreCase));

                string loadBalancerName = CreateRandomLoadBalancerName();

                LoadBalancerConfiguration configuration = new LoadBalancerConfiguration(
                    name: loadBalancerName,
                    nodes: null,
                    protocol: httpProtocol,
                    virtualAddresses: new[] { new LoadBalancerVirtualAddress(LoadBalancerVirtualAddressType.ServiceNet) },
                    halfClosed: null,
                    accessList: null,
                    algorithm: LoadBalancingAlgorithm.RoundRobin,
                    connectionLogging: null,
                    contentCaching: null,
                    connectionThrottle: null,
                    healthMonitor: null,
                    metadata: null,
                    timeout: null,
                    sessionPersistence: null);
                Task<LoadBalancer> tempLoadBalancer = provider.CreateLoadBalancerAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                string loadBalancer2Name = CreateRandomLoadBalancerName();

                LoadBalancerConfiguration configuration2 = new LoadBalancerConfiguration(
                    name: loadBalancer2Name,
                    nodes: null,
                    protocol: httpProtocol,
                    virtualAddresses: new[] { new LoadBalancerVirtualAddress(LoadBalancerVirtualAddressType.ServiceNet) },
                    halfClosed: null,
                    accessList: null,
                    algorithm: LoadBalancingAlgorithm.RoundRobin,
                    connectionLogging: null,
                    contentCaching: null,
                    connectionThrottle: null,
                    healthMonitor: null,
                    metadata: null,
                    timeout: null,
                    sessionPersistence: null);
                Task<LoadBalancer> tempLoadBalancer2 = provider.CreateLoadBalancerAsync(configuration2, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                await Task.Factory.ContinueWhenAll(new Task[] { tempLoadBalancer, tempLoadBalancer2 }, TaskExtrasExtensions.PropagateExceptions);
                Assert.AreEqual(LoadBalancerStatus.Active, tempLoadBalancer.Result.Status);
                Assert.AreEqual(LoadBalancerStatus.Active, tempLoadBalancer2.Result.Status);

                /* Cleanup
                 */

                await provider.RemoveLoadBalancerRangeAsync(new[] { tempLoadBalancer.Result.Id, tempLoadBalancer2.Result.Id }, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public async Task TestGetErrorPage()
        {
            ILoadBalancerService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(10))))
            {
                LoadBalancer[] loadBalancers = ListAllLoadBalancers(provider, null, cancellationTokenSource.Token).ToArray();
                if (!loadBalancers.Any())
                    Assert.Inconclusive("The account does not appear to contain any load balancers");

                foreach (LoadBalancer loadBalancer in loadBalancers)
                {
                    Console.WriteLine("{0}: {1}", loadBalancer.Id, loadBalancer.Name);
                    Console.WriteLine("Error page:");
                    Console.WriteLine(await provider.GetErrorPageAsync(loadBalancer.Id, cancellationTokenSource.Token));
                }
            }
        }

        /// <summary>
        /// This method performs integration testing for the following methods:
        /// <list type="bullet">
        /// <item><see cref="ILoadBalancerService.GetErrorPageAsync"/></item>
        /// <item><see cref="ILoadBalancerService.SetErrorPageAsync"/></item>
        /// <item><see cref="ILoadBalancerService.RemoveErrorPageAsync"/></item>
        /// </list>
        /// </summary>
        /// <returns>A <see cref="Task"/> object representing the asynchronous operation.</returns>
        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public async Task TestModifyErrorPage()
        {
            ILoadBalancerService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(30))))
            {
                IEnumerable<LoadBalancingProtocol> protocols = await provider.ListProtocolsAsync(cancellationTokenSource.Token);
                LoadBalancingProtocol httpProtocol = protocols.First(i => i.Name.Equals("HTTP", StringComparison.OrdinalIgnoreCase));

                string loadBalancerName = CreateRandomLoadBalancerName();

                LoadBalancerConfiguration configuration = new LoadBalancerConfiguration(
                    name: loadBalancerName,
                    nodes: null,
                    protocol: httpProtocol,
                    virtualAddresses: new[] { new LoadBalancerVirtualAddress(LoadBalancerVirtualAddressType.ServiceNet) },
                    halfClosed: null,
                    accessList: null,
                    algorithm: LoadBalancingAlgorithm.RoundRobin,
                    connectionLogging: null,
                    contentCaching: null,
                    connectionThrottle: null,
                    healthMonitor: null,
                    metadata: null,
                    timeout: null,
                    sessionPersistence: null);
                LoadBalancer tempLoadBalancer = await provider.CreateLoadBalancerAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                Assert.AreEqual(LoadBalancerStatus.Active, tempLoadBalancer.Status);

                Console.WriteLine("Error page:");
                string defaultErrorPage = await provider.GetErrorPageAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                string customErrorPage = "Some custom error...";

                await provider.SetErrorPageAsync(tempLoadBalancer.Id, customErrorPage, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                LoadBalancer details = await provider.GetLoadBalancerAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.AreEqual(LoadBalancerStatus.Active, details.Status);
                Assert.AreEqual(customErrorPage, await provider.GetErrorPageAsync(tempLoadBalancer.Id, cancellationTokenSource.Token));

                await provider.RemoveErrorPageAsync(tempLoadBalancer.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                details = await provider.GetLoadBalancerAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.AreEqual(LoadBalancerStatus.Active, details.Status);
                Assert.AreEqual(defaultErrorPage, await provider.GetErrorPageAsync(tempLoadBalancer.Id, cancellationTokenSource.Token));

                /* Cleanup
                 */

                await provider.RemoveLoadBalancerAsync(tempLoadBalancer.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public async Task TestGetStatistics()
        {
            ILoadBalancerService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(10))))
            {
                LoadBalancer[] loadBalancers = ListAllLoadBalancers(provider, null, cancellationTokenSource.Token).ToArray();
                if (!loadBalancers.Any())
                    Assert.Inconclusive("The account does not appear to contain any load balancers");

                foreach (LoadBalancer loadBalancer in loadBalancers)
                {
                    LoadBalancerStatistics statistics = await provider.GetStatisticsAsync(loadBalancer.Id, cancellationTokenSource.Token);
                    Console.WriteLine("{0}: {1}", loadBalancer.Id, loadBalancer.Name);
                    Console.WriteLine("  Predefined Statistics");
                    Console.WriteLine("    Connection Error: {0}", statistics.ConnectionError);
                    Console.WriteLine("    Connection Failure: {0}", statistics.ConnectionFailure);
                    Console.WriteLine("    Connection Timed Out: {0}", statistics.ConnectionTimedOut);
                    Console.WriteLine("    Data Timed Out: {0}", statistics.DataTimedOut);
                    Console.WriteLine("    Keep Alive Timed Out: {0}", statistics.KeepAliveTimedOut);
                    Console.WriteLine("    Max Connections: {0}", statistics.MaxConnections);
                    Console.WriteLine("  Generic Statistics");
                    foreach (var pair in statistics)
                        Console.WriteLine("    {0}: {1}", pair.Key, pair.Value);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public async Task TestAddNode()
        {
            ILoadBalancerService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(60))))
            {
                IPHostEntry entry = await Task.Factory.FromAsync<IPHostEntry>(Dns.BeginGetHostEntry("docs.rackspace.com", null, null), Dns.EndGetHostEntry);

                IEnumerable<LoadBalancingProtocol> protocols = await provider.ListProtocolsAsync(cancellationTokenSource.Token);
                LoadBalancingProtocol httpProtocol = protocols.First(i => i.Name.Equals("HTTP", StringComparison.OrdinalIgnoreCase));

                string loadBalancerName = CreateRandomLoadBalancerName();

                LoadBalancerConfiguration configuration = new LoadBalancerConfiguration(
                    name: loadBalancerName,
                    nodes: null,
                    protocol: httpProtocol,
                    virtualAddresses: new[] { new LoadBalancerVirtualAddress(LoadBalancerVirtualAddressType.ServiceNet) },
                    algorithm: LoadBalancingAlgorithm.RoundRobin);
                LoadBalancer tempLoadBalancer = await provider.CreateLoadBalancerAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                Assert.AreEqual(LoadBalancerStatus.Active, tempLoadBalancer.Status);

                NodeConfiguration nodeConfiguration = new NodeConfiguration(entry.AddressList[0], 80, NodeCondition.Enabled, NodeType.Primary, null);
                Node node = await provider.AddNodeAsync(tempLoadBalancer.Id, nodeConfiguration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                Assert.AreEqual(NodeStatus.Online, node.Status);
                Assert.AreEqual(nodeConfiguration.Address, node.Address);

                LoadBalancer loadBalancer = await provider.GetLoadBalancerAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(loadBalancer.Nodes);
                Assert.AreEqual(1, loadBalancer.Nodes.Count);

                Node[] listNodes = ListAllLoadBalancerNodes(provider, tempLoadBalancer.Id, null, cancellationTokenSource.Token).ToArray();
                Assert.IsNotNull(listNodes);
                Assert.AreEqual(1, listNodes.Length);

                Node getNode = await provider.GetNodeAsync(tempLoadBalancer.Id, listNodes[0].Id, cancellationTokenSource.Token);
                Assert.IsNotNull(getNode);
                Assert.AreEqual(node.Id, getNode.Id);

                await provider.RemoveNodeAsync(tempLoadBalancer.Id, node.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                loadBalancer = await provider.GetLoadBalancerAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                if (loadBalancer.Nodes != null)
                    Assert.AreEqual(0, loadBalancer.Nodes.Count);

                /* Cleanup
                 */

                await provider.RemoveLoadBalancerAsync(tempLoadBalancer.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public async Task TestAddNodes()
        {
            ILoadBalancerService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(60))))
            {
                IPHostEntry entryA = await Task.Factory.FromAsync<IPHostEntry>(Dns.BeginGetHostEntry("docs.rackspace.com", null, null), Dns.EndGetHostEntry);
                IPHostEntry entryB = await Task.Factory.FromAsync<IPHostEntry>(Dns.BeginGetHostEntry("developer.rackspace.com", null, null), Dns.EndGetHostEntry);

                IEnumerable<LoadBalancingProtocol> protocols = await provider.ListProtocolsAsync(cancellationTokenSource.Token);
                LoadBalancingProtocol httpProtocol = protocols.First(i => i.Name.Equals("HTTP", StringComparison.OrdinalIgnoreCase));

                string loadBalancerName = CreateRandomLoadBalancerName();

                LoadBalancerConfiguration configuration = new LoadBalancerConfiguration(
                    name: loadBalancerName,
                    nodes: null,
                    protocol: httpProtocol,
                    virtualAddresses: new[] { new LoadBalancerVirtualAddress(LoadBalancerVirtualAddressType.ServiceNet) },
                    algorithm: LoadBalancingAlgorithm.RoundRobin);
                LoadBalancer tempLoadBalancer = await provider.CreateLoadBalancerAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                Assert.AreEqual(LoadBalancerStatus.Active, tempLoadBalancer.Status);

                NodeConfiguration[] nodeConfigurations =
                    new[]
                    {
                        new NodeConfiguration(entryA.AddressList[0], 80, NodeCondition.Enabled, NodeType.Primary, null),
                        new NodeConfiguration(entryB.AddressList[0], 80, NodeCondition.Enabled, NodeType.Primary, null)
                    };

                IEnumerable<Node> nodes = await provider.AddNodeRangeAsync(tempLoadBalancer.Id, nodeConfigurations, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                Node[] nodesArray = nodes.ToArray();
                Assert.AreEqual(2, nodesArray.Length);

                Assert.AreEqual(NodeStatus.Online, nodesArray[0].Status);
                Assert.AreEqual(NodeStatus.Online, nodesArray[1].Status);

                if (nodeConfigurations[0].Address == nodesArray[1].Address)
                    nodesArray[1] = Interlocked.Exchange(ref nodesArray[0], nodesArray[1]);

                Assert.AreEqual(nodeConfigurations[0].Address, nodesArray[0].Address);
                Assert.AreEqual(nodeConfigurations[1].Address, nodesArray[1].Address);

                LoadBalancer loadBalancer = await provider.GetLoadBalancerAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(loadBalancer.Nodes);
                Assert.AreEqual(2, loadBalancer.Nodes.Count);

                Node[] listNodes = ListAllLoadBalancerNodes(provider, tempLoadBalancer.Id, null, cancellationTokenSource.Token).ToArray();
                Assert.IsNotNull(listNodes);
                Assert.AreEqual(2, listNodes.Length);

                await provider.RemoveNodeRangeAsync(tempLoadBalancer.Id, nodesArray.Select(i => i.Id), AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                loadBalancer = await provider.GetLoadBalancerAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                if (loadBalancer.Nodes != null)
                    Assert.AreEqual(0, loadBalancer.Nodes.Count);

                /* Cleanup
                 */

                await provider.RemoveLoadBalancerAsync(tempLoadBalancer.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public async Task TestUpdateNode()
        {
            ILoadBalancerService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(60))))
            {
                IPHostEntry entry = await Task.Factory.FromAsync<IPHostEntry>(Dns.BeginGetHostEntry("docs.rackspace.com", null, null), Dns.EndGetHostEntry);

                IEnumerable<LoadBalancingProtocol> protocols = await provider.ListProtocolsAsync(cancellationTokenSource.Token);
                LoadBalancingProtocol httpProtocol = protocols.First(i => i.Name.Equals("HTTP", StringComparison.OrdinalIgnoreCase));

                string loadBalancerName = CreateRandomLoadBalancerName();

                LoadBalancerConfiguration configuration = new LoadBalancerConfiguration(
                    name: loadBalancerName,
                    nodes: new[] { new NodeConfiguration(entry.AddressList[0], 80, NodeCondition.Enabled, NodeType.Primary, null) },
                    protocol: httpProtocol,
                    virtualAddresses: new[] { new LoadBalancerVirtualAddress(LoadBalancerVirtualAddressType.ServiceNet) },
                    algorithm: LoadBalancingAlgorithm.RoundRobin);
                LoadBalancer tempLoadBalancer = await provider.CreateLoadBalancerAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                Node node = tempLoadBalancer.Nodes.Single();
                Assert.AreEqual(LoadBalancerStatus.Active, tempLoadBalancer.Status);

                NodeUpdate updatedNodeConfiguration = new NodeUpdate(condition: NodeCondition.Draining);
                await provider.UpdateNodeAsync(tempLoadBalancer.Id, node.Id, updatedNodeConfiguration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                /* Cleanup
                 */

                await provider.RemoveLoadBalancerAsync(tempLoadBalancer.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public async Task TestListNodeServiceEvents()
        {
            ILoadBalancerService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(60))))
            {
                IPHostEntry entry = await Task.Factory.FromAsync<IPHostEntry>(Dns.BeginGetHostEntry("docs.rackspace.com", null, null), Dns.EndGetHostEntry);

                IEnumerable<LoadBalancingProtocol> protocols = await provider.ListProtocolsAsync(cancellationTokenSource.Token);
                LoadBalancingProtocol httpProtocol = protocols.First(i => i.Name.Equals("HTTP", StringComparison.OrdinalIgnoreCase));

                string loadBalancerName = CreateRandomLoadBalancerName();

                LoadBalancerConfiguration configuration = new LoadBalancerConfiguration(
                    name: loadBalancerName,
                    nodes: new[] { new NodeConfiguration(entry.AddressList[0], 80, NodeCondition.Enabled, NodeType.Primary, null) },
                    protocol: httpProtocol,
                    virtualAddresses: new[] { new LoadBalancerVirtualAddress(LoadBalancerVirtualAddressType.ServiceNet) },
                    algorithm: LoadBalancingAlgorithm.RoundRobin);
                LoadBalancer tempLoadBalancer = await provider.CreateLoadBalancerAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                Node node = tempLoadBalancer.Nodes.Single();
                Assert.AreEqual(LoadBalancerStatus.Active, tempLoadBalancer.Status);

                NodeUpdate updatedNodeConfiguration = new NodeUpdate(condition: NodeCondition.Draining);
                await provider.UpdateNodeAsync(tempLoadBalancer.Id, node.Id, updatedNodeConfiguration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                NodeServiceEvent[] serviceEvents = ListAllNodeServiceEvents(provider, tempLoadBalancer.Id, null, cancellationTokenSource.Token).ToArray();
                if (!serviceEvents.Any())
                    Assert.Inconclusive("The load balancer did not report any node service events.");

                /* Cleanup
                 */

                await provider.RemoveLoadBalancerAsync(tempLoadBalancer.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public async Task TestListVirtualAddresses()
        {
            ILoadBalancerService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(60))))
            {
                IEnumerable<LoadBalancingProtocol> protocols = await provider.ListProtocolsAsync(cancellationTokenSource.Token);
                LoadBalancingProtocol httpProtocol = protocols.First(i => i.Name.Equals("HTTP", StringComparison.OrdinalIgnoreCase));

                string loadBalancerName = CreateRandomLoadBalancerName();

                LoadBalancerConfiguration configuration = new LoadBalancerConfiguration(
                    name: loadBalancerName,
                    nodes: null,
                    protocol: httpProtocol,
                    virtualAddresses: new[] { new LoadBalancerVirtualAddress(LoadBalancerVirtualAddressType.ServiceNet) },
                    algorithm: LoadBalancingAlgorithm.RoundRobin);
                LoadBalancer tempLoadBalancer = await provider.CreateLoadBalancerAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                IEnumerable<LoadBalancerVirtualAddress> addresses = await provider.ListVirtualAddressesAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                if (!addresses.Any())
                    Assert.Inconclusive("The load balancer did not report any virtual addresses.");

                foreach (var address in addresses)
                {
                    Console.WriteLine("{0}: {1} ({2}, {3})", address.Id, address.Address, address.IPVersion, address.Type);
                }

                /* Cleanup
                 */

                await provider.RemoveLoadBalancerAsync(tempLoadBalancer.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public async Task TestAddRemoveVirtualAddresses()
        {
            ILoadBalancerService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(240))))
            {
                IEnumerable<LoadBalancingProtocol> protocols = await provider.ListProtocolsAsync(cancellationTokenSource.Token);
                LoadBalancingProtocol httpProtocol = protocols.First(i => i.Name.Equals("HTTP", StringComparison.OrdinalIgnoreCase));

                string loadBalancerName = CreateRandomLoadBalancerName();

                LoadBalancerConfiguration configuration = new LoadBalancerConfiguration(
                    name: loadBalancerName,
                    nodes: null,
                    protocol: httpProtocol,
                    virtualAddresses: new[] { new LoadBalancerVirtualAddress(LoadBalancerVirtualAddressType.Public) },
                    algorithm: LoadBalancingAlgorithm.RoundRobin);
                LoadBalancer tempLoadBalancer = await provider.CreateLoadBalancerAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                IEnumerable<LoadBalancerVirtualAddress> addresses = await provider.ListVirtualAddressesAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                if (!addresses.Any())
                    Assert.Inconclusive("The load balancer did not report any virtual addresses.");

                Console.WriteLine("Initial virtual addresses");
                foreach (var address in addresses)
                    Console.WriteLine("{0}: {1} ({2}, {3})", address.Id, address.Address, address.IPVersion, address.Type);

                List<LoadBalancerVirtualAddress> addedAddresses = new List<LoadBalancerVirtualAddress>();

                for (int i = 0; i < 4; i++)
                {
                    // add a virtual address
                    LoadBalancerVirtualAddress addedAddress = await provider.AddVirtualAddressAsync(tempLoadBalancer.Id, LoadBalancerVirtualAddressType.Public, AddressFamily.InterNetworkV6, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                    Assert.IsNotNull(addedAddress);
                    Assert.IsNotNull(addedAddress.Address);
                    Assert.AreEqual(AddressFamily.InterNetworkV6, addedAddress.Address.AddressFamily);
                    addedAddresses.Add(addedAddress);
                }

                addresses = await provider.ListVirtualAddressesAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Console.WriteLine("After adding IPv6 virtual address");
                foreach (var address in addresses)
                    Console.WriteLine("{0}: {1} ({2}, {3})", address.Id, address.Address, address.IPVersion, address.Type);

                // remove a single virtual address
                await provider.RemoveVirtualAddressAsync(tempLoadBalancer.Id, addedAddresses[0].Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                addresses = await provider.ListVirtualAddressesAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Console.WriteLine("After removing 1 IPv6 virtual address");
                foreach (var address in addresses)
                    Console.WriteLine("{0}: {1} ({2}, {3})", address.Id, address.Address, address.IPVersion, address.Type);

                // remove multiple virtual addresses
                await provider.RemoveVirtualAddressRangeAsync(tempLoadBalancer.Id, new[] { addedAddresses[1].Id, addedAddresses[3].Id }, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                addresses = await provider.ListVirtualAddressesAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Console.WriteLine("After removing 1 IPv6 virtual address");
                foreach (var address in addresses)
                    Console.WriteLine("{0}: {1} ({2}, {3})", address.Id, address.Address, address.IPVersion, address.Type);

                /* Cleanup
                 */

                await provider.RemoveLoadBalancerAsync(tempLoadBalancer.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public async Task TestListAllowedDomains()
        {
            ILoadBalancerService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(10))))
            {
                Console.WriteLine("Allowed domains:");
                IEnumerable<string> allowedDomains = await provider.ListAllowedDomainsAsync(cancellationTokenSource.Token);
                Assert.IsNotNull(allowedDomains);

                foreach (string domain in allowedDomains)
                {
                    Assert.IsNotNull(domain);
                    Console.WriteLine("    {0}", domain);
                }

                if (!allowedDomains.Any())
                    Assert.Inconclusive("No allowed domains were returned by the call.");
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public void TestListBillableLoadBalancers()
        {
            Assert.Inconclusive("Not yet implemented.");
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public void TestListAccountLevelUsage()
        {
            Assert.Inconclusive("Not yet implemented.");
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public void TestListHistoricalUsage()
        {
            Assert.Inconclusive("Not yet implemented.");
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public void TestListCurrentUsage()
        {
            Assert.Inconclusive("Not yet implemented.");
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public async Task TestAccessLists()
        {
            ILoadBalancerService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(60))))
            {
                IEnumerable<LoadBalancingProtocol> protocols = await provider.ListProtocolsAsync(cancellationTokenSource.Token);
                LoadBalancingProtocol httpProtocol = protocols.First(i => i.Name.Equals("HTTP", StringComparison.OrdinalIgnoreCase));

                string loadBalancerName = CreateRandomLoadBalancerName();

                LoadBalancerConfiguration configuration = new LoadBalancerConfiguration(
                    name: loadBalancerName,
                    nodes: null,
                    protocol: httpProtocol,
                    virtualAddresses: new[] { new LoadBalancerVirtualAddress(LoadBalancerVirtualAddressType.ServiceNet) },
                    algorithm: LoadBalancingAlgorithm.RoundRobin);
                LoadBalancer tempLoadBalancer = await provider.CreateLoadBalancerAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                // verify initially null
                IEnumerable<NetworkItem> accessList = await provider.ListAccessListAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(accessList);
                Assert.IsFalse(accessList.Any());

                // allow docs.rackspace.com
                IPHostEntry resolved = await Task.Factory.FromAsync<IPHostEntry>(Dns.BeginGetHostEntry("docs.rackspace.com", null, null), Dns.EndGetHostEntry);
                NetworkItem firstNetworkItem = new NetworkItem(resolved.AddressList[0], AccessType.Allow);
                await provider.CreateAccessListAsync(tempLoadBalancer.Id, firstNetworkItem, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                //List<NetworkItem> expected = new List<NetworkItem> { firstNetworkItem };
                LoadBalancer loadBalancer = await provider.GetLoadBalancerAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(loadBalancer.AccessList);
                Assert.AreEqual(1, loadBalancer.AccessList.Count);
                Assert.AreEqual(firstNetworkItem.Address, loadBalancer.AccessList[0].Address);
                Assert.AreEqual(firstNetworkItem.AccessType, loadBalancer.AccessList[0].AccessType);

                //for (int i = 0; i < expected.Count; i++)
                //{
                //    Assert.AreEqual(expected[i].Address, loadBalancer.AccessList[i].Address);
                //    Assert.AreEqual(expected[i].AccessType, loadBalancer.AccessList[i].AccessType);
                //}

                // allow developer.rackspace.com
                // deny google.com, yahoo.com, microsoft.com, amazon.com
                NetworkItem[] batch =
                    new[]
                    {
                        new NetworkItem(
                            (await Task.Factory.FromAsync<IPHostEntry>(Dns.BeginGetHostEntry("google.com", null, null), Dns.EndGetHostEntry)).AddressList[0],
                            AccessType.Deny),
                        new NetworkItem(
                            (await Task.Factory.FromAsync<IPHostEntry>(Dns.BeginGetHostEntry("developer.rackspace.com", null, null), Dns.EndGetHostEntry)).AddressList[0],
                            AccessType.Allow),
                        new NetworkItem(
                            (await Task.Factory.FromAsync<IPHostEntry>(Dns.BeginGetHostEntry("yahoo.com", null, null), Dns.EndGetHostEntry)).AddressList[0],
                            AccessType.Deny),
                        new NetworkItem(
                            (await Task.Factory.FromAsync<IPHostEntry>(Dns.BeginGetHostEntry("microsoft.com", null, null), Dns.EndGetHostEntry)).AddressList[0],
                            AccessType.Deny),
                        new NetworkItem(
                            (await Task.Factory.FromAsync<IPHostEntry>(Dns.BeginGetHostEntry("amazon.com", null, null), Dns.EndGetHostEntry)).AddressList[0],
                            AccessType.Deny),
                    };
                await provider.CreateAccessListAsync(tempLoadBalancer.Id, batch, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                //expected.AddRange(batch.AsEnumerable().Reverse());
                loadBalancer = await provider.GetLoadBalancerAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(loadBalancer.AccessList);
                Assert.AreEqual(6, loadBalancer.AccessList.Count);

                //for (int i = 0; i < expected.Count; i++)
                //{
                //    Assert.AreEqual(expected[i].Address, loadBalancer.AccessList[i].Address);
                //    Assert.AreEqual(expected[i].AccessType, loadBalancer.AccessList[i].AccessType);
                //}

                // remove a single item from the middle of the access list: developer.rackspace.com
                await provider.RemoveAccessListAsync(tempLoadBalancer.Id, loadBalancer.AccessList[2].Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                //expected.RemoveAt(2);
                loadBalancer = await provider.GetLoadBalancerAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(loadBalancer.AccessList);
                Assert.AreEqual(5, loadBalancer.AccessList.Count);

                //for (int i = 0; i < expected.Count; i++)
                //{
                //    Assert.AreEqual(expected[i].Address, loadBalancer.AccessList[i].Address);
                //    Assert.AreEqual(expected[i].AccessType, loadBalancer.AccessList[i].AccessType);
                //}

                // remove two items from the middle of the access list: developer.rackspace.com
                await provider.RemoveAccessListRangeAsync(tempLoadBalancer.Id, new[] { loadBalancer.AccessList[0].Id, loadBalancer.AccessList[2].Id }, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                //expected.RemoveAt(2);
                //expected.RemoveAt(0);
                loadBalancer = await provider.GetLoadBalancerAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(loadBalancer.AccessList);
                Assert.AreEqual(3, loadBalancer.AccessList.Count);

                //for (int i = 0; i < expected.Count; i++)
                //{
                //    Assert.AreEqual(expected[i].Address, loadBalancer.AccessList[i].Address);
                //    Assert.AreEqual(expected[i].AccessType, loadBalancer.AccessList[i].AccessType);
                //}

                // clear the access list
                await provider.ClearAccessListAsync(tempLoadBalancer.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                // clear the access list again
                try
                {
                    try
                    {
                        await provider.ClearAccessListAsync(tempLoadBalancer.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                    }
                    catch (WebException ex)
                    {
                        throw new AggregateException(ex);
                    }
                }
                catch (AggregateException aggregate)
                {
                    aggregate.Flatten().Handle(
                        ex =>
                        {
                            WebException webException = ex as WebException;
                            if (webException == null)
                                return false;

                            HttpWebResponse response = webException.Response as HttpWebResponse;
                            if (response == null)
                                return false;

                            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
                            return true;
                        });
                }

                /* Cleanup
                 */
                await provider.RemoveLoadBalancerAsync(tempLoadBalancer.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public async Task TestSetHttpCookieSessionPersistence()
        {
            ILoadBalancerService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(60))))
            {
                IEnumerable<LoadBalancingProtocol> protocols = await provider.ListProtocolsAsync(cancellationTokenSource.Token);
                LoadBalancingProtocol httpProtocol = protocols.First(i => i.Name.Equals("HTTP", StringComparison.OrdinalIgnoreCase));

                string loadBalancerName = CreateRandomLoadBalancerName();

                LoadBalancerConfiguration configuration = new LoadBalancerConfiguration(
                    name: loadBalancerName,
                    nodes: null,
                    protocol: httpProtocol,
                    virtualAddresses: new[] { new LoadBalancerVirtualAddress(LoadBalancerVirtualAddressType.ServiceNet) },
                    halfClosed: null,
                    accessList: null,
                    algorithm: LoadBalancingAlgorithm.RoundRobin,
                    connectionLogging: null,
                    contentCaching: null,
                    connectionThrottle: null,
                    healthMonitor: null,
                    metadata: null,
                    timeout: null,
                    sessionPersistence: null);
                LoadBalancer tempLoadBalancer = await provider.CreateLoadBalancerAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                // verify initially null
                SessionPersistence sessionPersistence = await provider.GetSessionPersistenceAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(sessionPersistence);
                Assert.IsNull(sessionPersistence.PersistenceType);

                // set to cookie
                await provider.SetSessionPersistenceAsync(tempLoadBalancer.Id, new SessionPersistence(SessionPersistenceType.HttpCookie), AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                sessionPersistence = await provider.GetSessionPersistenceAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(sessionPersistence);
                Assert.AreEqual(SessionPersistenceType.HttpCookie, sessionPersistence.PersistenceType);

                // set to cookie again
                await provider.SetSessionPersistenceAsync(tempLoadBalancer.Id, new SessionPersistence(SessionPersistenceType.HttpCookie), AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                sessionPersistence = await provider.GetSessionPersistenceAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(sessionPersistence);
                Assert.AreEqual(SessionPersistenceType.HttpCookie, sessionPersistence.PersistenceType);

                // should fail to set to source address
                try
                {
                    try
                    {
                        await provider.SetSessionPersistenceAsync(tempLoadBalancer.Id, new SessionPersistence(SessionPersistenceType.SourceAddress), AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                        Assert.Fail("Expected a WebException");
                    }
                    catch (WebException ex)
                    {
                        throw new AggregateException(ex);
                    }
                }
                catch (AggregateException aggregate)
                {
                    aggregate.Flatten().Handle(
                        ex =>
                        {
                            WebException webException = ex as WebException;
                            if (webException == null)
                                return false;

                            HttpWebResponse response = webException.Response as HttpWebResponse;
                            if (response == null)
                                return false;

                            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
                            return true;
                        });
                }

                sessionPersistence = await provider.GetSessionPersistenceAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(sessionPersistence);
                Assert.AreEqual(SessionPersistenceType.HttpCookie, sessionPersistence.PersistenceType);

                // set to none
                await provider.RemoveSessionPersistenceAsync(tempLoadBalancer.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                sessionPersistence = await provider.GetSessionPersistenceAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(sessionPersistence);
                Assert.IsNull(sessionPersistence.PersistenceType);

                try
                {
                    try
                    {
                        // set to none again
                        await provider.RemoveSessionPersistenceAsync(tempLoadBalancer.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                        Assert.Fail("Expected a WebException");
                    }
                    catch (WebException ex)
                    {
                        throw new AggregateException(ex);
                    }
                }
                catch (AggregateException aggregate)
                {
                    aggregate.Flatten().Handle(
                        ex =>
                        {
                            WebException webException = ex as WebException;
                            if (webException == null)
                                return false;

                            HttpWebResponse response = webException.Response as HttpWebResponse;
                            if (response == null)
                                return false;

                            Assert.AreEqual((HttpStatusCode)422, response.StatusCode);
                            return true;
                        });
                }

                sessionPersistence = await provider.GetSessionPersistenceAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(sessionPersistence);
                Assert.IsNull(sessionPersistence.PersistenceType);

                /* Cleanup
                 */

                await provider.RemoveLoadBalancerAsync(tempLoadBalancer.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public async Task TestSetSourceAddressSessionPersistence()
        {
            ILoadBalancerService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(60))))
            {
                IEnumerable<LoadBalancingProtocol> protocols = await provider.ListProtocolsAsync(cancellationTokenSource.Token);
                LoadBalancingProtocol ldapProtocol = protocols.First(i => i.Name.Equals("LDAP", StringComparison.OrdinalIgnoreCase));

                string loadBalancerName = CreateRandomLoadBalancerName();

                LoadBalancerConfiguration configuration = new LoadBalancerConfiguration(
                    name: loadBalancerName,
                    nodes: null,
                    protocol: ldapProtocol,
                    virtualAddresses: new[] { new LoadBalancerVirtualAddress(LoadBalancerVirtualAddressType.ServiceNet) },
                    halfClosed: null,
                    accessList: null,
                    algorithm: LoadBalancingAlgorithm.RoundRobin,
                    connectionLogging: null,
                    contentCaching: null,
                    connectionThrottle: null,
                    healthMonitor: null,
                    metadata: null,
                    timeout: null,
                    sessionPersistence: null);
                LoadBalancer tempLoadBalancer = await provider.CreateLoadBalancerAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                // verify initially null
                SessionPersistence sessionPersistence = await provider.GetSessionPersistenceAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(sessionPersistence);
                Assert.IsNull(sessionPersistence.PersistenceType);

                // set to source address
                await provider.SetSessionPersistenceAsync(tempLoadBalancer.Id, new SessionPersistence(SessionPersistenceType.SourceAddress), AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                sessionPersistence = await provider.GetSessionPersistenceAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(sessionPersistence);
                Assert.AreEqual(SessionPersistenceType.SourceAddress, sessionPersistence.PersistenceType);

                // set to source address again
                await provider.SetSessionPersistenceAsync(tempLoadBalancer.Id, new SessionPersistence(SessionPersistenceType.SourceAddress), AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                sessionPersistence = await provider.GetSessionPersistenceAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(sessionPersistence);
                Assert.AreEqual(SessionPersistenceType.SourceAddress, sessionPersistence.PersistenceType);

                // should fail to set to HTTP cookie
                try
                {
                    try
                    {
                        await provider.SetSessionPersistenceAsync(tempLoadBalancer.Id, new SessionPersistence(SessionPersistenceType.HttpCookie), AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                        Assert.Fail("Expected a WebException");
                    }
                    catch (WebException ex)
                    {
                        throw new AggregateException(ex);
                    }
                }
                catch (AggregateException aggregate)
                {
                    aggregate.Flatten().Handle(
                        ex =>
                        {
                            WebException webException = ex as WebException;
                            if (webException == null)
                                return false;

                            HttpWebResponse response = webException.Response as HttpWebResponse;
                            if (response == null)
                                return false;

                            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
                            return true;
                        });
                }

                sessionPersistence = await provider.GetSessionPersistenceAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(sessionPersistence);
                Assert.AreEqual(SessionPersistenceType.SourceAddress, sessionPersistence.PersistenceType);

                // set to none
                await provider.RemoveSessionPersistenceAsync(tempLoadBalancer.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                sessionPersistence = await provider.GetSessionPersistenceAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(sessionPersistence);
                Assert.IsNull(sessionPersistence.PersistenceType);

                try
                {
                    try
                    {
                        // set to none again
                        await provider.RemoveSessionPersistenceAsync(tempLoadBalancer.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                        Assert.Fail("Expected a WebException");
                    }
                    catch (WebException ex)
                    {
                        throw new AggregateException(ex);
                    }
                }
                catch (AggregateException aggregate)
                {
                    aggregate.Flatten().Handle(
                        ex =>
                        {
                            WebException webException = ex as WebException;
                            if (webException == null)
                                return false;

                            HttpWebResponse response = webException.Response as HttpWebResponse;
                            if (response == null)
                                return false;

                            Assert.AreEqual((HttpStatusCode)422, response.StatusCode);
                            return true;
                        });
                }

                sessionPersistence = await provider.GetSessionPersistenceAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(sessionPersistence);
                Assert.IsNull(sessionPersistence.PersistenceType);

                /* Cleanup
                 */

                await provider.RemoveLoadBalancerAsync(tempLoadBalancer.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public async Task TestSetConnectionLogging()
        {
            ILoadBalancerService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(60))))
            {
                IEnumerable<LoadBalancingProtocol> protocols = await provider.ListProtocolsAsync(cancellationTokenSource.Token);
                LoadBalancingProtocol httpProtocol = protocols.First(i => i.Name.Equals("HTTP", StringComparison.OrdinalIgnoreCase));

                string loadBalancerName = CreateRandomLoadBalancerName();

                LoadBalancerConfiguration configuration = new LoadBalancerConfiguration(
                    name: loadBalancerName,
                    nodes: null,
                    protocol: httpProtocol,
                    virtualAddresses: new[] { new LoadBalancerVirtualAddress(LoadBalancerVirtualAddressType.ServiceNet) },
                    halfClosed: null,
                    accessList: null,
                    algorithm: LoadBalancingAlgorithm.RoundRobin,
                    connectionLogging: false,
                    contentCaching: null,
                    connectionThrottle: null,
                    healthMonitor: null,
                    metadata: null,
                    timeout: null,
                    sessionPersistence: null);
                LoadBalancer tempLoadBalancer = await provider.CreateLoadBalancerAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                // verify initially false
                bool connectionLogging = await provider.GetConnectionLoggingAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsFalse(connectionLogging);

                // set to true
                await provider.SetConnectionLoggingAsync(tempLoadBalancer.Id, true, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                connectionLogging = await provider.GetConnectionLoggingAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsTrue(connectionLogging);

                // set to true again
                await provider.SetConnectionLoggingAsync(tempLoadBalancer.Id, true, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                connectionLogging = await provider.GetConnectionLoggingAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsTrue(connectionLogging);

                // set to false
                await provider.SetConnectionLoggingAsync(tempLoadBalancer.Id, false, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                connectionLogging = await provider.GetConnectionLoggingAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsFalse(connectionLogging);

                // set to false again
                await provider.SetConnectionLoggingAsync(tempLoadBalancer.Id, false, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                connectionLogging = await provider.GetConnectionLoggingAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsFalse(connectionLogging);

                /* Cleanup
                 */

                await provider.RemoveLoadBalancerAsync(tempLoadBalancer.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public async Task TestConnectionThrottleInitialization()
        {
            ILoadBalancerService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(60))))
            {
                IEnumerable<LoadBalancingProtocol> protocols = await provider.ListProtocolsAsync(cancellationTokenSource.Token);
                LoadBalancingProtocol httpProtocol = protocols.First(i => i.Name.Equals("HTTP", StringComparison.OrdinalIgnoreCase));

                string loadBalancerName = CreateRandomLoadBalancerName();

                int maxConnectionRate = 3;
                int maxConnections = 2;
                int minConnections = 1;
                TimeSpan rateInterval = TimeSpan.FromSeconds(5);
                ConnectionThrottles throttles = new ConnectionThrottles(maxConnectionRate, maxConnections, minConnections, rateInterval);

                LoadBalancerConfiguration configuration = new LoadBalancerConfiguration(
                    name: loadBalancerName,
                    nodes: null,
                    protocol: httpProtocol,
                    virtualAddresses: new[] { new LoadBalancerVirtualAddress(LoadBalancerVirtualAddressType.ServiceNet) },
                    halfClosed: null,
                    accessList: null,
                    algorithm: LoadBalancingAlgorithm.RoundRobin,
                    connectionLogging: null,
                    contentCaching: null,
                    connectionThrottle: throttles,
                    healthMonitor: null,
                    metadata: null,
                    timeout: null,
                    sessionPersistence: null);
                LoadBalancer tempLoadBalancer = await provider.CreateLoadBalancerAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                Assert.AreEqual(LoadBalancerStatus.Active, tempLoadBalancer.Status);
                Assert.IsNotNull(tempLoadBalancer.ConnectionThrottles);

                throttles = tempLoadBalancer.ConnectionThrottles;
                Assert.AreEqual(maxConnectionRate, throttles.MaxConnectionRate);
                Assert.AreEqual(maxConnections, throttles.MaxConnections);
                Assert.AreEqual(minConnections, throttles.MinConnections);
                Assert.AreEqual(rateInterval, throttles.RateInterval);

                throttles = await provider.ListThrottlesAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.AreEqual(maxConnectionRate, throttles.MaxConnectionRate);
                Assert.AreEqual(maxConnections, throttles.MaxConnections);
                Assert.AreEqual(minConnections, throttles.MinConnections);
                Assert.AreEqual(rateInterval, throttles.RateInterval);

                /* Cleanup
                 */

                await provider.RemoveLoadBalancerAsync(tempLoadBalancer.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public async Task TestConnectionThrottleConfiguration()
        {
            ILoadBalancerService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(60))))
            {
                IEnumerable<LoadBalancingProtocol> protocols = await provider.ListProtocolsAsync(cancellationTokenSource.Token);
                LoadBalancingProtocol httpProtocol = protocols.First(i => i.Name.Equals("HTTP", StringComparison.OrdinalIgnoreCase));

                string loadBalancerName = CreateRandomLoadBalancerName();

                LoadBalancerConfiguration configuration = new LoadBalancerConfiguration(
                    name: loadBalancerName,
                    nodes: null,
                    protocol: httpProtocol,
                    virtualAddresses: new[] { new LoadBalancerVirtualAddress(LoadBalancerVirtualAddressType.ServiceNet) },
                    halfClosed: null,
                    accessList: null,
                    algorithm: LoadBalancingAlgorithm.RoundRobin,
                    connectionLogging: null,
                    contentCaching: null,
                    connectionThrottle: null,
                    healthMonitor: null,
                    metadata: null,
                    timeout: null,
                    sessionPersistence: null);
                LoadBalancer tempLoadBalancer = await provider.CreateLoadBalancerAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                Assert.AreEqual(LoadBalancerStatus.Active, tempLoadBalancer.Status);
                Assert.IsNull(tempLoadBalancer.ConnectionThrottles);
                Console.WriteLine("Reported by create:");
                Console.WriteLine(JsonConvert.SerializeObject(tempLoadBalancer.ConnectionThrottles, Formatting.Indented));

                ConnectionThrottles throttles = await provider.ListThrottlesAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Console.WriteLine("After create:");
                Console.WriteLine(JsonConvert.SerializeObject(throttles, Formatting.Indented));

                int maxConnectionRate = 3;
                int maxConnections = 2;
                int minConnections = 1;
                TimeSpan rateInterval = TimeSpan.FromSeconds(5);
                throttles = new ConnectionThrottles(maxConnectionRate, maxConnections, minConnections, rateInterval);
                await provider.UpdateThrottlesAsync(tempLoadBalancer.Id, throttles, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                throttles = await provider.ListThrottlesAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.AreEqual(maxConnectionRate, throttles.MaxConnectionRate);
                Assert.AreEqual(maxConnections, throttles.MaxConnections);
                Assert.AreEqual(minConnections, throttles.MinConnections);
                Assert.AreEqual(rateInterval, throttles.RateInterval);
                Console.WriteLine("After update:");
                Console.WriteLine(JsonConvert.SerializeObject(throttles, Formatting.Indented));

                await provider.RemoveThrottlesAsync(tempLoadBalancer.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                throttles = await provider.ListThrottlesAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Console.WriteLine("After removal:");
                Console.WriteLine(JsonConvert.SerializeObject(throttles, Formatting.Indented));

                /* Cleanup
                 */

                await provider.RemoveLoadBalancerAsync(tempLoadBalancer.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public async Task TestSetContentCaching()
        {
            ILoadBalancerService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(60))))
            {
                IEnumerable<LoadBalancingProtocol> protocols = await provider.ListProtocolsAsync(cancellationTokenSource.Token);
                LoadBalancingProtocol httpProtocol = protocols.First(i => i.Name.Equals("HTTP", StringComparison.OrdinalIgnoreCase));

                string loadBalancerName = CreateRandomLoadBalancerName();

                LoadBalancerConfiguration configuration = new LoadBalancerConfiguration(
                    name: loadBalancerName,
                    nodes: null,
                    protocol: httpProtocol,
                    virtualAddresses: new[] { new LoadBalancerVirtualAddress(LoadBalancerVirtualAddressType.ServiceNet) },
                    halfClosed: null,
                    accessList: null,
                    algorithm: LoadBalancingAlgorithm.RoundRobin,
                    connectionLogging: null,
                    contentCaching: false,
                    connectionThrottle: null,
                    healthMonitor: null,
                    metadata: null,
                    timeout: null,
                    sessionPersistence: null);
                LoadBalancer tempLoadBalancer = await provider.CreateLoadBalancerAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                // verify initially false
                bool contentCaching = await provider.GetContentCachingAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsFalse(contentCaching);

                // set to true
                await provider.SetContentCachingAsync(tempLoadBalancer.Id, true, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                contentCaching = await provider.GetContentCachingAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsTrue(contentCaching);

                // set to true again
                await provider.SetContentCachingAsync(tempLoadBalancer.Id, true, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                contentCaching = await provider.GetContentCachingAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsTrue(contentCaching);

                // set to false
                await provider.SetContentCachingAsync(tempLoadBalancer.Id, false, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                contentCaching = await provider.GetContentCachingAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsFalse(contentCaching);

                // set to false again
                await provider.SetContentCachingAsync(tempLoadBalancer.Id, false, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                contentCaching = await provider.GetContentCachingAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsFalse(contentCaching);

                /* Cleanup
                 */

                await provider.RemoveLoadBalancerAsync(tempLoadBalancer.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public async Task TestListProtocols()
        {
            ILoadBalancerService provider = CreateProvider();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(60)));

            IEnumerable<LoadBalancingProtocol> protocols = await provider.ListProtocolsAsync(cancellationTokenSource.Token);
            if (!protocols.Any())
                Assert.Inconclusive("No load balancer protocols were returned by the server.");

            foreach (LoadBalancingProtocol protocol in protocols)
                Console.WriteLine("{0} ({1})", protocol.Name, protocol.Port);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public async Task TestListAlgorithms()
        {
            ILoadBalancerService provider = CreateProvider();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(60)));

            IEnumerable<LoadBalancingAlgorithm> algorithms = await provider.ListAlgorithmsAsync(cancellationTokenSource.Token);
            if (!algorithms.Any())
                Assert.Inconclusive("No load balancer algorithms were returned by the server.");

            foreach (LoadBalancingAlgorithm algorithm in algorithms)
                Console.WriteLine(algorithm.Name);

            Assert.IsTrue(algorithms.Contains(LoadBalancingAlgorithm.LeastConnections));
            Assert.IsTrue(algorithms.Contains(LoadBalancingAlgorithm.Random));
            Assert.IsTrue(algorithms.Contains(LoadBalancingAlgorithm.RoundRobin));
            Assert.IsTrue(algorithms.Contains(LoadBalancingAlgorithm.WeightedLeastConnections));
            Assert.IsTrue(algorithms.Contains(LoadBalancingAlgorithm.WeightedRoundRobin));
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public async Task TestSslTermination()
        {
            ILoadBalancerService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(240))))
            {
                IEnumerable<LoadBalancingProtocol> protocols = await provider.ListProtocolsAsync(cancellationTokenSource.Token);
                LoadBalancingProtocol httpProtocol = protocols.First(i => i.Name.Equals("HTTP", StringComparison.OrdinalIgnoreCase));

                string loadBalancerName = CreateRandomLoadBalancerName();

                LoadBalancerConfiguration configuration = new LoadBalancerConfiguration(
                    name: loadBalancerName,
                    nodes: null,
                    protocol: httpProtocol,
                    virtualAddresses: new[] { new LoadBalancerVirtualAddress(LoadBalancerVirtualAddressType.ServiceNet) },
                    algorithm: LoadBalancingAlgorithm.RoundRobin);
                LoadBalancer tempLoadBalancer = await provider.CreateLoadBalancerAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                string privateKey;
                string certificate;

                CngKeyCreationParameters keyParams = new CngKeyCreationParameters();
                keyParams.ExportPolicy = CngExportPolicies.AllowExport | CngExportPolicies.AllowPlaintextExport;
                keyParams.KeyUsage = CngKeyUsages.AllUsages;
                keyParams.Provider = CngProvider.MicrosoftSoftwareKeyStorageProvider;
                using (CngKey key = CngKey.Create(CngAlgorithm2.Rsa, Guid.NewGuid().ToString(), keyParams))
                {
                    byte[] exported = key.Export(CngKeyBlobFormat.Pkcs8PrivateBlob);

                    StringBuilder formatted = new StringBuilder();
                    formatted.AppendLine("-----BEGIN RSA PRIVATE KEY-----");
                    formatted.AppendLine(Convert.ToBase64String(exported, Base64FormattingOptions.InsertLineBreaks));
                    formatted.Append("-----END RSA PRIVATE KEY-----");
                    Console.WriteLine(formatted.ToString());
                    privateKey = formatted.ToString();

                    X509CertificateCreationParameters certParams = new X509CertificateCreationParameters(new X500DistinguishedName(string.Format("CN={0}, OU=Integration Testing, O=openstacknetsdk, L=San Antonio, S=Texas, C=US", loadBalancerName)));
                    certParams.SignatureAlgorithm = X509CertificateSignatureAlgorithm.RsaSha1;
                    certParams.StartTime = DateTime.Now;
                    certParams.EndTime = DateTime.Now.AddYears(10);
                    certParams.TakeOwnershipOfKey = true;

                    X509Certificate signed = key.CreateSelfSignedCertificate(certParams);
                    exported = signed.Export(X509ContentType.Cert);

                    formatted = new StringBuilder();
                    formatted.AppendLine("-----BEGIN CERTIFICATE-----");
                    formatted.AppendLine(Convert.ToBase64String(exported, Base64FormattingOptions.InsertLineBreaks));
                    formatted.Append("-----END CERTIFICATE-----");
                    Console.WriteLine(formatted.ToString());
                    certificate = formatted.ToString();
                }

                string intermediateCertificate = null;
                LoadBalancerSslConfiguration sslConfiguration = new LoadBalancerSslConfiguration(true, false, 443, privateKey, certificate, intermediateCertificate);

                await provider.UpdateSslConfigurationAsync(tempLoadBalancer.Id, sslConfiguration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                LoadBalancerSslConfiguration updatedConfiguration = new LoadBalancerSslConfiguration(true, true, 443);
                await provider.UpdateSslConfigurationAsync(tempLoadBalancer.Id, updatedConfiguration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                await provider.RemoveSslConfigurationAsync(tempLoadBalancer.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                /* Cleanup
                 */

                await provider.RemoveLoadBalancerAsync(tempLoadBalancer.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public async Task TestLoadBalancerMetadata()
        {
            ILoadBalancerService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(60))))
            {
                IEnumerable<LoadBalancingProtocol> protocols = await provider.ListProtocolsAsync(cancellationTokenSource.Token);
                LoadBalancingProtocol httpProtocol = protocols.First(i => i.Name.Equals("HTTP", StringComparison.OrdinalIgnoreCase));

                string loadBalancerName = CreateRandomLoadBalancerName();

                string expectedKey = "key1";
                string expectedValue = "value!!";
                LoadBalancerConfiguration configuration = new LoadBalancerConfiguration(
                    name: loadBalancerName,
                    nodes: null,
                    protocol: httpProtocol,
                    virtualAddresses: new[] { new LoadBalancerVirtualAddress(LoadBalancerVirtualAddressType.ServiceNet) },
                    algorithm: LoadBalancingAlgorithm.RoundRobin,
                    metadata: new[] { new LoadBalancerMetadataItem(expectedKey, expectedValue) });
                LoadBalancer tempLoadBalancer = await provider.CreateLoadBalancerAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                Assert.AreEqual(LoadBalancerStatus.Active, tempLoadBalancer.Status);

                IEnumerable<LoadBalancerMetadataItem> metadata = await provider.ListLoadBalancerMetadataAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(metadata);

                LoadBalancerMetadataItem expectedItem = null;
                foreach (LoadBalancerMetadataItem item in metadata)
                {
                    Console.WriteLine("  {0}: {1} = {2}", item.Id, item.Key, item.Value);
                    LoadBalancerMetadataItem singleItem = await provider.GetLoadBalancerMetadataItemAsync(tempLoadBalancer.Id, item.Id, cancellationTokenSource.Token);
                    Assert.AreEqual(item.Id, singleItem.Id);
                    Assert.AreEqual(item.Key, singleItem.Key);
                    Assert.AreEqual(item.Value, singleItem.Value);

                    if (item.Key == expectedKey)
                        expectedItem = item;
                }

                Assert.IsNotNull(expectedItem);
                Assert.AreEqual(expectedKey, expectedItem.Key);
                Assert.AreEqual(expectedValue, expectedItem.Value);

                string updatedValue = "My new value.";
                await provider.UpdateLoadBalancerMetadataItemAsync(tempLoadBalancer.Id, expectedItem.Id, updatedValue, cancellationTokenSource.Token);
                // verify that the update command did not place the load balancer in the "PendingUpdate" state
                Assert.AreEqual(LoadBalancerStatus.Active, (await provider.GetLoadBalancerAsync(tempLoadBalancer.Id, cancellationTokenSource.Token)).Status);
                expectedItem = await provider.GetLoadBalancerMetadataItemAsync(tempLoadBalancer.Id, expectedItem.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(expectedItem);
                Assert.AreEqual(expectedKey, expectedItem.Key);
                Assert.AreEqual(updatedValue, expectedItem.Value);

                string expectedSecondKey = "key2";
                Dictionary<string, string> updatedMetadata = new Dictionary<string, string> { { expectedSecondKey, expectedValue } };
                IEnumerable<LoadBalancerMetadataItem> updateMetadataItems = await provider.AddLoadBalancerMetadataAsync(tempLoadBalancer.Id, updatedMetadata, cancellationTokenSource.Token);
                LoadBalancerMetadataItem secondItem = updateMetadataItems.SingleOrDefault();
                Assert.IsNotNull(secondItem);
                Assert.AreEqual(expectedSecondKey, secondItem.Key);
                Assert.AreEqual(expectedValue, secondItem.Value);

                await provider.RemoveLoadBalancerMetadataItemAsync(tempLoadBalancer.Id, new[] { expectedItem.Id, secondItem.Id }, cancellationTokenSource.Token);
                metadata = await provider.ListLoadBalancerMetadataAsync(tempLoadBalancer.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(metadata);
                Assert.IsFalse(metadata.Any());

                /* Cleanup
                 */

                await provider.RemoveLoadBalancerAsync(tempLoadBalancer.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.LoadBalancers)]
        public async Task TestNodeMetadata()
        {
            ILoadBalancerService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(60))))
            {
                IPHostEntry entry = await Task.Factory.FromAsync<IPHostEntry>(Dns.BeginGetHostEntry("docs.rackspace.com", null, null), Dns.EndGetHostEntry);

                IEnumerable<LoadBalancingProtocol> protocols = await provider.ListProtocolsAsync(cancellationTokenSource.Token);
                LoadBalancingProtocol httpProtocol = protocols.First(i => i.Name.Equals("HTTP", StringComparison.OrdinalIgnoreCase));

                string loadBalancerName = CreateRandomLoadBalancerName();

                string expectedKey = "key1";
                string expectedValue = "value!!";
                LoadBalancerConfiguration configuration = new LoadBalancerConfiguration(
                    name: loadBalancerName,
                    nodes: new[] { new NodeConfiguration(entry.AddressList[0], 80, NodeCondition.Enabled, NodeType.Primary, null) },
                    protocol: httpProtocol,
                    virtualAddresses: new[] { new LoadBalancerVirtualAddress(LoadBalancerVirtualAddressType.ServiceNet) },
                    algorithm: LoadBalancingAlgorithm.RoundRobin);
                LoadBalancer tempLoadBalancer = await provider.CreateLoadBalancerAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                Node node = tempLoadBalancer.Nodes.Single();
                Assert.AreEqual(LoadBalancerStatus.Active, tempLoadBalancer.Status);

                // can't set node metadata during the initial creation, so we set it right afterwards
                await provider.AddNodeMetadataAsync(tempLoadBalancer.Id, node.Id, new Dictionary<string, string> { { expectedKey, expectedValue } }, cancellationTokenSource.Token);

                IEnumerable<LoadBalancerMetadataItem> metadata = await provider.ListNodeMetadataAsync(tempLoadBalancer.Id, node.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(metadata);

                LoadBalancerMetadataItem expectedItem = null;
                foreach (LoadBalancerMetadataItem item in metadata)
                {
                    Console.WriteLine("  {0}: {1} = {2}", item.Id, item.Key, item.Value);
                    LoadBalancerMetadataItem singleItem = await provider.GetNodeMetadataItemAsync(tempLoadBalancer.Id, node.Id, item.Id, cancellationTokenSource.Token);
                    Assert.AreEqual(item.Id, singleItem.Id);
                    Assert.AreEqual(item.Key, singleItem.Key);
                    Assert.AreEqual(item.Value, singleItem.Value);

                    if (item.Key == expectedKey)
                        expectedItem = item;
                }

                Assert.IsNotNull(expectedItem);
                Assert.AreEqual(expectedKey, expectedItem.Key);
                Assert.AreEqual(expectedValue, expectedItem.Value);

                string updatedValue = "My new value.";
                await provider.UpdateNodeMetadataItemAsync(tempLoadBalancer.Id, node.Id, expectedItem.Id, updatedValue, cancellationTokenSource.Token);
                // verify that the update command did not place the load balancer in the "PendingUpdate" state
                Assert.AreEqual(NodeStatus.Online, (await provider.GetNodeAsync(tempLoadBalancer.Id, node.Id, cancellationTokenSource.Token)).Status);
                expectedItem = await provider.GetNodeMetadataItemAsync(tempLoadBalancer.Id, node.Id, expectedItem.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(expectedItem);
                Assert.AreEqual(expectedKey, expectedItem.Key);
                Assert.AreEqual(updatedValue, expectedItem.Value);

                string expectedSecondKey = "key2";
                Dictionary<string, string> updatedMetadata = new Dictionary<string, string> { { expectedSecondKey, expectedValue } };
                IEnumerable<LoadBalancerMetadataItem> updateMetadataItems = await provider.AddNodeMetadataAsync(tempLoadBalancer.Id, node.Id, updatedMetadata, cancellationTokenSource.Token);
                LoadBalancerMetadataItem secondItem = updateMetadataItems.SingleOrDefault();
                Assert.IsNotNull(secondItem);
                Assert.AreEqual(expectedSecondKey, secondItem.Key);
                Assert.AreEqual(expectedValue, secondItem.Value);

                await provider.RemoveNodeMetadataItemAsync(tempLoadBalancer.Id, node.Id, new[] { expectedItem.Id, secondItem.Id }, cancellationTokenSource.Token);
                metadata = await provider.ListNodeMetadataAsync(tempLoadBalancer.Id, node.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(metadata);
                Assert.IsFalse(metadata.Any());

                /* Cleanup
                 */

                await provider.RemoveLoadBalancerAsync(tempLoadBalancer.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
            }
        }

        /// <summary>
        /// Gets all existing load balancers through a series of asynchronous operations,
        /// each of which requests a subset of the available load balancers.
        /// </summary>
        /// <param name="provider">The load balancer service.</param>
        /// <param name="limit">The maximum number of <see cref="LoadBalancer"/> objects to return from a single task. If this value is <c>null</c>, a provider-specific default is used.</param>
        /// <returns>
        /// A collection of <see cref="LoadBalancer"/> objects describing the available load
        /// balancers.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="provider"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="limit"/> is less than or equal to 0.</exception>
        private static IEnumerable<LoadBalancer> ListAllLoadBalancers(ILoadBalancerService provider, int? limit, CancellationToken cancellationToken)
        {
            if (limit <= 0)
                throw new ArgumentOutOfRangeException("limit");

            LoadBalancer lastLoadBalancer = null;

            do
            {
                LoadBalancerId marker = lastLoadBalancer != null ? lastLoadBalancer.Id : null;
                IEnumerable<LoadBalancer> loadBalancers = provider.ListLoadBalancersAsync(marker, limit, cancellationToken).Result;
                lastLoadBalancer = null;
                foreach (LoadBalancer loadBalancer in loadBalancers)
                {
                    yield return loadBalancer;
                    lastLoadBalancer = loadBalancer;
                }

                // pagination for this call does not match the documentation.
            } while (false && lastLoadBalancer != null);
        }

        private static IEnumerable<NodeServiceEvent> ListAllNodeServiceEvents(ILoadBalancerService provider, LoadBalancerId loadBalancerId, int? limit, CancellationToken cancellationToken)
        {
            if (limit <= 0)
                throw new ArgumentOutOfRangeException("limit");

            NodeServiceEvent lastServiceEvent = null;

            do
            {
                NodeServiceEventId marker = lastServiceEvent != null ? lastServiceEvent.Id : null;
                IEnumerable<NodeServiceEvent> serviceEvents = provider.ListNodeServiceEventsAsync(loadBalancerId, marker, limit, cancellationToken).Result;
                lastServiceEvent = null;
                foreach (NodeServiceEvent serviceEvent in serviceEvents)
                {
                    yield return serviceEvent;
                    lastServiceEvent = serviceEvent;
                }
            } while (lastServiceEvent != null);
        }

        /// <summary>
        /// Gets all existing load balancer nodes through a series of asynchronous operations,
        /// each of which requests a subset of the available nodes.
        /// </summary>
        /// <param name="provider">The load balancer service.</param>
        /// <param name="limit">The maximum number of <see cref="Node"/> to return from a single task. If this value is <c>null</c>, a provider-specific default is used.</param>
        /// <returns>
        /// A collection of <see cref="Node"/> objects, each of which represents a subset
        /// of the available load balancer nodes.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="provider"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="loadBalancerId"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">If <paramref name="loadBalancerId"/> is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="limit"/> is less than or equal to 0.</exception>
        private static IEnumerable<Node> ListAllLoadBalancerNodes(ILoadBalancerService provider, LoadBalancerId loadBalancerId, int? limit, CancellationToken cancellationToken)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (limit <= 0)
                throw new ArgumentOutOfRangeException("limit");

            // this API call is not currently paginated
            IEnumerable<Node> loadBalancers = provider.ListNodesAsync(loadBalancerId, cancellationToken).Result;
            return loadBalancers;
        }

        private TimeSpan TestTimeout(TimeSpan timeout)
        {
            if (Debugger.IsAttached)
            {
                Console.WriteLine("Using extended timeout due to attached debugger.");
                return TimeSpan.FromDays(1);
            }

            return timeout;
        }

        /// <summary>
        /// Creates a random load balancer name with the proper prefix for integration testing.
        /// </summary>
        /// <returns>A unique, randomly-generated load balancer name.</returns>
        private string CreateRandomLoadBalancerName()
        {
            return TestLoadBalancerPrefix + Path.GetRandomFileName();
        }

        /// <summary>
        /// Creates an instance of <see cref="ILoadBalancerService"/> for testing using
        /// the <see cref="OpenstackNetSetings.TestIdentity"/>.
        /// </summary>
        /// <returns>An instance of <see cref="ILoadBalancerService"/> for integration testing.</returns>
        private ILoadBalancerService CreateProvider()
        {
            var provider = new CloudLoadBalancerProvider(Bootstrapper.Settings.TestIdentity, null, null, null);
            provider.BeforeAsyncWebRequest +=
                (sender, e) =>
                {
                    Console.WriteLine("{0} (Request) {1} {2}", DateTime.Now, e.Request.Method, e.Request.RequestUri);
                };
            provider.AfterAsyncWebResponse +=
                (sender, e) =>
                {
                    Console.WriteLine("{0} (Result {1}) {2}", DateTime.Now, e.Response.StatusCode, e.Response.ResponseUri);
                };

            provider.ConnectionLimit = 3;

            return provider;
        }
    }
}
#if false
Test Name:	TestSslTermination
Test Outcome:	Failed
Result Message:	
Test method Net.OpenStack.Testing.Integration.Providers.Rackspace.UserLoadBalancerTests.TestSslTermination threw exception: 
System.AggregateException: One or more errors occurred. ---> System.AggregateException: One or more errors occurred. ---> System.Net.WebException: {"message":"Unable to parse pemblock to RSA Key","code":400} ---> System.AggregateException: One or more errors occurred. ---> System.Net.WebException: The remote server returned an error: (400) Bad Request.
Result StandardOutput:	
10/28/2013 14:17:05 (Request) GET https://ord.loadbalancers.api.rackspacecloud.com/v1.0/841614/loadbalancers/protocols
10/28/2013 14:17:05 (Result OK) https://ord.loadbalancers.api.rackspacecloud.com/v1.0/841614/loadbalancers/protocols
{
  "protocols": [
    {
      "name": "DNS_TCP",
      "port": 53
    },
    {
      "name": "DNS_UDP",
      "port": 53
    },
    {
      "name": "FTP",
      "port": 21
    },
    {
      "name": "HTTP",
      "port": 80
    },
    {
      "name": "HTTPS",
      "port": 443
    },
    {
      "name": "IMAPS",
      "port": 993
    },
    {
      "name": "IMAPv2",
      "port": 143
    },
    {
      "name": "IMAPv3",
      "port": 220
    },
    {
      "name": "IMAPv4",
      "port": 143
    },
    {
      "name": "LDAP",
      "port": 389
    },
    {
      "name": "LDAPS",
      "port": 636
    },
    {
      "name": "MYSQL",
      "port": 3306
    },
    {
      "name": "POP3",
      "port": 110
    },
    {
      "name": "POP3S",
      "port": 995
    },
    {
      "name": "SFTP",
      "port": 22
    },
    {
      "name": "SMTP",
      "port": 25
    },
    {
      "name": "TCP",
      "port": 0
    },
    {
      "name": "TCP_CLIENT_FIRST",
      "port": 0
    },
    {
      "name": "UDP",
      "port": 0
    },
    {
      "name": "UDP_STREAM",
      "port": 0
    }
  ]
}
{
  "loadBalancer": {
    "name": "UnitTestLB-fj3r0koj.euf",
    "protocol": "HTTP",
    "virtualIps": [
      {
        "type": "SERVICENET"
      }
    ],
    "algorithm": "ROUND_ROBIN",
    "port": 80
  }
}
10/28/2013 14:17:06 (Request) POST https://ord.loadbalancers.api.rackspacecloud.com/v1.0/841614/loadbalancers
10/28/2013 14:17:06 (Result Accepted) https://ord.loadbalancers.api.rackspacecloud.com/v1.0/841614/loadbalancers
{
  "loadBalancer": {
    "name": "UnitTestLB-fj3r0koj.euf",
    "id": 204627,
    "protocol": "HTTP",
    "port": 80,
    "algorithm": "ROUND_ROBIN",
    "status": "BUILD",
    "cluster": {
      "name": "ztm-n19.ord1.lbaas.rackspace.net"
    },
    "timeout": 30,
    "created": {
      "time": "2013-10-28T19:17:07Z"
    },
    "virtualIps": [
      {
        "address": "10.189.246.60",
        "id": 13703,
        "type": "SERVICENET",
        "ipVersion": "IPV4"
      }
    ],
    "sourceAddresses": {
      "ipv6Public": "2001:4801:7901::19/64",
      "ipv4Servicenet": "10.189.246.6",
      "ipv4Public": "192.237.224.6"
    },
    "updated": {
      "time": "2013-10-28T19:17:07Z"
    },
    "halfClosed": false,
    "connectionLogging": {
      "enabled": false
    },
    "contentCaching": {
      "enabled": false
    }
  }
}
10/28/2013 14:17:06 (Request) GET https://ord.loadbalancers.api.rackspacecloud.com/v1.0/841614/loadbalancers/204627
10/28/2013 14:17:06 (Result OK) https://ord.loadbalancers.api.rackspacecloud.com/v1.0/841614/loadbalancers/204627
{
  "loadBalancer": {
    "name": "UnitTestLB-fj3r0koj.euf",
    "id": 204627,
    "protocol": "HTTP",
    "port": 80,
    "algorithm": "ROUND_ROBIN",
    "status": "BUILD",
    "cluster": {
      "name": "ztm-n19.ord1.lbaas.rackspace.net"
    },
    "timeout": 30,
    "created": {
      "time": "2013-10-28T19:17:07Z"
    },
    "virtualIps": [
      {
        "address": "10.189.246.60",
        "id": 13703,
        "type": "SERVICENET",
        "ipVersion": "IPV4"
      }
    ],
    "sourceAddresses": {
      "ipv6Public": "2001:4801:7901::19/64",
      "ipv4Servicenet": "10.189.246.6",
      "ipv4Public": "192.237.224.6"
    },
    "updated": {
      "time": "2013-10-28T19:17:07Z"
    },
    "halfClosed": false,
    "connectionLogging": {
      "enabled": false
    },
    "contentCaching": {
      "enabled": false
    }
  }
}
10/28/2013 14:17:07 (Request) GET https://ord.loadbalancers.api.rackspacecloud.com/v1.0/841614/loadbalancers/204627
10/28/2013 14:17:08 (Result OK) https://ord.loadbalancers.api.rackspacecloud.com/v1.0/841614/loadbalancers/204627
{
  "loadBalancer": {
    "name": "UnitTestLB-fj3r0koj.euf",
    "id": 204627,
    "protocol": "HTTP",
    "port": 80,
    "algorithm": "ROUND_ROBIN",
    "status": "BUILD",
    "cluster": {
      "name": "ztm-n19.ord1.lbaas.rackspace.net"
    },
    "timeout": 30,
    "created": {
      "time": "2013-10-28T19:17:07Z"
    },
    "virtualIps": [
      {
        "address": "10.189.246.60",
        "id": 13703,
        "type": "SERVICENET",
        "ipVersion": "IPV4"
      }
    ],
    "sourceAddresses": {
      "ipv6Public": "2001:4801:7901::19/64",
      "ipv4Servicenet": "10.189.246.6",
      "ipv4Public": "192.237.224.6"
    },
    "updated": {
      "time": "2013-10-28T19:17:07Z"
    },
    "halfClosed": false,
    "connectionLogging": {
      "enabled": false
    },
    "contentCaching": {
      "enabled": false
    }
  }
}
10/28/2013 14:17:09 (Request) GET https://ord.loadbalancers.api.rackspacecloud.com/v1.0/841614/loadbalancers/204627
10/28/2013 14:17:09 (Result OK) https://ord.loadbalancers.api.rackspacecloud.com/v1.0/841614/loadbalancers/204627
{
  "loadBalancer": {
    "name": "UnitTestLB-fj3r0koj.euf",
    "id": 204627,
    "protocol": "HTTP",
    "port": 80,
    "algorithm": "ROUND_ROBIN",
    "status": "BUILD",
    "cluster": {
      "name": "ztm-n19.ord1.lbaas.rackspace.net"
    },
    "timeout": 30,
    "created": {
      "time": "2013-10-28T19:17:07Z"
    },
    "virtualIps": [
      {
        "address": "10.189.246.60",
        "id": 13703,
        "type": "SERVICENET",
        "ipVersion": "IPV4"
      }
    ],
    "sourceAddresses": {
      "ipv6Public": "2001:4801:7901::19/64",
      "ipv4Servicenet": "10.189.246.6",
      "ipv4Public": "192.237.224.6"
    },
    "updated": {
      "time": "2013-10-28T19:17:07Z"
    },
    "halfClosed": false,
    "connectionLogging": {
      "enabled": false
    },
    "contentCaching": {
      "enabled": false
    }
  }
}
10/28/2013 14:17:10 (Request) GET https://ord.loadbalancers.api.rackspacecloud.com/v1.0/841614/loadbalancers/204627
10/28/2013 14:17:10 (Result OK) https://ord.loadbalancers.api.rackspacecloud.com/v1.0/841614/loadbalancers/204627
{
  "loadBalancer": {
    "name": "UnitTestLB-fj3r0koj.euf",
    "id": 204627,
    "protocol": "HTTP",
    "port": 80,
    "algorithm": "ROUND_ROBIN",
    "status": "BUILD",
    "cluster": {
      "name": "ztm-n19.ord1.lbaas.rackspace.net"
    },
    "timeout": 30,
    "created": {
      "time": "2013-10-28T19:17:07Z"
    },
    "virtualIps": [
      {
        "address": "10.189.246.60",
        "id": 13703,
        "type": "SERVICENET",
        "ipVersion": "IPV4"
      }
    ],
    "sourceAddresses": {
      "ipv6Public": "2001:4801:7901::19/64",
      "ipv4Servicenet": "10.189.246.6",
      "ipv4Public": "192.237.224.6"
    },
    "updated": {
      "time": "2013-10-28T19:17:07Z"
    },
    "halfClosed": false,
    "connectionLogging": {
      "enabled": false
    },
    "contentCaching": {
      "enabled": false
    }
  }
}
10/28/2013 14:17:11 (Request) GET https://ord.loadbalancers.api.rackspacecloud.com/v1.0/841614/loadbalancers/204627
10/28/2013 14:17:11 (Result OK) https://ord.loadbalancers.api.rackspacecloud.com/v1.0/841614/loadbalancers/204627
{
  "loadBalancer": {
    "name": "UnitTestLB-fj3r0koj.euf",
    "id": 204627,
    "protocol": "HTTP",
    "port": 80,
    "algorithm": "ROUND_ROBIN",
    "status": "BUILD",
    "cluster": {
      "name": "ztm-n19.ord1.lbaas.rackspace.net"
    },
    "timeout": 30,
    "created": {
      "time": "2013-10-28T19:17:07Z"
    },
    "virtualIps": [
      {
        "address": "10.189.246.60",
        "id": 13703,
        "type": "SERVICENET",
        "ipVersion": "IPV4"
      }
    ],
    "sourceAddresses": {
      "ipv6Public": "2001:4801:7901::19/64",
      "ipv4Servicenet": "10.189.246.6",
      "ipv4Public": "192.237.224.6"
    },
    "updated": {
      "time": "2013-10-28T19:17:07Z"
    },
    "halfClosed": false,
    "connectionLogging": {
      "enabled": false
    },
    "contentCaching": {
      "enabled": false
    }
  }
}
10/28/2013 14:17:12 (Request) GET https://ord.loadbalancers.api.rackspacecloud.com/v1.0/841614/loadbalancers/204627
10/28/2013 14:17:12 (Result OK) https://ord.loadbalancers.api.rackspacecloud.com/v1.0/841614/loadbalancers/204627
{
  "loadBalancer": {
    "name": "UnitTestLB-fj3r0koj.euf",
    "id": 204627,
    "protocol": "HTTP",
    "port": 80,
    "algorithm": "ROUND_ROBIN",
    "status": "BUILD",
    "cluster": {
      "name": "ztm-n19.ord1.lbaas.rackspace.net"
    },
    "timeout": 30,
    "created": {
      "time": "2013-10-28T19:17:07Z"
    },
    "virtualIps": [
      {
        "address": "10.189.246.60",
        "id": 13703,
        "type": "SERVICENET",
        "ipVersion": "IPV4"
      }
    ],
    "sourceAddresses": {
      "ipv6Public": "2001:4801:7901::19/64",
      "ipv4Servicenet": "10.189.246.6",
      "ipv4Public": "192.237.224.6"
    },
    "updated": {
      "time": "2013-10-28T19:17:07Z"
    },
    "halfClosed": false,
    "connectionLogging": {
      "enabled": false
    },
    "contentCaching": {
      "enabled": false
    }
  }
}
10/28/2013 14:17:13 (Request) GET https://ord.loadbalancers.api.rackspacecloud.com/v1.0/841614/loadbalancers/204627
10/28/2013 14:17:13 (Result OK) https://ord.loadbalancers.api.rackspacecloud.com/v1.0/841614/loadbalancers/204627
{
  "loadBalancer": {
    "name": "UnitTestLB-fj3r0koj.euf",
    "id": 204627,
    "protocol": "HTTP",
    "port": 80,
    "algorithm": "ROUND_ROBIN",
    "status": "BUILD",
    "cluster": {
      "name": "ztm-n19.ord1.lbaas.rackspace.net"
    },
    "timeout": 30,
    "created": {
      "time": "2013-10-28T19:17:07Z"
    },
    "virtualIps": [
      {
        "address": "10.189.246.60",
        "id": 13703,
        "type": "SERVICENET",
        "ipVersion": "IPV4"
      }
    ],
    "sourceAddresses": {
      "ipv6Public": "2001:4801:7901::19/64",
      "ipv4Servicenet": "10.189.246.6",
      "ipv4Public": "192.237.224.6"
    },
    "updated": {
      "time": "2013-10-28T19:17:07Z"
    },
    "halfClosed": false,
    "connectionLogging": {
      "enabled": false
    },
    "contentCaching": {
      "enabled": false
    }
  }
}
10/28/2013 14:17:14 (Request) GET https://ord.loadbalancers.api.rackspacecloud.com/v1.0/841614/loadbalancers/204627
10/28/2013 14:17:14 (Result OK) https://ord.loadbalancers.api.rackspacecloud.com/v1.0/841614/loadbalancers/204627
{
  "loadBalancer": {
    "name": "UnitTestLB-fj3r0koj.euf",
    "id": 204627,
    "protocol": "HTTP",
    "port": 80,
    "algorithm": "ROUND_ROBIN",
    "status": "ACTIVE",
    "cluster": {
      "name": "ztm-n19.ord1.lbaas.rackspace.net"
    },
    "timeout": 30,
    "created": {
      "time": "2013-10-28T19:17:07Z"
    },
    "virtualIps": [
      {
        "address": "10.189.246.60",
        "id": 13703,
        "type": "SERVICENET",
        "ipVersion": "IPV4"
      }
    ],
    "sourceAddresses": {
      "ipv6Public": "2001:4801:7901::19/64",
      "ipv4Servicenet": "10.189.246.6",
      "ipv4Public": "192.237.224.6"
    },
    "updated": {
      "time": "2013-10-28T19:17:15Z"
    },
    "halfClosed": false,
    "connectionLogging": {
      "enabled": false
    },
    "contentCaching": {
      "enabled": false
    }
  }
}
{
  "enabled": true,
  "secureTrafficOnly": false,
  "securePort": 443,
  "privatekey": "-----BEGIN RSA PRIVATE KEY-----\nMIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBAN0UbfcMDPgrIJejPbFk6wRikCXM\ndtgtmju5bQKw+YijVDrWvi8/RhhfkzzqsDWyod3GvwbXN/efr5YswWXdTGGdH+XPywd+nVnIK/hZ\nQk/b0na5GnGNtwT1oNtIlyJ4cAe4bxDjYTAEYDyoQ8udLtf3tW7s0NzRffxwLZdcXMvjAgMBAAEC\ngYAHQf3wL9GXLhdGUyIZ2kXK4Y945jL6mtim4En/Xh77CN2hht48f9fFwhF73PqG8MPWm4k26Mba\nFsB1bie1N+QjepXtdb32lMpsCcwCXVi/34gjy2tyWuL+abmm7ZVVRFwvz8IF4LIMLHuAyIv+P9a3\nyRZWMtcByJG5AK/Jqcdi4QJBAP1IAstA0kJuUaiWi5KttDsGBydCSsve+VLs5b6LRAIS936/LOiR\nhwCtltF8a9Agj2olrLcS5VDSrFJaUkZIvyECQQDfc+63vjnnpKulBYz6kGrPYV5PPhchWUn1ZBnN\nXJ6LIdjX9YqJAl03YGV2EbSolxp4AqpzeMLmwlQNyXiouT6DAkEA1yACv4AfFi19TiQQCFVhb4B/\nMukrfl20hqqPuHexG9HgRNc73Y09jWXjY6q6J9x/8zhsqlJyU2Oc2ZW9q9S+AQJAGn4KPWqmPtp9\nJzGDR2m74B5xKf/4ihGl/RwDGx/yBBjdNq174UaQerJJFTiALMTNdJEMWGhm4ykDAidy0L41tQJA\nCcvprsobQgMtuXIrQDPBTre76EECZ9PlpuW3ad5UEfRh1bakYh6jpDha3A0Y8ikS0HXGt67J24ec\nBQwpbavtJg==\n-----END RSA PRIVATE KEY-----",
  "certificate": "-----BEGIN CERTIFICATE-----\nMIICnzCCAgigAwIBAgIQGRDXkwGNMI1OrNEoLj/D2zANBgkqhkiG9w0BAQUFADCBjTELMAkGA1UE\nBhMCVVMxDjAMBgNVBAgTBVRleGFzMRQwEgYDVQQHEwtTYW4gQW50b25pbzEYMBYGA1UEChMPb3Bl\nbnN0YWNrbmV0c2RrMRwwGgYDVQQLExNJbnRlZ3JhdGlvbiBUZXN0aW5nMSAwHgYDVQQDExdVbml0\nVGVzdExCLWZqM3Iwa29qLmV1ZjAeFw0xMzEwMjgxNDE3MTRaFw0yMzEwMjgxNDE3MTRaMIGNMQsw\nCQYDVQQGEwJVUzEOMAwGA1UECBMFVGV4YXMxFDASBgNVBAcTC1NhbiBBbnRvbmlvMRgwFgYDVQQK\nEw9vcGVuc3RhY2tuZXRzZGsxHDAaBgNVBAsTE0ludGVncmF0aW9uIFRlc3RpbmcxIDAeBgNVBAMT\nF1VuaXRUZXN0TEItZmozcjBrb2ouZXVmMIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDdFG33\nDAz4KyCXoz2xZOsEYpAlzHbYLZo7uW0CsPmIo1Q61r4vP0YYX5M86rA1sqHdxr8G1zf3n6+WLMFl\n3UxhnR/lz8sHfp1ZyCv4WUJP29J2uRpxjbcE9aDbSJcieHAHuG8Q42EwBGA8qEPLnS7X97Vu7NDc\n0X38cC2XXFzL4wIDAQABMA0GCSqGSIb3DQEBBQUAA4GBAD64852KnNfsxpyl5R2Ac5U51sOd+mPI\nOkxUphANpJQ2zYXi2nGgl5KPEph/tTk8TSzgqeQiw+6zIj3E8W+/sflnMzY2O7h8HoB90X2jk1vY\nGhSIxg4nMHQ9gjWfcvB04HNBAqZAbGjJ9vOfsdx9pTY+nN0InjNhVIXgFxKoTARV\n-----END CERTIFICATE-----"
}
10/28/2013 14:17:14 (Request) PUT https://ord.loadbalancers.api.rackspacecloud.com/v1.0/841614/loadbalancers/204627/ssltermination
10/28/2013 14:17:14 (Result BadRequest) https://ord.loadbalancers.api.rackspacecloud.com/v1.0/841614/loadbalancers/204627/ssltermination

#endif
