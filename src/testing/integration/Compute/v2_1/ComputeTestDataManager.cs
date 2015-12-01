using System;
using System.Collections.Generic;
using System.Linq;

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
                //DeleteServers(_testData.OfType<Server>());
            }
            catch (AggregateException ex) { errors.AddRange(ex.InnerExceptions); }

            if (errors.Any())
                throw new AggregateException("Unable to remove all test data!", errors);
        }

        #region Servers
        //public ServerDefinition BuildServer()
        //{
        //    return new ServerDefinition
        //    {
        //        Name = TestData.GenerateName()
        //    };
        //}

        //public async Task<Server> CreateServer()
        //{
        //    var definition = BuildServer();
        //    return await CreateServer(definition);
        //}

        //public async Task<Server> CreateServer(ServerDefinition definition)
        //{
        //    var server = await _compute.CreateServerAsync(definition);
        //    Register(server);
        //    return server;
        //}

        //public async Task<IEnumerable<Server>> CreateServers()
        //{
        //    var definitions = new[] { BuildServer(), BuildServer(), BuildServer() };
        //    return await CreateServers(definitions);
        //}

        //public async Task<IEnumerable<Server>> CreateServers(IEnumerable<ServerDefinition> definitions)
        //{
        //    var servers = await _compute.CreateServersAsync(definitions);
        //    Register(servers);
        //    return servers;
        //}

        //public void DeleteServers(IEnumerable<Server> networks)
        //{
        //    var deletes = networks.Select(x => _compute.DeleteServerAsync(x.Id)).ToArray();
        //    Task.WaitAll(deletes);
        //}
        #endregion
    }
}
