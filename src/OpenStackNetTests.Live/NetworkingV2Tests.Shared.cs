namespace OpenStackNetTests.Live
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenStack.Collections;
    using OpenStack.Security.Authentication;
    using OpenStack.Services.Networking;
    using OpenStack.Services.Networking.V2;
    using Extension = OpenStack.Services.Identity.V2.Extension;
    using ExtensionResponse = OpenStack.Services.Identity.V2.ExtensionResponse;
    using GetExtensionApiCall = OpenStack.Services.Identity.V2.GetExtensionApiCall;
    using Link = OpenStack.Services.Identity.Link;
    using ListExtensionsApiCall = OpenStack.Services.Identity.V2.ListExtensionsApiCall;
    using Path = System.IO.Path;

    partial class NetworkingV2Tests
    {
        /// <summary>
        /// This prefix is used for networks created by unit tests, to avoid overwriting networks created by other
        /// applications.
        /// </summary>
        private const string TestNetworkPrefix = "UnitTestNetwork-";

        /// <summary>
        /// This prefix is used for subnets created by unit tests, to avoid overwriting subnets created by other
        /// applications.
        /// </summary>
        private const string TestSubnetPrefix = "UnitTestSubnet-";

        /// <summary>
        /// This prefix is used for ports created by unit tests, to avoid overwriting ports created by other
        /// applications.
        /// </summary>
        private const string TestPortPrefix = "UnitTestPort-";

        protected Uri BaseAddress
        {
            get
            {
                TestCredentials credentials = Credentials;
                if (credentials == null)
                    return null;

                return credentials.BaseAddress;
            }
        }

        protected TestProxy Proxy
        {
            get
            {
                TestCredentials credentials = Credentials;
                if (credentials == null)
                    return null;

                return credentials.Proxy;
            }
        }

        protected string Vendor
        {
            get
            {
                TestCredentials credentials = Credentials;
                if (credentials == null)
                    return "OpenStack";

                return credentials.Vendor;
            }
        }

        protected TimeSpan TestTimeout(TimeSpan timeSpan)
        {
            if (Debugger.IsAttached)
                return TimeSpan.FromDays(6);

            return timeSpan;
        }

        /// <summary>
        /// This unit test deletes all networks created by the unit tests.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategories.Cleanup)]
        public async Task CleanupTestNetworks()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(120)));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                Stopwatch timer = Stopwatch.StartNew();

                TestHelpers.SuppressOutput = true;

                INetworkingService service = CreateService();
                ReadOnlyCollection<Network> networks = await ListAllNetworksAsync(service, cancellationToken);
                foreach (Network network in networks)
                {
                    if (!network.Name.StartsWith(TestNetworkPrefix))
                        continue;

                    Console.WriteLine("Removing test network: {0}", network.Name);
                    await service.RemoveNetworkAsync(network.Id, cancellationToken);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Networking)]
        public async Task TestListApiVersions()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (INetworkingService service = CreateService())
                {
                    ListApiVersionsApiCall apiCall = await service.PrepareListApiVersionsAsync(cancellationTokenSource.Token);
                    Tuple<HttpResponseMessage, ReadOnlyCollectionPage<ApiVersion>> response = await apiCall.SendAsync(cancellationTokenSource.Token);

                    Assert.IsNotNull(response);
                    Assert.IsNotNull(response.Item1);

                    ReadOnlyCollectionPage<ApiVersion> versions = response.Item2;
                    Assert.IsNotNull(versions);
                    Assert.AreNotEqual(0, versions.Count);
                    Assert.IsFalse(versions.CanHaveNextPage);
                    Assert.IsFalse(versions.Contains(null));

                    foreach (ApiVersion version in versions)
                    {
                        Assert.IsNotNull(version);
                        Assert.IsNotNull(version.Id);
                        Assert.IsFalse(version.Links.IsDefault);
                        Assert.IsNotNull(version.Status);

                        Assert.AreNotEqual(0, version.Links.Length);
                        foreach (Link link in version.Links)
                        {
                            Assert.IsNotNull(link);
                            Assert.IsNotNull(link.Target);
                            Assert.IsNotNull(link.Relation);
                            Assert.IsTrue(link.Target.IsAbsoluteUri);
                        }
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Networking)]
        public async Task TestListApiVersionsSimple()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (INetworkingService service = CreateService())
                {
                    // test using the simple extension method
                    ReadOnlyCollectionPage<ApiVersion> versions = await service.ListApiVersionsAsync(cancellationTokenSource.Token);
                    Assert.IsNotNull(versions);
                    Assert.AreNotEqual(0, versions.Count);
                    Assert.IsFalse(versions.CanHaveNextPage);
                    Assert.IsFalse(versions.Contains(null));
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Networking)]
        public async Task TestGetApiDetails()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (INetworkingService service = CreateService())
                {
                    // test using the simple extension method
                    ApiDetails details = await service.GetApiDetailsAsync(cancellationTokenSource.Token);
                    Assert.IsNotNull(details);

                    Console.WriteLine("Resources:");
                    Assert.IsFalse(details.Resources.IsDefault);
                    Assert.AreNotEqual(0, details.Resources);
                    foreach (var resource in details.Resources)
                    {
                        Assert.IsNotNull(resource);
                        Assert.IsFalse(string.IsNullOrEmpty(resource.Name));
                        Assert.IsFalse(string.IsNullOrEmpty(resource.Collection));
                        Assert.IsFalse(resource.Links.IsDefault);
                        Assert.IsFalse(resource.Links.Contains(null));
                        Assert.IsTrue(resource.Links.Any(link => string.Equals("self", link.Relation, StringComparison.OrdinalIgnoreCase)));
                        Console.WriteLine("  {0} => {1}", resource.Name, resource.Links.FirstOrDefault(link => string.Equals("self", link.Relation, StringComparison.OrdinalIgnoreCase)).Target);
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Networking)]
        public async Task TestListNetworks()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (INetworkingService service = CreateService())
                {
                    ListNetworksApiCall apiCall = await service.PrepareListNetworksAsync(cancellationTokenSource.Token);
                    Tuple<HttpResponseMessage, ReadOnlyCollectionPage<Network>> response = await apiCall.SendAsync(cancellationTokenSource.Token);

                    Assert.IsNotNull(response);
                    Assert.IsNotNull(response.Item2);

                    ReadOnlyCollectionPage<Network> networks = response.Item2;
                    Assert.IsNotNull(networks);
                    if (networks.Count == 0)
                        Assert.Inconclusive("The networking service does not appear to support any networks.");

                    Assert.IsFalse(networks.CanHaveNextPage);

                    foreach (Network network in networks)
                        CheckNetwork(network);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Networking)]
        public async Task TestListNetworksSimple()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (INetworkingService service = CreateService())
                {
                    ReadOnlyCollectionPage<Network> networks = await service.ListNetworksAsync(cancellationTokenSource.Token);
                    Assert.IsNotNull(networks);
                    if (networks.Count == 0)
                        Assert.Inconclusive("The networking service does not appear to support any networks.");

                    Assert.IsFalse(networks.CanHaveNextPage);

                    foreach (Network network in networks)
                        CheckNetwork(network);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Networking)]
        public async Task TestAddRemoveNetwork()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (INetworkingService service = CreateService())
                {
                    string networkName = TestNetworkPrefix + Path.GetRandomFileName();
                    NetworkData networkData = new NetworkData(networkName);

                    AddNetworkApiCall apiCall = await service.PrepareAddNetworkAsync(new NetworkRequest(networkData), cancellationTokenSource.Token);
                    Tuple<HttpResponseMessage, NetworkResponse> response = await apiCall.SendAsync(cancellationTokenSource.Token);

                    Assert.IsNotNull(response);
                    Assert.IsNotNull(response.Item2);

                    Network network = response.Item2.Network;
                    Assert.IsNotNull(network);
                    CheckNetwork(network);
                    Assert.AreEqual(networkName, network.Name);

                    RemoveNetworkApiCall removeNetworkApiCall = await service.PrepareRemoveNetworkAsync(network.Id, cancellationTokenSource.Token);
                    await removeNetworkApiCall.SendAsync(cancellationTokenSource.Token);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Networking)]
        public async Task TestAddRemoveNetworkSimple()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (INetworkingService service = CreateService())
                {
                    string networkName = TestNetworkPrefix + Path.GetRandomFileName();
                    NetworkData networkData = new NetworkData(networkName);

                    Network network = await service.AddNetworkAsync(networkData, cancellationTokenSource.Token);
                    Assert.IsNotNull(network);
                    CheckNetwork(network);
                    Assert.AreEqual(networkName, network.Name);

                    await service.RemoveNetworkAsync(network.Id, cancellationTokenSource.Token);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Networking)]
        public async Task TestUpdateNetwork()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (INetworkingService service = CreateService())
                {
                    string networkName = TestNetworkPrefix + Path.GetRandomFileName();
                    NetworkData networkData = new NetworkData(networkName);

                    Network network = await service.AddNetworkAsync(networkData, cancellationTokenSource.Token);
                    Assert.IsNotNull(network);
                    CheckNetwork(network);
                    Assert.AreEqual(networkName, network.Name);

                    string updatedNetworkName = TestNetworkPrefix + Path.GetRandomFileName();
                    NetworkData updatedNetworkData = new NetworkData(updatedNetworkName);

                    UpdateNetworkApiCall apiCall = await service.PrepareUpdateNetworkAsync(network.Id, new NetworkRequest(updatedNetworkData), cancellationTokenSource.Token);
                    var response = await apiCall.SendAsync(cancellationTokenSource.Token);
                    Assert.IsNotNull(response);
                    Assert.IsNotNull(response.Item2);
                    Network updatedNetwork = response.Item2.Network;

                    Assert.IsNotNull(updatedNetwork);
                    CheckNetwork(updatedNetwork);
                    Assert.AreEqual(network.Id, updatedNetwork.Id);
                    Assert.AreEqual(updatedNetworkName, updatedNetwork.Name);

                    await service.RemoveNetworkAsync(network.Id, cancellationTokenSource.Token);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Networking)]
        public async Task TestUpdateNetworkSimple()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (INetworkingService service = CreateService())
                {
                    string networkName = TestNetworkPrefix + Path.GetRandomFileName();
                    NetworkData networkData = new NetworkData(networkName);

                    Network network = await service.AddNetworkAsync(networkData, cancellationTokenSource.Token);
                    Assert.IsNotNull(network);
                    CheckNetwork(network);
                    Assert.AreEqual(networkName, network.Name);

                    string updatedNetworkName = TestNetworkPrefix + Path.GetRandomFileName();
                    NetworkData updatedNetworkData = new NetworkData(updatedNetworkName);
                    Network updatedNetwork = await service.UpdateNetworkAsync(network.Id, updatedNetworkData, cancellationTokenSource.Token);
                    Assert.IsNotNull(updatedNetwork);
                    CheckNetwork(updatedNetwork);
                    Assert.AreEqual(network.Id, updatedNetwork.Id);
                    Assert.AreEqual(updatedNetworkName, updatedNetwork.Name);

                    await service.RemoveNetworkAsync(network.Id, cancellationTokenSource.Token);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Networking)]
        public async Task TestGetNetwork()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (INetworkingService service = CreateService())
                {
                    ListNetworksApiCall apiCall = await service.PrepareListNetworksAsync(cancellationTokenSource.Token);
                    Tuple<HttpResponseMessage, ReadOnlyCollectionPage<Network>> response = await apiCall.SendAsync(cancellationTokenSource.Token);

                    Assert.IsNotNull(response);
                    Assert.IsNotNull(response.Item2);

                    ReadOnlyCollectionPage<Network> networks = response.Item2;
                    Assert.IsNotNull(networks);
                    if (networks.Count == 0)
                        Assert.Inconclusive("The networking service does not appear to support any networks.");

                    Assert.IsFalse(networks.CanHaveNextPage);

                    foreach (Network listedNetwork in networks)
                    {
                        Assert.IsNotNull(listedNetwork);
                        Assert.IsNotNull(listedNetwork.Id);

                        GetNetworkApiCall getApiCall = await service.PrepareGetNetworkAsync(listedNetwork.Id, cancellationTokenSource.Token);
                        Tuple<HttpResponseMessage, NetworkResponse> getResponse = await getApiCall.SendAsync(cancellationTokenSource.Token);

                        Network network = getResponse.Item2.Network;
                        CheckNetwork(network);
                        Assert.AreEqual(listedNetwork.Id, network.Id);
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Networking)]
        public async Task TestGetNetworkSimple()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (INetworkingService service = CreateService())
                {
                    ReadOnlyCollectionPage<Network> networks = await service.ListNetworksAsync(cancellationTokenSource.Token);

                    Assert.IsNotNull(networks);
                    if (networks.Count == 0)
                        Assert.Inconclusive("The networking service does not appear to support any networks.");

                    Assert.IsFalse(networks.CanHaveNextPage);

                    foreach (Network listedNetwork in networks)
                    {
                        Assert.IsNotNull(listedNetwork);
                        Assert.IsNotNull(listedNetwork.Id);

                        Network network = await service.GetNetworkAsync(listedNetwork.Id, cancellationTokenSource.Token);
                        CheckNetwork(network);
                        Assert.AreEqual(listedNetwork.Id, network.Id);
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Networking)]
        public async Task TestListSubnets()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (INetworkingService service = CreateService())
                {
                    ListSubnetsApiCall apiCall = await service.PrepareListSubnetsAsync(cancellationTokenSource.Token);
                    Tuple<HttpResponseMessage, ReadOnlyCollectionPage<Subnet>> response = await apiCall.SendAsync(cancellationTokenSource.Token);

                    Assert.IsNotNull(response);
                    Assert.IsNotNull(response.Item2);

                    ReadOnlyCollectionPage<Subnet> subnets = response.Item2;
                    Assert.IsNotNull(subnets);
                    if (subnets.Count == 0)
                        Assert.Inconclusive("The networking service does not appear to support any subnets.");

                    Assert.IsFalse(subnets.CanHaveNextPage);

                    foreach (Subnet subnet in subnets)
                        CheckSubnet(subnet);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Networking)]
        public async Task TestListSubnetsSimple()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (INetworkingService service = CreateService())
                {
                    ReadOnlyCollectionPage<Subnet> subnets = await service.ListSubnetsAsync(cancellationTokenSource.Token);
                    Assert.IsNotNull(subnets);
                    if (subnets.Count == 0)
                        Assert.Inconclusive("The networking service does not appear to support any subnets.");

                    Assert.IsFalse(subnets.CanHaveNextPage);

                    foreach (Subnet subnet in subnets)
                        CheckSubnet(subnet);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Networking)]
        public async Task TestGetSubnet()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (INetworkingService service = CreateService())
                {
                    ListSubnetsApiCall apiCall = await service.PrepareListSubnetsAsync(cancellationTokenSource.Token);
                    Tuple<HttpResponseMessage, ReadOnlyCollectionPage<Subnet>> response = await apiCall.SendAsync(cancellationTokenSource.Token);

                    Assert.IsNotNull(response);
                    Assert.IsNotNull(response.Item2);

                    ReadOnlyCollectionPage<Subnet> subnets = response.Item2;
                    Assert.IsNotNull(subnets);
                    if (subnets.Count == 0)
                        Assert.Inconclusive("The networking service does not appear to support any subnets.");

                    Assert.IsFalse(subnets.CanHaveNextPage);

                    foreach (Subnet listedSubnet in subnets)
                    {
                        Assert.IsNotNull(listedSubnet);
                        Assert.IsNotNull(listedSubnet.Id);

                        GetSubnetApiCall getApiCall = await service.PrepareGetSubnetAsync(listedSubnet.Id, cancellationTokenSource.Token);
                        Tuple<HttpResponseMessage, SubnetResponse> getResponse = await getApiCall.SendAsync(cancellationTokenSource.Token);

                        Subnet subnet = getResponse.Item2.Subnet;
                        CheckSubnet(subnet);
                        Assert.AreEqual(listedSubnet.Id, subnet.Id);
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Networking)]
        public async Task TestGetSubnetSimple()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (INetworkingService service = CreateService())
                {
                    ReadOnlyCollectionPage<Subnet> subnets = await service.ListSubnetsAsync(cancellationTokenSource.Token);

                    Assert.IsNotNull(subnets);
                    if (subnets.Count == 0)
                        Assert.Inconclusive("The networking service does not appear to support any subnets.");

                    Assert.IsFalse(subnets.CanHaveNextPage);

                    foreach (Subnet listedSubnet in subnets)
                    {
                        Assert.IsNotNull(listedSubnet);
                        Assert.IsNotNull(listedSubnet.Id);

                        Subnet subnet = await service.GetSubnetAsync(listedSubnet.Id, cancellationTokenSource.Token);
                        CheckSubnet(subnet);
                        Assert.AreEqual(listedSubnet.Id, subnet.Id);
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Networking)]
        public async Task TestListPorts()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (INetworkingService service = CreateService())
                {
                    ListPortsApiCall apiCall = await service.PrepareListPortsAsync(cancellationTokenSource.Token);
                    Tuple<HttpResponseMessage, ReadOnlyCollectionPage<Port>> response = await apiCall.SendAsync(cancellationTokenSource.Token);

                    Assert.IsNotNull(response);
                    Assert.IsNotNull(response.Item2);

                    ReadOnlyCollectionPage<Port> ports = response.Item2;
                    Assert.IsNotNull(ports);
                    if (ports.Count == 0)
                        Assert.Inconclusive("The networking service does not appear to support any ports.");

                    Assert.IsFalse(ports.CanHaveNextPage);

                    foreach (Port port in ports)
                        CheckPort(port);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Networking)]
        public async Task TestListPortsSimple()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (INetworkingService service = CreateService())
                {
                    ReadOnlyCollectionPage<Port> ports = await service.ListPortsAsync(cancellationTokenSource.Token);
                    Assert.IsNotNull(ports);
                    if (ports.Count == 0)
                        Assert.Inconclusive("The networking service does not appear to support any ports.");

                    Assert.IsFalse(ports.CanHaveNextPage);

                    foreach (Port port in ports)
                        CheckPort(port);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Networking)]
        public async Task TestGetPort()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (INetworkingService service = CreateService())
                {
                    ListPortsApiCall apiCall = await service.PrepareListPortsAsync(cancellationTokenSource.Token);
                    Tuple<HttpResponseMessage, ReadOnlyCollectionPage<Port>> response = await apiCall.SendAsync(cancellationTokenSource.Token);

                    Assert.IsNotNull(response);
                    Assert.IsNotNull(response.Item2);

                    ReadOnlyCollectionPage<Port> ports = response.Item2;
                    Assert.IsNotNull(ports);
                    if (ports.Count == 0)
                        Assert.Inconclusive("The networking service does not appear to support any ports.");

                    Assert.IsFalse(ports.CanHaveNextPage);

                    foreach (Port listedPort in ports)
                    {
                        Assert.IsNotNull(listedPort);
                        Assert.IsNotNull(listedPort.Id);

                        GetPortApiCall getApiCall = await service.PrepareGetPortAsync(listedPort.Id, cancellationTokenSource.Token);
                        Tuple<HttpResponseMessage, PortResponse> getResponse = await getApiCall.SendAsync(cancellationTokenSource.Token);

                        Port port = getResponse.Item2.Port;
                        CheckPort(port);
                        Assert.AreEqual(listedPort.Id, port.Id);
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Networking)]
        public async Task TestGetPortSimple()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (INetworkingService service = CreateService())
                {
                    ReadOnlyCollectionPage<Port> ports = await service.ListPortsAsync(cancellationTokenSource.Token);

                    Assert.IsNotNull(ports);
                    if (ports.Count == 0)
                        Assert.Inconclusive("The networking service does not appear to support any ports.");

                    Assert.IsFalse(ports.CanHaveNextPage);

                    foreach (Port listedPort in ports)
                    {
                        Assert.IsNotNull(listedPort);
                        Assert.IsNotNull(listedPort.Id);

                        Port port = await service.GetPortAsync(listedPort.Id, cancellationTokenSource.Token);
                        CheckPort(port);
                        Assert.AreEqual(listedPort.Id, port.Id);

                        Assert.IsNotNull(listedPort.NetworkId);
                        Network network = await service.GetNetworkAsync(listedPort.NetworkId, cancellationTokenSource.Token);
                        Assert.IsNotNull(network);
                        Assert.IsNotNull(network.Id);
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Networking)]
        public async Task TestListExtensions()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (INetworkingService service = CreateService())
                {
                    ListExtensionsApiCall apiCall = await service.PrepareListExtensionsAsync(cancellationTokenSource.Token);
                    Tuple<HttpResponseMessage, ReadOnlyCollectionPage<Extension>> response = await apiCall.SendAsync(cancellationTokenSource.Token);

                    Assert.IsNotNull(response);
                    Assert.IsNotNull(response.Item2);

                    ReadOnlyCollectionPage<Extension> extensions = response.Item2;
                    Assert.IsNotNull(extensions);
                    if (extensions.Count == 0)
                        Assert.Inconclusive("The networking service does not appear to support any extensions.");

                    Assert.IsFalse(extensions.CanHaveNextPage);

                    foreach (Extension extension in extensions)
                        CheckExtension(extension);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Networking)]
        public async Task TestListExtensionsSimple()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (INetworkingService service = CreateService())
                {
                    ReadOnlyCollectionPage<Extension> extensions = await service.ListExtensionsAsync(cancellationTokenSource.Token);
                    Assert.IsNotNull(extensions);
                    if (extensions.Count == 0)
                        Assert.Inconclusive("The networking service does not appear to support any extensions.");

                    Assert.IsFalse(extensions.CanHaveNextPage);

                    foreach (Extension extension in extensions)
                        CheckExtension(extension);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Networking)]
        public async Task TestGetExtension()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (INetworkingService service = CreateService())
                {
                    ListExtensionsApiCall apiCall = await service.PrepareListExtensionsAsync(cancellationTokenSource.Token);
                    Tuple<HttpResponseMessage, ReadOnlyCollectionPage<Extension>> response = await apiCall.SendAsync(cancellationTokenSource.Token);

                    Assert.IsNotNull(response);
                    Assert.IsNotNull(response.Item2);

                    ReadOnlyCollectionPage<Extension> extensions = response.Item2;
                    Assert.IsNotNull(extensions);
                    if (extensions.Count == 0)
                        Assert.Inconclusive("The networking service does not appear to support any extensions.");

                    Assert.IsFalse(extensions.CanHaveNextPage);

                    foreach (Extension listedExtension in extensions)
                    {
                        Assert.IsNotNull(listedExtension);
                        Assert.IsNotNull(listedExtension.Alias);

                        GetExtensionApiCall getApiCall = await service.PrepareGetExtensionAsync(listedExtension.Alias, cancellationTokenSource.Token);
                        Tuple<HttpResponseMessage, ExtensionResponse> getResponse = await getApiCall.SendAsync(cancellationTokenSource.Token);

                        Extension extension = getResponse.Item2.Extension;
                        CheckExtension(extension);
                        Assert.AreEqual(listedExtension.Alias, extension.Alias);
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Networking)]
        public async Task TestGetExtensionSimple()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(TestTimeout(TimeSpan.FromSeconds(10)));

                using (INetworkingService service = CreateService())
                {
                    ReadOnlyCollectionPage<Extension> extensions = await service.ListExtensionsAsync(cancellationTokenSource.Token);

                    Assert.IsNotNull(extensions);
                    if (extensions.Count == 0)
                        Assert.Inconclusive("The networking service does not appear to support any extensions.");

                    Assert.IsFalse(extensions.CanHaveNextPage);

                    foreach (Extension listedExtension in extensions)
                    {
                        Assert.IsNotNull(listedExtension);
                        Assert.IsNotNull(listedExtension.Alias);

                        Extension extension = await service.GetExtensionAsync(listedExtension.Alias, cancellationTokenSource.Token);
                        CheckExtension(extension);
                        Assert.AreEqual(listedExtension.Alias, extension.Alias);
                    }
                }
            }
        }

        protected async Task<ReadOnlyCollection<Network>> ListAllNetworksAsync(INetworkingService service, CancellationToken cancellationToken)
        {
            return await (await service.ListNetworksAsync(cancellationToken).ConfigureAwait(false)).GetAllPagesAsync(cancellationToken, null);
        }

        protected async Task<ReadOnlyCollection<Subnet>> ListAllSubnetsAsync(INetworkingService service, CancellationToken cancellationToken)
        {
            return await (await service.ListSubnetsAsync(cancellationToken).ConfigureAwait(false)).GetAllPagesAsync(cancellationToken, null);
        }

        protected async Task<ReadOnlyCollection<Port>> ListAllPortsAsync(INetworkingService service, CancellationToken cancellationToken)
        {
            return await (await service.ListPortsAsync(cancellationToken).ConfigureAwait(false)).GetAllPagesAsync(cancellationToken, null);
        }

        protected void CheckNetwork(Network network)
        {
            Assert.IsNotNull(network);
            Assert.IsNotNull(network.Id);
            //Assert.IsNotNull(network.Description);
            Assert.IsNotNull(network.Name);
            //Assert.IsNotNull(network.Namespace);
            //Assert.IsNotNull(network.LastModified);
        }

        protected void CheckSubnet(Subnet subnet)
        {
            Assert.IsNotNull(subnet);
            Assert.IsNotNull(subnet.Id);
            //Assert.IsNotNull(subnet.Description);
            Assert.IsNotNull(subnet.Name);
            //Assert.IsNotNull(subnet.Namespace);
            //Assert.IsNotNull(subnet.LastModified);
        }

        protected void CheckPort(Port port)
        {
            Assert.IsNotNull(port);
            Assert.IsNotNull(port.Id);
            Assert.IsNotNull(port.NetworkId);
            Assert.IsNotNull(port.ProjectId);
            Assert.IsNotNull(port.Status);
            Assert.IsNotNull(port.Name);
            Assert.IsNotNull(port.PhysicalAddress);
            Assert.IsNotNull(port.DeviceOwner);
            Assert.IsNotNull(port.DeviceId);
        }

        protected void CheckExtension(Extension extension)
        {
            Assert.IsNotNull(extension);
            Assert.IsNotNull(extension.Alias);
            Assert.IsNotNull(extension.Description);
            Assert.IsNotNull(extension.Name);
            Assert.IsNotNull(extension.Namespace);
            Assert.IsNotNull(extension.LastModified);
        }

        private INetworkingService CreateService()
        {
            IAuthenticationService authenticationService = IdentityV2Tests.CreateAuthenticationService(Credentials);
            return CreateService(authenticationService, Credentials);
        }

        internal static INetworkingService CreateService(IAuthenticationService authenticationService, TestCredentials credentials)
        {
            NetworkingClient client;
            switch (credentials.Vendor)
            {
            case "HP":
                // currently HP does not have a vendor-specific INetworkingService
                goto default;

            case "Rackspace":
                // currently Rackspace does not have a vendor-specific INetworkingService
                goto default;

            case "OpenStack":
            default:
                client = new NetworkingClient(authenticationService, credentials.DefaultRegion, false);
                break;
            }

            TestProxy.ConfigureService(client, credentials.Proxy);
            client.BeforeAsyncWebRequest += TestHelpers.HandleBeforeAsyncWebRequest;
            client.AfterAsyncWebResponse += TestHelpers.HandleAfterAsyncWebResponse;

            return client;
        }
    }
}
