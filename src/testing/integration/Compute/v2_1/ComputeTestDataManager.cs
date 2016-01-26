using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.openstack.Providers.Rackspace;
using OpenStack.BlockStorage.v2;

namespace OpenStack.Compute.v2_1
{
    public class ComputeTestDataManager : IDisposable
    {
        private readonly ComputeService _compute;
        private readonly HashSet<object> _testData;
         
        public ComputeTestDataManager(ComputeService compute)
        {
            _compute = compute;
            _testData = new HashSet<object>();

            var identityProvider = TestIdentityProvider.GetIdentityProvider();
            var blockStorage = new CloudBlockStorageProvider(null, "RegionOne", identityProvider, null);
            BlockStorage = new BlockStorageTestDataManager(blockStorage);
        }

        public BlockStorageTestDataManager BlockStorage { get; }        

        public void Register(IEnumerable<object> testItems)
        {
            foreach (var testItem in testItems)
            {
                Register(testItem);
            }
        }

        public void Register(object testItem)
        {
            _testData.Add(testItem);
        }

        public void Dispose()
        {
            var errors = new List<Exception>();
            try
            {
                DeleteServers(_testData.OfType<Server>());
            }
            catch (AggregateException ex) { errors.AddRange(ex.InnerExceptions); }

            try
            {
                DeleteImages(_testData.OfType<Image>());
            }
            catch (AggregateException ex) { errors.AddRange(ex.InnerExceptions); }

            try
            {
                DeleteSecurityGroups(_testData.OfType<SecurityGroup>());
            }
            catch (AggregateException ex) { errors.AddRange(ex.InnerExceptions); }

            try
            {
                DeleteKeyPairs(_testData.OfType<KeyPair>());
            }
            catch (AggregateException ex) { errors.AddRange(ex.InnerExceptions); }

            try
            {
                BlockStorage.Dispose();
            }
            catch (AggregateException ex) { errors.AddRange(ex.InnerExceptions); }

            if (errors.Any())
                throw new AggregateException("Unable to remove all test data!", errors);
        }

        #region Servers
        public ServerCreateDefinition BuildServer()
        {
            string name = TestData.GenerateName();
            const string flavor = "1"; // m1.tiny
            Identifier image = new Identifier("74382d40-c0c0-49b0-bacd-42eb3fbaf271"); // cirros
            return new ServerCreateDefinition(name, image, flavor);
        }

        public async Task<Server> CreateServer()
        {
            var definition = BuildServer();
            return await CreateServer(definition);
        }

        public async Task<Server> CreateServer(ServerCreateDefinition definition)
        {
            var server = await _compute.CreateServerAsync(definition);
            Register(server);
            return server;
        }

        public async Task<IEnumerable<Server>> CreateServers()
        {
            var definitions = new[] { BuildServer(), BuildServer(), BuildServer() };
            return await CreateServers(definitions);
        }

        public async Task<IEnumerable<Server>> CreateServers(IEnumerable<ServerCreateDefinition> definitions)
        {
            var creates = definitions.Select(definition => _compute.CreateServerAsync(definition)).ToArray();
            var servers = await Task.WhenAll(creates);
            Register(servers);
            return servers;
        }

        public void DeleteServers(IEnumerable<Server> servers)
        {
            var deletes = servers.Select(x => x.DeleteAsync()).ToArray();
            Task.WaitAll(deletes);
        }
        #endregion

        #region Images
        public void DeleteImages(IEnumerable<Image> images)
        {
            var deletes = images.Select(x => x.DeleteAsync()).ToArray();
            Task.WaitAll(deletes);
        }
        #endregion

        #region Security Groups
        public SecurityGroupDefinition BuildSecurityGroup()
        {
            string name = TestData.GenerateName();
            return new SecurityGroupDefinition(name, "ci test data");
        }

        public async Task<SecurityGroup> CreateSecurityGroup()
        {
            var definition = BuildSecurityGroup();
            return await CreateSecurityGroup(definition);
        }

        public async Task<SecurityGroup> CreateSecurityGroup(SecurityGroupDefinition definition)
        {
            var securityGroup = await _compute.CreateSecurityGroupAsync(definition);
            Register(securityGroup);
            return securityGroup;
        }
        
        public void DeleteSecurityGroups(IEnumerable<SecurityGroup> securityGroups)
        {
            var deletes = securityGroups.Select(x => x.DeleteAsync()).ToArray();
            Task.WaitAll(deletes);
        }
        #endregion

        #region Key Pairs
        public KeyPairRequest BuildKeyPairRequest()
        {
            return new KeyPairRequest(TestData.GenerateName());
        }

        public Task<KeyPairResponse> CreateKeyPair()
        {
            return CreateKeyPair(BuildKeyPairRequest());
        }

        public async Task<KeyPairResponse> CreateKeyPair(KeyPairRequest request)
        {
            var keypair = await _compute.CreateKeyPairAsync(request);
            Register(keypair);
            return keypair;
        }

        public void DeleteKeyPairs(IEnumerable<KeyPair> keypairs)
        {
            var deletes = keypairs.Select(x => x.DeleteAsync()).ToArray();
            Task.WaitAll(deletes);
        }
        #endregion
    }
}
