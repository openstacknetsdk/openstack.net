using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenStack.Compute.v2_1
{
    public class ComputeTestDataManager : IDisposable
    {
        private readonly ComputeService _compute;
        private readonly HashSet<object> _testData;
         
        public ComputeTestDataManager(ComputeService networkingService)
        {
            _compute = networkingService;
            _testData = new HashSet<object>();
        }

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
    }
}
