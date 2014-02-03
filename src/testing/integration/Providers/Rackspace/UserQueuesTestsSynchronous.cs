namespace Net.OpenStack.Testing.Integration.Providers.Rackspace
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using net.openstack.Core.Collections;
    using net.openstack.Core.Domain;
    using net.openstack.Core.Domain.Queues;
    using net.openstack.Core.Providers;
    using net.openstack.Core.Synchronous;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using CancellationToken = System.Threading.CancellationToken;
    using CancellationTokenSource = System.Threading.CancellationTokenSource;
    using Path = System.IO.Path;
    using Thread = System.Threading.Thread;

    /// <preliminary/>
    [TestClass]
    public class UserQueuesTestsSynchronous
    {
        /// <summary>
        /// The prefix to use for names of queues created during integration testing.
        /// </summary>
        public static readonly string TestQueuePrefix = "UnitTestQueue-";

        /// <summary>
        /// This method can be used to clean up queues created during integration testing.
        /// </summary>
        /// <remarks>
        /// The Cloud Queues integration tests generally delete queues created during the
        /// tests, but test failures may lead to unused queues gathering on the system.
        /// This method searches for all queues matching the "integration testing" pattern
        /// (i.e., queues whose name starts with <see cref="TestQueuePrefix"/>), and
        /// attempts to delete them.
        /// </remarks>
        [TestMethod]
        [TestCategory(TestCategories.Cleanup)]
        public void SynchronousCleanupTestQueues()
        {
            IQueueingService provider = CreateProvider();
            QueueName queueName = CreateRandomQueueName();

            CloudQueue[] allQueues = ListAllQueues(provider, null, false).ToArray();
            foreach (CloudQueue queue in allQueues)
            {
                Console.WriteLine("Deleting queue: {0}", queue.Name);
                provider.DeleteQueue(queue.Name);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.QueuesSynchronous)]
        public void SynchronousTestGetHome()
        {
            IQueueingService provider = CreateProvider();
            HomeDocument document = provider.GetHome();
            Assert.IsNotNull(document);
            Console.WriteLine(JsonConvert.SerializeObject(document, Formatting.Indented));
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.QueuesSynchronous)]
        public void SynchronousTestGetNodeHealth()
        {
            IQueueingService provider = CreateProvider();
            provider.GetNodeHealth();
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.QueuesSynchronous)]
        public void SynchronousTestCreateQueue()
        {
            IQueueingService provider = CreateProvider();
            QueueName queueName = CreateRandomQueueName();

            bool created = provider.CreateQueue(queueName);
            Assert.IsTrue(created);

            bool recreated = provider.CreateQueue(queueName);
            Assert.IsFalse(recreated);

            provider.DeleteQueue(queueName);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.QueuesSynchronous)]
        public void SynchronousTestListQueues()
        {
            IQueueingService provider = CreateProvider();
            QueueName queueName = CreateRandomQueueName();

            provider.CreateQueue(queueName);

            foreach (CloudQueue queue in ListAllQueues(provider, null, true))
                Console.WriteLine("{0}: {1}", queue.Name, queue.Href);

            provider.DeleteQueue(queueName);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.QueuesSynchronous)]
        public void SynchronousTestQueueExists()
        {
            IQueueingService provider = CreateProvider();
            QueueName queueName = CreateRandomQueueName();

            provider.CreateQueue(queueName);
            Assert.IsTrue(provider.QueueExists(queueName));
            provider.DeleteQueue(queueName);
            Assert.IsFalse(provider.QueueExists(queueName));
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.QueuesSynchronous)]
        public void SynchronousTestQueueMetadataStatic()
        {
            IQueueingService provider = CreateProvider();
            QueueName queueName = CreateRandomQueueName();

            provider.CreateQueue(queueName);

            SampleMetadata metadata = new SampleMetadata(3, "yes");
            Assert.AreEqual(3, metadata.ValueA);
            Assert.AreEqual("yes", metadata.ValueB);

            provider.SetQueueMetadata(queueName, metadata);
            SampleMetadata result = provider.GetQueueMetadata<SampleMetadata>(queueName);
            Assert.AreEqual(metadata.ValueA, result.ValueA);
            Assert.AreEqual(metadata.ValueB, result.ValueB);

            provider.DeleteQueue(queueName);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.QueuesSynchronous)]
        public void SynchronousTestQueueMetadataDynamic()
        {
            IQueueingService provider = CreateProvider();
            QueueName queueName = CreateRandomQueueName();

            provider.CreateQueue(queueName);

            JObject metadata = new JObject(
                new JProperty("valueA", 3),
                new JProperty("valueB", "yes"));

            provider.SetQueueMetadata(queueName, metadata);
            JObject result = provider.GetQueueMetadata(queueName);
            Assert.AreEqual(3, result["valueA"]);
            Assert.AreEqual("yes", result["valueB"]);

            provider.DeleteQueue(queueName);
        }

        [JsonObject(MemberSerialization.OptIn)]
        private class SampleMetadata
        {
            public SampleMetadata(int valueA, string valueB)
            {
                ValueA = valueA;
                ValueB = valueB;
            }

            [JsonProperty("valueA")]
            public int ValueA
            {
                get;
                private set;
            }

            [JsonProperty("valueB")]
            public string ValueB
            {
                get;
                private set;
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.QueuesSynchronous)]
        public void SynchronousTestQueueStatistics()
        {
            IQueueingService provider = CreateProvider();
            QueueName queueName = CreateRandomQueueName();

            provider.CreateQueue(queueName);

            QueueStatistics statistics = provider.GetQueueStatistics(queueName);
            Assert.IsNotNull(statistics);

            QueueMessagesStatistics messageStatistics = statistics.MessageStatistics;
            Assert.IsNotNull(messageStatistics);
            Assert.AreEqual(messageStatistics.Free, 0);
            Assert.AreEqual(messageStatistics.Claimed, 0);
            Assert.AreEqual(messageStatistics.Total, 0);
            Assert.IsNull(messageStatistics.Oldest);
            Assert.IsNull(messageStatistics.Newest);

            Console.WriteLine("Statistics:");
            Console.WriteLine();
            Console.WriteLine(JsonConvert.SerializeObject(statistics, Formatting.Indented));

            provider.PostMessages(queueName, new Message<SampleMetadata>(TimeSpan.FromSeconds(120), new SampleMetadata(3, "yes")));

            statistics = provider.GetQueueStatistics(queueName);
            Assert.IsNotNull(statistics);

            messageStatistics = statistics.MessageStatistics;
            Assert.IsNotNull(messageStatistics);
            Assert.AreEqual(messageStatistics.Free, 1);
            Assert.AreEqual(messageStatistics.Claimed, 0);
            Assert.AreEqual(messageStatistics.Total, 1);
            Assert.IsNotNull(messageStatistics.Oldest);
            Assert.IsNotNull(messageStatistics.Newest);

            Console.WriteLine("Statistics:");
            Console.WriteLine();
            Console.WriteLine(JsonConvert.SerializeObject(statistics, Formatting.Indented));

            provider.DeleteQueue(queueName);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.QueuesSynchronous)]
        public void SynchronousTestListAllQueueMessagesWithUpdates()
        {
            IQueueingService provider = CreateProvider();
            QueueName queueName = CreateRandomQueueName();

            provider.CreateQueue(queueName);

            for (int i = 0; i < 28; i++)
            {
                provider.PostMessages(queueName, new Message<SampleMetadata>(TimeSpan.FromSeconds(120), new SampleMetadata(i, "Some Message " + i)));
            }

            HashSet<int> locatedMessages = new HashSet<int>();

            QueuedMessageList messages = provider.ListMessages(queueName, null, null, true, false);
            foreach (QueuedMessage message in messages)
                Assert.IsTrue(locatedMessages.Add(message.Body.ToObject<SampleMetadata>().ValueA), "Received the same message more than once.");

            int deletedMessage = messages[0].Body.ToObject<SampleMetadata>().ValueA;
            provider.DeleteMessage(queueName, messages[0].Id, null);

            while (messages.Count > 0)
            {
                QueuedMessageList tempList = provider.ListMessages(queueName, messages.NextPageId, null, true, false);
                if (tempList.Count > 0)
                {
                    Assert.IsTrue(locatedMessages.Add(tempList[0].Body.ToObject<SampleMetadata>().ValueA), "Received the same message more than once.");
                    provider.DeleteMessage(queueName, tempList[0].Id, null);
                }

                messages = provider.ListMessages(queueName, messages.NextPageId, null, true, false);
                foreach (QueuedMessage message in messages)
                {
                    Assert.IsTrue(locatedMessages.Add(message.Body.ToObject<SampleMetadata>().ValueA), "Received the same message more than once.");
                }
            }

            Assert.AreEqual(28, locatedMessages.Count);
            for (int i = 0; i < 28; i++)
            {
                Assert.IsTrue(locatedMessages.Contains(i), "The message listing did not include message '{0}', which was in the queue when the listing started and still in it afterwards.", i);
            }

            provider.DeleteQueue(queueName);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.QueuesSynchronous)]
        public void SynchronousTestListAllQueueMessages()
        {
            IQueueingService provider = CreateProvider();
            QueueName queueName = CreateRandomQueueName();

            provider.CreateQueue(queueName);

            for (int i = 0; i < 28; i++)
            {
                provider.PostMessages(queueName, new Message<SampleMetadata>(TimeSpan.FromSeconds(120), new SampleMetadata(i, "Some Message " + i)));
            }

            HashSet<int> locatedMessages = new HashSet<int>();

            ReadOnlyCollection<QueuedMessage> messages = ListAllMessages(provider, queueName, null, true, false);
            foreach (QueuedMessage message in messages)
                Assert.IsTrue(locatedMessages.Add(message.Body.ToObject<SampleMetadata>().ValueA), "Received the same message more than once.");

            Assert.AreEqual(28, locatedMessages.Count);
            for (int i = 0; i < 28; i++)
            {
                Assert.IsTrue(locatedMessages.Contains(i), "The message listing did not include message '{0}', which was in the queue when the listing started and still in it afterwards.", i);
            }

            provider.DeleteQueue(queueName);
        }

        /// <summary>
        /// Tests the queueing service message functionality by creating two queues
        /// and two sub-processes and using them for two-way communication.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.QueuesSynchronous)]
        public void SynchronousTestQueueMessages()
        {
            int clientCount = 3;
            int serverCount = 2;

            QueueName requestQueueName = CreateRandomQueueName();
            QueueName[] responseQueueNames = Enumerable.Range(0, clientCount).Select(i => CreateRandomQueueName()).ToArray();

            IQueueingService provider = CreateProvider();

            Stopwatch initializationTimer = Stopwatch.StartNew();
            Console.WriteLine("Creating request queue...");
            provider.CreateQueue(requestQueueName);
            Console.WriteLine("Creating {0} response queues...", responseQueueNames.Length);
            foreach (QueueName queueName in responseQueueNames)
                provider.CreateQueue(queueName);
            TimeSpan initializationTime = initializationTimer.Elapsed;
            Console.WriteLine("Initialization time: {0} sec", initializationTime.TotalSeconds);

            TimeSpan testDuration = TimeSpan.FromSeconds(10);

            int[] clientResults = new int[clientCount];
            int[] serverResults = new int[serverCount];
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(testDuration))
            {
                Stopwatch processingTimer = Stopwatch.StartNew();

                List<Exception> errors = new List<Exception>();

                List<Thread> clientThreads = new List<Thread>();

                for (int i = 0; i < clientCount; i++)
                {
                    int clientIndex = i;
                    Thread client = new Thread(
                        () =>
                        {
                            try
                            {
                                PublishMessages(requestQueueName, responseQueueNames[clientIndex], ref clientResults[clientIndex], cancellationTokenSource.Token);
                            }
                            catch (Exception ex)
                            {
                                errors.Add(ex);
                            }
                        });
                    client.Start();
                    client.Name = "Client " + i;
                    clientThreads.Add(client);
                }

                List<Thread> serverThreads = new List<Thread>();
                for (int i = 0; i < serverCount; i++)
                {
                    int serverIndex = i;
                    Thread server = new Thread(
                        () =>
                        {
                            try
                            {
                                SubscribeMessages(requestQueueName, ref serverResults[serverIndex], cancellationTokenSource.Token);
                            }
                            catch (Exception ex)
                            {
                                errors.Add(ex);
                            }
                        });
                    server.Start();
                    server.Name = "Server " + i;
                    serverThreads.Add(server);
                }

                // wait for all client and server threads to finish processing
                foreach (Thread thread in clientThreads.Concat(serverThreads))
                    thread.Join();

                if (errors.Count > 0)
                    throw new AggregateException(errors);
            }

            int clientTotal = 0;
            int serverTotal = 0;
            for (int i = 0; i < clientResults.Length; i++)
            {
                Console.WriteLine("Client {0}: {1} messages", i, clientResults[i]);
                clientTotal += clientResults[i];
            }
            for (int i = 0; i < serverResults.Length; i++)
            {
                Console.WriteLine("Server {0}: {1} messages", i, serverResults[i]);
                serverTotal += serverResults[i];
            }

            double clientRate = clientTotal / testDuration.TotalSeconds;
            double serverRate = serverTotal / testDuration.TotalSeconds;
            Console.WriteLine("Total client messages: {0} ({1} messages/sec, {2} messages/sec/client)", clientTotal, clientRate, clientRate / clientCount);
            Console.WriteLine("Total server messages: {0} ({1} messages/sec, {2} messages/sec/server)", serverTotal, serverRate, serverRate / serverCount);

            Console.WriteLine("Deleting request queue...");
            provider.DeleteQueue(requestQueueName);
            Console.WriteLine("Deleting {0} response queues...", responseQueueNames.Length);
            foreach (QueueName queueName in responseQueueNames)
                provider.DeleteQueue(queueName);

            if (clientTotal == 0)
                Assert.Inconclusive("No messages were fully processed by the test.");
        }

        private void PublishMessages(QueueName requestQueueName, QueueName replyQueueName, ref int processedMessages, CancellationToken token)
        {
            IQueueingService queueingService = CreateProvider();
            processedMessages = 0;
            Random random = new Random();

            while (true)
            {
                long x = random.Next();
                long y = random.Next();

                Message<CalculatorOperation> message = new Message<CalculatorOperation>(TimeSpan.FromMinutes(5), new CalculatorOperation(replyQueueName, "+", x, y));
                queueingService.PostMessages(requestQueueName, message);

                bool handled = false;
                while (true)
                {
                    // process reply messages
                    using (Claim claim = queueingService.ClaimMessage(replyQueueName, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1)))
                    {
                        foreach (QueuedMessage queuedMessage in claim.Messages)
                        {
                            CalculatorResult result = queuedMessage.Body.ToObject<CalculatorResult>();
                            if (result._id == message.Body._id)
                            {
                                // this is the reply to this thread's operation
                                Assert.AreEqual(message.Body._operand1 + message.Body._operand2, result._result);
                                Assert.AreEqual(x + y, result._result);
                                queueingService.DeleteMessage(replyQueueName, queuedMessage.Id, claim);
                                processedMessages++;
                                handled = true;
                            }
                            else if (token.IsCancellationRequested)
                            {
                                // shutdown trigger
                                return;
                            }
                        }
                    }

                    if (handled)
                        break;

                    if (token.IsCancellationRequested)
                    {
                        // shutdown trigger
                        return;
                    }
                }

                if (token.IsCancellationRequested)
                {
                    // shutdown trigger
                    return;
                }
            }
        }

        private void SubscribeMessages(QueueName requestQueueName, ref int processedMessages, CancellationToken token)
        {
            IQueueingService queueingService = CreateProvider();
            processedMessages = 0;

            while (true)
            {
                // process request messages
                using (Claim claim = queueingService.ClaimMessage(requestQueueName, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1)))
                {
                    List<QueuedMessage> messagesToDelete = new List<QueuedMessage>();

                    foreach (QueuedMessage queuedMessage in claim.Messages)
                    {
                        if (token.IsCancellationRequested)
                        {
                            return;
                        }

                        CalculatorOperation operation = queuedMessage.Body.ToObject<CalculatorOperation>();
                        CalculatorResult result;
                        switch (operation._command)
                        {
                        case "+":
                            result = new CalculatorResult(operation, operation._operand1 + operation._operand2);
                            break;

                        case "-":
                            result = new CalculatorResult(operation, operation._operand1 - operation._operand2);
                            break;

                        case "*":
                            result = new CalculatorResult(operation, operation._operand1 * operation._operand2);
                            break;

                        case "/":
                            result = new CalculatorResult(operation, operation._operand1 / operation._operand2);
                            break;

                        default:
                            throw new InvalidOperationException();
                        }

                        messagesToDelete.Add(queuedMessage);

                        queueingService.PostMessages(operation._replyQueueName, new Message<CalculatorResult>(TimeSpan.FromMinutes(5), result));
                        processedMessages++;
                    }

                    if (messagesToDelete.Count > 0)
                        queueingService.DeleteMessages(requestQueueName, messagesToDelete.Select(i => i.Id));
                }

                if (token.IsCancellationRequested)
                {
                    return;
                }
            }
        }

        [JsonObject(MemberSerialization.OptIn)]
        private class CalculatorOperation
        {
            [JsonProperty("id")]
            public readonly Guid _id;
            [JsonProperty("command")]
            public readonly string _command;
            [JsonProperty("x")]
            public readonly long _operand1;
            [JsonProperty("y")]
            public readonly long _operand2;
            [JsonProperty("reply")]
            public readonly QueueName _replyQueueName;

            [JsonConstructor]
            private CalculatorOperation()
            {
            }

            public CalculatorOperation(QueueName replyQueueName, string command, long operand1, long operand2)
            {
                _id = Guid.NewGuid();
                _replyQueueName = replyQueueName;
                _command = command;
                _operand1 = operand1;
                _operand2 = operand2;
            }
        }

        [JsonObject(MemberSerialization.OptIn)]
        private class CalculatorResult
        {
            [JsonProperty("id")]
            public readonly Guid _id;
            [JsonProperty("result")]
            public readonly long _result;

            [JsonConstructor]
            private CalculatorResult()
            {
            }

            public CalculatorResult(CalculatorOperation operation, long result)
            {
                if (operation == null)
                    throw new ArgumentNullException("operation");

                _id = operation._id;
                _result = result;
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.QueuesSynchronous)]
        public void SynchronousTestQueueClaims()
        {
            IQueueingService provider = CreateProvider();
            QueueName queueName = CreateRandomQueueName();

            provider.CreateQueue(queueName);

            provider.PostMessages(queueName, new Message<SampleMetadata>(TimeSpan.FromSeconds(120), new SampleMetadata(3, "yes")));

            QueueStatistics statistics;
            using (Claim claim = provider.ClaimMessage(queueName, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(1)))
            {
                Assert.AreEqual(TimeSpan.FromMinutes(5), claim.TimeToLive);

                Assert.IsNotNull(claim.Messages);
                Assert.AreEqual(1, claim.Messages.Count);

                statistics = provider.GetQueueStatistics(queueName);
                Assert.AreEqual(1, statistics.MessageStatistics.Claimed);

                QueuedMessage message = provider.GetMessage(queueName, claim.Messages[0].Id);
                Assert.IsNotNull(message);

                TimeSpan age = claim.Age;
                Thread.Sleep(TimeSpan.FromSeconds(2));
                claim.Refresh();
                Assert.IsTrue(claim.Age >= age + TimeSpan.FromSeconds(2));

                claim.Renew(TimeSpan.FromMinutes(10));
                Assert.AreEqual(TimeSpan.FromMinutes(10), claim.TimeToLive);
            }

            statistics = provider.GetQueueStatistics(queueName);
            Assert.AreEqual(0, statistics.MessageStatistics.Claimed);

            provider.DeleteQueue(queueName);
        }

        /// <summary>
        /// Gets all existing message queues through a series of synchronous operations,
        /// each of which requests a subset of the available queues.
        /// </summary>
        /// <param name="provider">The queueing service.</param>
        /// <param name="limit">The maximum number of <see cref="CloudQueue"/> to return from a single call to <see cref="QueueingServiceExtensions.ListQueues"/>. If this value is <see langword="null"/>, a provider-specific default is used.</param>
        /// <param name="detailed"><see langword="true"/> to return detailed information for each queue; otherwise, <see langword="false"/>.</param>
        /// <returns>A collection of <see cref="CloudQueue"/> objects describing the available queues.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="provider"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="limit"/> is less than or equal to 0.</exception>
        /// <exception cref="WebException">If the REST request does not return successfully.</exception>
        private static ReadOnlyCollection<CloudQueue> ListAllQueues(IQueueingService provider, int? limit, bool detailed)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (limit <= 0)
                throw new ArgumentOutOfRangeException("limit");

            return provider.ListQueues(null, limit, detailed).GetAllPages();
        }

        /// <summary>
        /// Gets all existing messages in a queue through a series of synchronous operations,
        /// each of which requests a subset of the available messages.
        /// </summary>
        /// <param name="provider">The queueing service.</param>
        /// <param name="queueName">The queue name.</param>
        /// <param name="limit">The maximum number of <see cref="QueuedMessage"/> objects to return from a single task. If this value is <see langword="null"/>, a provider-specific default is used.</param>
        /// <param name="echo"><see langword="true"/> to include messages created by the current client; otherwise, <see langword="false"/>.</param>
        /// <param name="includeClaimed"><see langword="true"/> to include claimed messages; otherwise <see langword="false"/> to return only unclaimed messages.</param>
        /// <param name="progress">An optional callback object to receive progress notifications. If this is <see langword="null"/>, no progress notifications are sent.</param>
        /// <returns>A collection of <see cref="CloudQueue"/> objects describing the available queues.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="provider"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="queueName"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="limit"/> is less than or equal to 0.</exception>
        /// <exception cref="WebException">If the REST request does not return successfully.</exception>
        private static ReadOnlyCollection<QueuedMessage> ListAllMessages(IQueueingService provider, QueueName queueName, int? limit, bool echo, bool includeClaimed)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (queueName == null)
                throw new ArgumentNullException("queueName");
            if (limit <= 0)
                throw new ArgumentOutOfRangeException("limit");

            return provider.ListMessages(queueName, null, limit, echo, includeClaimed).GetAllPages();
        }

        /// <summary>
        /// Creates a random queue name with the proper prefix for integration testing.
        /// </summary>
        /// <returns>A unique, randomly-generated queue name.</returns>
        private QueueName CreateRandomQueueName()
        {
            return new QueueName(TestQueuePrefix + Path.GetRandomFileName().Replace('.', '_'));
        }

        /// <summary>
        /// Creates an instance of <see cref="IQueueingService"/> for testing using
        /// the <see cref="OpenstackNetSetings.TestIdentity"/>.
        /// </summary>
        /// <returns>An instance of <see cref="IQueueingService"/> for integration testing.</returns>
        private IQueueingService CreateProvider()
        {
            return UserQueuesTests.CreateProvider();
        }
    }
}
