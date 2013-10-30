namespace Net.OpenStack.Testing.Integration.Providers.Rackspace
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using net.openstack.Core.Domain;
    using net.openstack.Core.Domain.Queues;
    using net.openstack.Core.Providers;
    using net.openstack.Providers.Rackspace;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using CancellationToken = System.Threading.CancellationToken;
    using CancellationTokenSource = System.Threading.CancellationTokenSource;
    using Path = System.IO.Path;
    using WebException = System.Net.WebException;
    using WebExceptionStatus = System.Net.WebExceptionStatus;

    /// <preliminary/>
    [TestClass]
    public class UserQueuesTests
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
        /// <para>
        /// The deletion requests are sent in parallel, so a single deletion failure will
        /// not prevent the method from cleaning up other queues that can be successfully
        /// deleted. Note that the system does not increase the
        /// <see cref="ProviderBase{TProvider}.ConnectionLimit"/>, so the underlying REST
        /// requests may be pipelined if the number of queues to delete exceeds the default
        /// system connection limit.
        /// </para>
        /// </remarks>
        [TestMethod]
        [TestCategory(TestCategories.Cleanup)]
        public void CleanupTestQueues()
        {
            IQueueingService provider = CreateProvider();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(60)));
            QueueName queueName = CreateRandomQueueName();

            IEnumerable<Task<IEnumerable<CloudQueue>>> allQueueTasks = ListAllQueuesAsync(provider, null, false, cancellationTokenSource.Token);
            CloudQueue[] allQueues = allQueueTasks.SelectMany(task => task.Result).ToArray();
            Task[] deleteTasks = Array.ConvertAll(allQueues, queue =>
            {
                Console.WriteLine("Deleting queue: {0}", queue.Name);
                return provider.DeleteQueueAsync(queue.Name, cancellationTokenSource.Token);
            });
            Task.WaitAll(deleteTasks);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Queues)]
        public async Task TestGetHome()
        {
            IQueueingService provider = CreateProvider();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(10)));
            HomeDocument document = await provider.GetHomeAsync(cancellationTokenSource.Token);
            Assert.IsNotNull(document);
            Console.WriteLine(JsonConvert.SerializeObject(document, Formatting.Indented));
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Queues)]
        public async Task TestGetNodeHealth()
        {
            IQueueingService provider = CreateProvider();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(10)));
            await provider.GetNodeHealthAsync(cancellationTokenSource.Token);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Queues)]
        public async Task TestCreateQueue()
        {
            IQueueingService provider = CreateProvider();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(10)));
            QueueName queueName = CreateRandomQueueName();

            bool created = await provider.CreateQueueAsync(queueName, cancellationTokenSource.Token);
            Assert.IsTrue(created);

            bool recreated = await provider.CreateQueueAsync(queueName, cancellationTokenSource.Token);
            Assert.IsFalse(recreated);

            await provider.DeleteQueueAsync(queueName, cancellationTokenSource.Token);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Queues)]
        public async Task TestListQueues()
        {
            IQueueingService provider = CreateProvider();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(10)));
            QueueName queueName = CreateRandomQueueName();

            await provider.CreateQueueAsync(queueName, cancellationTokenSource.Token);

            foreach (Task<IEnumerable<CloudQueue>> queuesTask in ListAllQueuesAsync(provider, null, true, cancellationTokenSource.Token))
            {
                foreach (CloudQueue queue in await queuesTask)
                    Console.WriteLine("{0}: {1}", queue.Name, queue.Href);
            }

            await provider.DeleteQueueAsync(queueName, cancellationTokenSource.Token);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Queues)]
        public async Task TestQueueExists()
        {
            IQueueingService provider = CreateProvider();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(10)));
            QueueName queueName = CreateRandomQueueName();

            await provider.CreateQueueAsync(queueName, cancellationTokenSource.Token);
            Assert.IsTrue(await provider.QueueExistsAsync(queueName, cancellationTokenSource.Token));
            await provider.DeleteQueueAsync(queueName, cancellationTokenSource.Token);
            Assert.IsFalse(await provider.QueueExistsAsync(queueName, cancellationTokenSource.Token));
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Queues)]
        public async Task TestQueueMetadataStatic()
        {
            IQueueingService provider = CreateProvider();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(10)));
            QueueName queueName = CreateRandomQueueName();

            await provider.CreateQueueAsync(queueName, cancellationTokenSource.Token);

            SampleMetadata metadata = new SampleMetadata(3, "yes");
            Assert.AreEqual(3, metadata.ValueA);
            Assert.AreEqual("yes", metadata.ValueB);

            await provider.SetQueueMetadataAsync(queueName, metadata, cancellationTokenSource.Token);
            SampleMetadata result = await provider.GetQueueMetadataAsync<SampleMetadata>(queueName, cancellationTokenSource.Token);
            Assert.AreEqual(metadata.ValueA, result.ValueA);
            Assert.AreEqual(metadata.ValueB, result.ValueB);

            await provider.DeleteQueueAsync(queueName, cancellationTokenSource.Token);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Queues)]
        public async Task TestQueueMetadataDynamic()
        {
            IQueueingService provider = CreateProvider();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(10)));
            QueueName queueName = CreateRandomQueueName();

            await provider.CreateQueueAsync(queueName, cancellationTokenSource.Token);

            JObject metadata = new JObject(
                new JProperty("valueA", 3),
                new JProperty("valueB", "yes"));

            await provider.SetQueueMetadataAsync(queueName, metadata, cancellationTokenSource.Token);
            JObject result = await provider.GetQueueMetadataAsync(queueName, cancellationTokenSource.Token);
            Assert.AreEqual(3, result["valueA"]);
            Assert.AreEqual("yes", result["valueB"]);

            await provider.DeleteQueueAsync(queueName, cancellationTokenSource.Token);
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
        [TestCategory(TestCategories.Queues)]
        public async Task TestQueueStatistics()
        {
            IQueueingService provider = CreateProvider();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(10)));
            QueueName queueName = CreateRandomQueueName();

            await provider.CreateQueueAsync(queueName, cancellationTokenSource.Token);

            QueueStatistics statistics = await provider.GetQueueStatisticsAsync(queueName, cancellationTokenSource.Token);
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

            await provider.PostMessagesAsync(queueName, cancellationTokenSource.Token, new Message<SampleMetadata>(TimeSpan.FromSeconds(120), new SampleMetadata(3, "yes")));

            statistics = await provider.GetQueueStatisticsAsync(queueName, cancellationTokenSource.Token);
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

            await provider.DeleteQueueAsync(queueName, cancellationTokenSource.Token);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Queues)]
        public async Task TestListAllQueueMessages()
        {
            IQueueingService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(10))))
            {
                QueueName queueName = CreateRandomQueueName();

                await provider.CreateQueueAsync(queueName, cancellationTokenSource.Token);

                List<Task> postMessagesTasks = new List<Task>();
                for (int i = 0; i < 28; i++)
                {
                    postMessagesTasks.Add(provider.PostMessagesAsync(queueName, cancellationTokenSource.Token, new Message<SampleMetadata>(TimeSpan.FromSeconds(120), new SampleMetadata(i, "Some Message " + i))));
                }

                await Task.Factory.ContinueWhenAll(postMessagesTasks.ToArray(), TaskExtrasExtensions.PropagateExceptions);

                HashSet<int> locatedMessages = new HashSet<int>();

                QueuedMessageList messages = await provider.ListMessagesAsync(queueName, null, null, true, false, cancellationTokenSource.Token);
                foreach (QueuedMessage message in messages.Messages)
                    Assert.IsTrue(locatedMessages.Add(message.Body.ToObject<SampleMetadata>().ValueA), "Received the same message more than once.");

                int deletedMessage = messages.Messages[0].Body.ToObject<SampleMetadata>().ValueA;
                await provider.DeleteMessageAsync(queueName, messages.Messages[0].Id, null, cancellationTokenSource.Token);

                while (messages.Messages.Count > 0)
                {
                    QueuedMessageList tempList = await provider.ListMessagesAsync(queueName, messages, null, true, false, cancellationTokenSource.Token);
                    if (tempList.Messages.Count > 0)
                    {
                        Assert.IsTrue(locatedMessages.Add(tempList.Messages[0].Body.ToObject<SampleMetadata>().ValueA), "Received the same message more than once.");
                        await provider.DeleteMessageAsync(queueName, tempList.Messages[0].Id, null, cancellationTokenSource.Token);
                    }

                    messages = await provider.ListMessagesAsync(queueName, messages, null, true, false, cancellationTokenSource.Token);
                    foreach (QueuedMessage message in messages.Messages)
                    {
                        Assert.IsTrue(locatedMessages.Add(message.Body.ToObject<SampleMetadata>().ValueA), "Received the same message more than once.");
                    }
                }

                Assert.AreEqual(28, locatedMessages.Count);
                for (int i = 0; i < 28; i++)
                {
                    Assert.IsTrue(locatedMessages.Contains(i), "The message listing did not include message '{0}', which was in the queue when the listing started and still in it afterwards.", i);
                }

                await provider.DeleteQueueAsync(queueName, cancellationTokenSource.Token);
            }
        }

        /// <summary>
        /// Tests the queueing service message functionality by creating two queues
        /// and two sub-processes and using them for two-way communication.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Queues)]
        public async Task TestQueueMessages()
        {
            int clientCount = 3;
            int serverCount = 2;

            QueueName requestQueueName = CreateRandomQueueName();
            QueueName[] responseQueueNames = Enumerable.Range(0, clientCount).Select(i => CreateRandomQueueName()).ToArray();

            IQueueingService provider = CreateProvider();
            CancellationTokenSource testCancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300)));

            Stopwatch initializationTimer = Stopwatch.StartNew();
            Console.WriteLine("Creating request queue...");
            await provider.CreateQueueAsync(requestQueueName, testCancellationTokenSource.Token);
            Console.WriteLine("Creating {0} response queues...", responseQueueNames.Length);
            await Task.Factory.ContinueWhenAll(responseQueueNames.Select(queueName => (Task)provider.CreateQueueAsync(queueName, testCancellationTokenSource.Token)).ToArray(), TaskExtrasExtensions.PropagateExceptions);
            TimeSpan initializationTime = initializationTimer.Elapsed;
            Console.WriteLine("Initialization time: {0} sec", initializationTime.TotalSeconds);

            TimeSpan testDuration = TimeSpan.FromSeconds(10);
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(testDuration);

            Stopwatch processingTimer = Stopwatch.StartNew();

            List<Task<int>> clientTasks = new List<Task<int>>();
            List<Task<int>> serverTasks = new List<Task<int>>();
            for (int i = 0; i < clientCount; i++)
                clientTasks.Add(PublishMessages(requestQueueName, responseQueueNames[i], cancellationTokenSource.Token));
            for (int i = 0; i < serverCount; i++)
                serverTasks.Add(SubscribeMessages(requestQueueName, cancellationTokenSource.Token));

            // wait for all client and server tasks to finish processing
            await Task.Factory.ContinueWhenAll(clientTasks.Concat(serverTasks).Cast<Task>().ToArray(), TaskExtrasExtensions.PropagateExceptions);

            int clientTotal = 0;
            int serverTotal = 0;
            for (int i = 0; i < clientTasks.Count; i++)
            {
                Console.WriteLine("Client {0}: {1} messages", i, clientTasks[i].Result);
                clientTotal += clientTasks[i].Result;
            }
            for (int i = 0; i < serverTasks.Count; i++)
            {
                Console.WriteLine("Server {0}: {1} messages", i, serverTasks[i].Result);
                serverTotal += serverTasks[i].Result;
            }

            double clientRate = clientTotal / testDuration.TotalSeconds;
            double serverRate = serverTotal / testDuration.TotalSeconds;
            Console.WriteLine("Total client messages: {0} ({1} messages/sec, {2} messages/sec/client)", clientTotal, clientRate, clientRate / clientCount);
            Console.WriteLine("Total server messages: {0} ({1} messages/sec, {2} messages/sec/server)", serverTotal, serverRate, serverRate / serverCount);

            Console.WriteLine("Deleting request queue...");
            await provider.DeleteQueueAsync(requestQueueName, testCancellationTokenSource.Token);
            Console.WriteLine("Deleting {0} response queues...", responseQueueNames.Length);
            await Task.Factory.ContinueWhenAll(responseQueueNames.Select(queueName => provider.DeleteQueueAsync(queueName, testCancellationTokenSource.Token)).ToArray(), TaskExtrasExtensions.PropagateExceptions);

            if (clientTotal == 0)
                Assert.Inconclusive("No messages were fully processed by the test.");
        }

        private async Task<int> PublishMessages(QueueName requestQueueName, QueueName replyQueueName, CancellationToken token)
        {
            IQueueingService queueingService = CreateProvider();
            int processedMessages = 0;
            try
            {
                Random random = new Random();

                while (true)
                {
                    long x = random.Next();
                    long y = random.Next();

                    Message<CalculatorOperation> message = new Message<CalculatorOperation>(TimeSpan.FromMinutes(5), new CalculatorOperation(replyQueueName, "+", x, y));
                    await queueingService.PostMessagesAsync(requestQueueName, token, message);

                    bool handled = false;
                    while (true)
                    {
                        // process reply messages
                        using (Claim claim = await queueingService.ClaimMessageAsync(replyQueueName, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), token))
                        {
                            foreach (QueuedMessage queuedMessage in claim.Messages)
                            {
                                CalculatorResult result = queuedMessage.Body.ToObject<CalculatorResult>();
                                if (result._id == message.Body._id)
                                {
                                    // this is the reply to this thread's operation
                                    Assert.AreEqual(message.Body._operand1 + message.Body._operand2, result._result);
                                    Assert.AreEqual(x + y, result._result);
                                    await queueingService.DeleteMessageAsync(replyQueueName, queuedMessage.Id, claim, token);
                                    processedMessages++;
                                    handled = true;
                                }
                                else if (token.IsCancellationRequested)
                                {
                                    // shutdown trigger
                                    return processedMessages;
                                }
                            }

                            // start the dispose process using DisposeAsync so the CancellationToken is honored
                            Task disposeTask = claim.DisposeAsync(token);
                        }

                        if (handled)
                            break;
                    }
                }
            }
            catch (AggregateException ex)
            {
                ex.Flatten().Handle(
                    e =>
                    {
                        if (e is TaskCanceledException)
                            return true;

                        WebException webException = e as WebException;
                        if (webException != null)
                        {
                            if (webException.Status == WebExceptionStatus.RequestCanceled)
                                return true;
                        }

                        return false;
                    });

                return processedMessages;
            }
            catch (TaskCanceledException)
            {
                return processedMessages;
            }
            catch (WebException ex)
            {
                if (ex.Status != WebExceptionStatus.RequestCanceled)
                    throw;

                return processedMessages;
            }
        }

        private async Task<int> SubscribeMessages(QueueName requestQueueName, CancellationToken token)
        {
            IQueueingService queueingService = CreateProvider();
            int processedMessages = 0;
            try
            {
                while (true)
                {
                    // process request messages
                    using (Claim claim = await queueingService.ClaimMessageAsync(requestQueueName, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), token))
                    {
                        List<QueuedMessage> messagesToDelete = new List<QueuedMessage>();

                        foreach (QueuedMessage queuedMessage in claim.Messages)
                        {
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

                            // Assigning result to a local suppresses a warning about calling an asynchronous operation.
                            // In this case, we do not need to wait for the task to finish.
                            Task postTask = queueingService.PostMessagesAsync(operation._replyQueueName, token, new Message<CalculatorResult>(TimeSpan.FromMinutes(5), result));
                            processedMessages++;
                        }

                        if (messagesToDelete.Count > 0)
                            await queueingService.DeleteMessagesAsync(requestQueueName, messagesToDelete.Select(i => i.Id), token);

                        // start the dispose process using DisposeAsync so the CancellationToken is honored
                        Task disposeTask = claim.DisposeAsync(token);
                    }

                    if (token.IsCancellationRequested)
                    {
                        return processedMessages;
                    }
                }
            }
            catch (AggregateException ex)
            {
                ex.Flatten().Handle(
                    e =>
                    {
                        if (e is TaskCanceledException)
                            return true;

                        WebException webException = e as WebException;
                        if (webException != null)
                        {
                            if (webException.Status == WebExceptionStatus.RequestCanceled)
                                return true;
                        }

                        return false;
                    });
                return processedMessages;
            }
            catch (TaskCanceledException)
            {
                return processedMessages;
            }
            catch (WebException ex)
            {
                if (ex.Status != WebExceptionStatus.RequestCanceled)
                    throw;

                return processedMessages;
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
        [TestCategory(TestCategories.Queues)]
        public async Task TestQueueClaims()
        {
            IQueueingService provider = CreateProvider();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(10)));
            QueueName queueName = CreateRandomQueueName();

            await provider.CreateQueueAsync(queueName, cancellationTokenSource.Token);

            await provider.PostMessagesAsync(queueName, cancellationTokenSource.Token, new Message<SampleMetadata>(TimeSpan.FromSeconds(120), new SampleMetadata(3, "yes")));

            QueueStatistics statistics;
            using (Claim claim = await provider.ClaimMessageAsync(queueName, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(1), cancellationTokenSource.Token))
            {
                Assert.AreEqual(TimeSpan.FromMinutes(5), claim.TimeToLive);

                Assert.IsNotNull(claim.Messages);
                Assert.AreEqual(1, claim.Messages.Count);

                statistics = await provider.GetQueueStatisticsAsync(queueName, cancellationTokenSource.Token);
                Assert.AreEqual(1, statistics.MessageStatistics.Claimed);

                QueuedMessage message = await provider.GetMessageAsync(queueName, claim.Messages[0].Id, cancellationTokenSource.Token);
                Assert.IsNotNull(message);

                TimeSpan age = claim.Age;
                await Task.Delay(TimeSpan.FromSeconds(2));
                await claim.RefreshAsync(cancellationTokenSource.Token);
                Assert.IsTrue(claim.Age >= age + TimeSpan.FromSeconds(2));

                await claim.RenewAsync(TimeSpan.FromMinutes(10), cancellationTokenSource.Token);
                Assert.AreEqual(TimeSpan.FromMinutes(10), claim.TimeToLive);
            }

            statistics = await provider.GetQueueStatisticsAsync(queueName, cancellationTokenSource.Token);
            Assert.AreEqual(0, statistics.MessageStatistics.Claimed);

            await provider.DeleteQueueAsync(queueName, cancellationTokenSource.Token);
        }

        /// <summary>
        /// Gets all existing message queues through a series of asynchronous operations,
        /// each of which requests a subset of the available queues.
        /// </summary>
        /// <remarks>
        /// Each of the returned tasks is executed asynchronously but sequentially. This
        /// method will not send concurrent requests to the queueing service.
        /// <para>
        /// Due to the way the list end is detected, the final task will return an empty
        /// collection of <see cref="CloudQueue"/> instances.
        /// </para>
        /// </remarks>
        /// <param name="provider">The queueing service.</param>
        /// <param name="limit">The maximum number of <see cref="CloudQueue"/> to return from a single task. If this value is <c>null</c>, a provider-specific default is used.</param>
        /// <param name="detailed"><c>true</c> to return detailed information for each queue; otherwise, <c>false</c>.</param>
        /// <returns>
        /// A collections of <see cref="Task{TResult}"/> objects, each of which
        /// represents an asynchronous operation to gather a subset of the available
        /// queues.
        /// </returns>
        private static IEnumerable<Task<IEnumerable<CloudQueue>>> ListAllQueuesAsync(IQueueingService provider, int? limit, bool detailed, CancellationToken cancellationToken)
        {
            if (limit <= 0)
                throw new ArgumentOutOfRangeException("limit");

            CloudQueue lastQueue = null;

            do
            {
                QueueName marker = lastQueue != null ? lastQueue.Name : null;
                Task<IEnumerable<CloudQueue>> queues = provider.ListQueuesAsync(marker, limit, detailed, cancellationToken);
                lastQueue = null;
                yield return queues.ContinueWith(task =>
                {
                    return task.Result.Select(queue =>
                    {
                        lastQueue = queue;
                        return queue;
                    });
                });
            } while (lastQueue != null);
        }

        private TimeSpan TestTimeout(TimeSpan timeout)
        {
            if (Debugger.IsAttached)
                return TimeSpan.FromDays(1);

            return timeout;
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
            var provider = new CloudQueuesProvider(Bootstrapper.Settings.TestIdentity, Bootstrapper.Settings.DefaultRegion, Guid.NewGuid(), false, null, null);
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

            provider.ConnectionLimit = 80;
            return provider;
        }
    }
}
