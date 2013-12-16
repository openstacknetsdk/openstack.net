namespace Net.OpenStack.Testing.Integration.Providers.Rackspace
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using net.openstack.Providers.Rackspace;
    using net.openstack.Providers.Rackspace.Objects.Monitoring;
    using Newtonsoft.Json.Linq;
    using CancellationToken = System.Threading.CancellationToken;
    using CancellationTokenSource = System.Threading.CancellationTokenSource;
    using CloudIdentity = net.openstack.Core.Domain.CloudIdentity;
    using HttpMethod = JSIStudios.SimpleRESTServices.Client.HttpMethod;
    using IIdentityProvider = net.openstack.Core.Providers.IIdentityProvider;
    using Path = System.IO.Path;

    [TestClass]
    public class UserMonitoringTests
    {
        /// <summary>
        /// The prefix to use for names of entities created during integration testing.
        /// </summary>
        public static readonly string TestEntityPrefix = "UnitTestEntity-";

        /// <summary>
        /// The prefix to use for names of notification plans created during integration testing.
        /// </summary>
        public static readonly string TestNotificationPlanPrefix = "UnitTestNotificationPlan-";

        /// <summary>
        /// The prefix to use for names of notifications created during integration testing.
        /// </summary>
        public static readonly string TestNotificationPrefix = "UnitTestNotification-";

        /// <summary>
        /// The prefix to use for names of agent tokens created during integration testing.
        /// </summary>
        public static readonly string TestAgentTokenPrefix = "UnitTestAgentToken-";

        [TestMethod]
        [TestCategory(TestCategories.Cleanup)]
        public async Task CleanupMonitoringEntities()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                List<Task> cleanupTasks = new List<Task>();
                Entity[] entities = await ListAllEntitiesAsync(provider, null, cancellationTokenSource.Token);
                foreach (Entity entity in entities)
                {
                    if (entity.Label == null || !entity.Label.StartsWith(TestEntityPrefix, StringComparison.OrdinalIgnoreCase))
                        continue;

                    Console.WriteLine("Removing entity '{0}' ({1})", entity.Label, entity.Id);
                    cleanupTasks.Add(provider.RemoveEntityAsync(entity.Id, cancellationTokenSource.Token));
                }

                if (cleanupTasks.Count > 0)
                    await Task.Factory.ContinueWhenAll(cleanupTasks.ToArray(), TaskExtrasExtensions.PropagateExceptions);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.Cleanup)]
        public void CleanupMonitoringChecks()
        {
            // these are cleaned up as part of the entity cleanup
        }

        [TestMethod]
        [TestCategory(TestCategories.Cleanup)]
        public void CleanupMonitoringAlarms()
        {
            // these are cleaned up as part of the entity cleanup
        }

        [TestMethod]
        [TestCategory(TestCategories.Cleanup)]
        public async Task CleanupMonitoringNotificationPlans()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                List<Task> cleanupTasks = new List<Task>();
                NotificationPlan[] plans = await ListAllNotificationPlansAsync(provider, null, cancellationTokenSource.Token);
                foreach (NotificationPlan plan in plans)
                {
                    if (plan.Label == null || !plan.Label.StartsWith(TestNotificationPlanPrefix, StringComparison.OrdinalIgnoreCase))
                        continue;

                    Console.WriteLine("Removing notification plan '{0}' ({1})", plan.Label, plan.Id);
                    cleanupTasks.Add(provider.RemoveNotificationPlanAsync(plan.Id, cancellationTokenSource.Token));
                }

                if (cleanupTasks.Count > 0)
                    await Task.Factory.ContinueWhenAll(cleanupTasks.ToArray(), TaskExtrasExtensions.PropagateExceptions);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.Cleanup)]
        public async Task CleanupMonitoringNotifications()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                List<Task> cleanupTasks = new List<Task>();
                Notification[] notifications = await ListAllNotificationsAsync(provider, null, cancellationTokenSource.Token);
                foreach (Notification notification in notifications)
                {
                    if (notification.Label == null || !notification.Label.StartsWith(TestNotificationPrefix, StringComparison.OrdinalIgnoreCase))
                        continue;

                    Console.WriteLine("Removing notification '{0}' ({1})", notification.Label, notification.Id);
                    cleanupTasks.Add(provider.RemoveNotificationAsync(notification.Id, cancellationTokenSource.Token));
                }

                if (cleanupTasks.Count > 0)
                    await Task.Factory.ContinueWhenAll(cleanupTasks.ToArray(), TaskExtrasExtensions.PropagateExceptions);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.Cleanup)]
        public async Task CleanupMonitoringAgentTokens()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                List<Task> cleanupTasks = new List<Task>();
                AgentToken[] agentTokens = await ListAllAgentTokensAsync(provider, null, cancellationTokenSource.Token);
                foreach (AgentToken agentToken in agentTokens)
                {
                    if (agentToken.Label == null || !agentToken.Label.StartsWith(TestAgentTokenPrefix, StringComparison.OrdinalIgnoreCase))
                        continue;

                    Console.WriteLine("Removing agent token '{0}' ({1})", agentToken.Label, agentToken.Id);
                    cleanupTasks.Add(provider.RemoveAgentTokenAsync(agentToken.Id, cancellationTokenSource.Token));
                }

                if (cleanupTasks.Count > 0)
                    await Task.Factory.ContinueWhenAll(cleanupTasks.ToArray(), TaskExtrasExtensions.PropagateExceptions);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestGetAccount()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                MonitoringAccount account = await provider.GetAccountAsync(cancellationTokenSource.Token);
                Assert.IsNotNull(account);
                Assert.IsNotNull(account.Id);
                Assert.IsNotNull(account.WebhookToken);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestUpdateAccount()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                MonitoringAccount account = await provider.GetAccountAsync(cancellationTokenSource.Token);
                Assert.IsNotNull(account);
                Assert.IsNotNull(account.Id);
                Assert.IsNotNull(account.WebhookToken);

                // since changes are account-wide, we have to set the webhook token to the original value
                await provider.UpdateAccountAsync(account.Id, new AccountConfiguration(account.WebhookToken), cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestGetLimits()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                MonitoringLimits limits = await provider.GetLimitsAsync(cancellationTokenSource.Token);
                Assert.IsNotNull(limits);
                Assert.IsNotNull(limits.RateLimits);
                Assert.IsTrue(limits.RateLimits.ContainsKey("global"));
                Assert.IsNotNull(limits.ResourceLimits);
                Assert.IsTrue(limits.ResourceLimits.ContainsKey("checks"));
                Assert.IsTrue(limits.ResourceLimits.ContainsKey("alarms"));
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestListAudits()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                Audit[] audits = await ListAllAuditsAsync(provider, null, null, null, cancellationTokenSource.Token);
                if (audits.Length == 0)
                    Assert.Inconclusive("The service did not report any audits.");

                foreach (Audit audit in audits)
                    Console.WriteLine("Audit {0} ({1})", audit.Url, audit.Id);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestCreateEntity()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string entityName = CreateRandomEntityName();
                NewEntityConfiguration configuration = new NewEntityConfiguration(entityName, null, null, null);
                EntityId entityId = await provider.CreateEntityAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(entityId);

                Entity entity = await provider.GetEntityAsync(entityId, cancellationTokenSource.Token);
                Assert.IsNotNull(entity);
                Assert.AreEqual(entityId, entity.Id);
                Assert.AreEqual(entityName, entity.Label);

                await provider.RemoveEntityAsync(entityId, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestCreateEntityWithMetadata()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string entityName = CreateRandomEntityName();
                IDictionary<string, string> metadata =
                    new Dictionary<string, string>
                    {
                        { "Key 1", "Value 1" },
                        { "key 1", "value 1" },
                        { "Key ²", "Value ²" },
                    };

                NewEntityConfiguration configuration = new NewEntityConfiguration(entityName, null, null, metadata);
                EntityId entityId = await provider.CreateEntityAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(entityId);

                Entity entity = await provider.GetEntityAsync(entityId, cancellationTokenSource.Token);
                Assert.IsNotNull(entity);
                Assert.AreEqual(entityId, entity.Id);
                Assert.AreEqual(entityName, entity.Label);
                Assert.AreEqual("Value 1", entity.Metadata["Key 1"]);
                Assert.AreEqual("value 1", entity.Metadata["key 1"]);
                Assert.AreEqual("Value ²", entity.Metadata["Key ²"]);

                await provider.RemoveEntityAsync(entityId, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestListEntities()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                Entity[] entities = await ListAllEntitiesAsync(provider, null, cancellationTokenSource.Token);
                if (entities.Length == 0)
                    Assert.Inconclusive("The service did not report any entities.");

                foreach (Entity entity in entities)
                    Console.WriteLine("Entity {0} ({1})", entity.Label, entity.Id);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestGetEntity()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                Entity[] entities = await ListAllEntitiesAsync(provider, null, cancellationTokenSource.Token);
                if (entities.Length == 0)
                    Assert.Inconclusive("The service did not report any entities.");

                foreach (Entity entity in entities)
                {
                    Entity singleEntity = await provider.GetEntityAsync(entity.Id, cancellationTokenSource.Token);
                    Assert.IsNotNull(singleEntity);
                    Assert.AreEqual(entity.Id, singleEntity.Id);
                    Assert.AreEqual(entity.AgentId, singleEntity.AgentId);
                    Assert.AreEqual(entity.Label, singleEntity.Label);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestUpdateEntity()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string entityName = CreateRandomEntityName();
                NewEntityConfiguration configuration = new NewEntityConfiguration(entityName, null, null, null);
                EntityId entityId = await provider.CreateEntityAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(entityId);

                Entity entity = await provider.GetEntityAsync(entityId, cancellationTokenSource.Token);
                Assert.IsNotNull(entity);
                Assert.AreEqual(entityId, entity.Id);
                Assert.AreEqual(entityName, entity.Label);

                string updatedEntityName = CreateRandomEntityName();
                UpdateEntityConfiguration updateConfiguration = new UpdateEntityConfiguration(updatedEntityName);
                await provider.UpdateEntityAsync(entityId, updateConfiguration, cancellationTokenSource.Token);
                Entity updatedEntity = await provider.GetEntityAsync(entityId, cancellationTokenSource.Token);
                Assert.IsNotNull(updatedEntity);
                Assert.AreEqual(entityId, updatedEntity.Id);
                Assert.AreEqual(updatedEntityName, updatedEntity.Label);

                await provider.RemoveEntityAsync(entityId, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestCreateCheck()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string entityName = CreateRandomEntityName();
                NewEntityConfiguration configuration = new NewEntityConfiguration(entityName, null, null, null);
                EntityId entityId = await provider.CreateEntityAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(entityId);

                MonitoringZone[] monitoringZones = await ListAllMonitoringZonesAsync(provider, null, cancellationTokenSource.Token);
                MonitoringZone zone = monitoringZones.FirstOrDefault(x => x.Label.IndexOf("DFW", StringComparison.OrdinalIgnoreCase) >= 0);
                zone = zone ?? monitoringZones.First();

                string checkLabel = CreateRandomCheckName();
                CheckTypeId checkTypeId = CheckTypeId.RemoteHttp;
                CheckDetails details = new HttpCheckDetails(
                    url: new Uri("http://docs.rackspace.com", UriKind.Absolute),
                    authUser: default(string),
                    authPassword: default(string),
                    body: default(string),
                    bodyMatches: default(IDictionary<string, string>),
                    followRedirects: default(bool?),
                    headers: default(IDictionary<string, string>),
                    method: default(HttpMethod?),
                    payload: default(string));
                IEnumerable<MonitoringZoneId> monitoringZonesPoll = new[] { zone.Id };
                TimeSpan? timeout = null;
                TimeSpan? period = null;
                string targetAlias = null;
                string targetHostname = "docs.rackspace.com";
                TargetResolverType resolverType = TargetResolverType.IPv4;
                IDictionary<string, string> metadata = null;
                NewCheckConfiguration checkConfiguration = new NewCheckConfiguration(checkLabel, checkTypeId, details, monitoringZonesPoll, timeout, period, targetAlias, targetHostname, resolverType, metadata);
                CheckId checkId = await provider.CreateCheckAsync(entityId, checkConfiguration, cancellationTokenSource.Token);
                Assert.IsNotNull(checkId);

                Entity entity = await provider.GetEntityAsync(entityId, cancellationTokenSource.Token);
                Assert.IsNotNull(entity);
                Assert.AreEqual(entityId, entity.Id);
                Assert.AreEqual(entityName, entity.Label);

                Check check = await provider.GetCheckAsync(entityId, checkId, cancellationTokenSource.Token);
                Assert.IsNotNull(check);
                Assert.AreEqual(checkId, check.Id);
                Assert.AreEqual(checkLabel, check.Label);
                Assert.AreEqual(checkTypeId, check.CheckTypeId);
                Assert.IsInstanceOfType(check.Details, typeof(HttpCheckDetails));

                await provider.RemoveCheckAsync(entityId, checkId, cancellationTokenSource.Token);

                await provider.RemoveEntityAsync(entityId, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestTestCheck()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string entityName = CreateRandomEntityName();
                NewEntityConfiguration configuration = new NewEntityConfiguration(entityName, null, null, null);
                EntityId entityId = await provider.CreateEntityAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(entityId);

                ReadOnlyCollection<MonitoringZone> monitoringZones = await provider.ListMonitoringZonesAsync(null, 1, cancellationTokenSource.Token);
                Assert.AreEqual(1, monitoringZones.Count);

                string checkLabel = CreateRandomCheckName();
                CheckTypeId checkTypeId = CheckTypeId.RemoteHttp;
                CheckDetails details = new HttpCheckDetails(
                    url: new Uri("http://docs.rackspace.com", UriKind.Absolute),
                    authUser: default(string),
                    authPassword: default(string),
                    body: default(string),
                    bodyMatches: default(IDictionary<string, string>),
                    followRedirects: default(bool?),
                    headers: default(IDictionary<string, string>),
                    method: default(HttpMethod?),
                    payload: default(string));
                IEnumerable<MonitoringZoneId> monitoringZonesPoll = monitoringZones.Select(i => i.Id);
                TimeSpan? timeout = null;
                TimeSpan? period = null;
                string targetAlias = null;
                string targetHostname = "docs.rackspace.com";
                TargetResolverType resolverType = TargetResolverType.IPv4;
                IDictionary<string, string> metadata = null;
                NewCheckConfiguration checkConfiguration = new NewCheckConfiguration(checkLabel, checkTypeId, details, monitoringZonesPoll, timeout, period, targetAlias, targetHostname, resolverType, metadata);

                CheckData[] checkData = await provider.TestCheckAsync(entityId, checkConfiguration, null, cancellationTokenSource.Token);
                Assert.IsNotNull(checkData);
                Assert.IsTrue(checkData.Length > 0);
                foreach (CheckData data in checkData)
                {
                    Assert.AreEqual(true, data.Available);
                    Assert.AreEqual(monitoringZones[0].Id, data.MonitoringZoneId);
                    Assert.IsTrue(data.Timestamp >= DateTimeOffset.UtcNow - TimeSpan.FromHours(1));
                    Assert.IsTrue(data.Timestamp <= DateTimeOffset.UtcNow + TimeSpan.FromHours(1));
                    foreach (KeyValuePair<string, CheckData.CheckMetric> checkMetric in data.Metrics)
                    {
                        Assert.IsNotNull(checkMetric.Key);
                        Assert.IsFalse(string.IsNullOrEmpty(checkMetric.Key));
                        Assert.IsNotNull(checkMetric.Value);
                        Assert.IsNotNull(checkMetric.Value.Data);
                        Assert.IsNotNull(checkMetric.Value.Type);
                        Assert.IsNotNull(checkMetric.Value.Unit);
                    }
                }

                await provider.RemoveEntityAsync(entityId, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestTestExistingCheck()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string entityName = CreateRandomEntityName();
                NewEntityConfiguration configuration = new NewEntityConfiguration(entityName, null, null, null);
                EntityId entityId = await provider.CreateEntityAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(entityId);

                ReadOnlyCollection<MonitoringZone> monitoringZones = await provider.ListMonitoringZonesAsync(null, 1, cancellationTokenSource.Token);
                Assert.AreEqual(1, monitoringZones.Count);

                string checkLabel = CreateRandomCheckName();
                CheckTypeId checkTypeId = CheckTypeId.RemoteHttp;
                CheckDetails details = new HttpCheckDetails(
                    url: new Uri("http://docs.rackspace.com", UriKind.Absolute),
                    authUser: default(string),
                    authPassword: default(string),
                    body: default(string),
                    bodyMatches: default(IDictionary<string, string>),
                    followRedirects: default(bool?),
                    headers: default(IDictionary<string, string>),
                    method: default(HttpMethod?),
                    payload: default(string));
                IEnumerable<MonitoringZoneId> monitoringZonesPoll = monitoringZones.Select(i => i.Id);
                TimeSpan? timeout = null;
                TimeSpan? period = null;
                string targetAlias = null;
                string targetHostname = "docs.rackspace.com";
                TargetResolverType resolverType = TargetResolverType.IPv4;
                IDictionary<string, string> metadata = null;
                NewCheckConfiguration checkConfiguration = new NewCheckConfiguration(checkLabel, checkTypeId, details, monitoringZonesPoll, timeout, period, targetAlias, targetHostname, resolverType, metadata);
                CheckId checkId = await provider.CreateCheckAsync(entityId, checkConfiguration, cancellationTokenSource.Token);
                Assert.IsNotNull(checkId);

                CheckData[] checkData = await provider.TestExistingCheckAsync(entityId, checkId, cancellationTokenSource.Token);
                Assert.IsNotNull(checkData);
                Assert.IsTrue(checkData.Length > 0);
                foreach (CheckData data in checkData)
                {
                    Assert.AreEqual(true, data.Available);
                    Assert.AreEqual(monitoringZones[0].Id, data.MonitoringZoneId);
                    Assert.IsTrue(data.Timestamp >= DateTimeOffset.UtcNow - TimeSpan.FromHours(1));
                    Assert.IsTrue(data.Timestamp <= DateTimeOffset.UtcNow + TimeSpan.FromHours(1));
                    foreach (KeyValuePair<string, CheckData.CheckMetric> checkMetric in data.Metrics)
                    {
                        Assert.IsNotNull(checkMetric.Key);
                        Assert.IsFalse(string.IsNullOrEmpty(checkMetric.Key));
                        Assert.IsNotNull(checkMetric.Value);
                        Assert.IsNotNull(checkMetric.Value.Data);
                        Assert.IsNotNull(checkMetric.Value.Type);
                        Assert.IsNotNull(checkMetric.Value.Unit);
                    }
                }

                await provider.RemoveCheckAsync(entityId, checkId, cancellationTokenSource.Token);

                await provider.RemoveEntityAsync(entityId, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestListChecks()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string entityName = CreateRandomEntityName();
                NewEntityConfiguration configuration = new NewEntityConfiguration(entityName, null, null, null);
                EntityId entityId = await provider.CreateEntityAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(entityId);

                ReadOnlyCollection<MonitoringZone> monitoringZones = await provider.ListMonitoringZonesAsync(null, 1, cancellationTokenSource.Token);
                Assert.AreEqual(1, monitoringZones.Count);

                string checkLabel = CreateRandomCheckName();
                CheckTypeId checkTypeId = CheckTypeId.RemoteHttp;
                CheckDetails details = new HttpCheckDetails(
                    url: new Uri("http://docs.rackspace.com", UriKind.Absolute),
                    authUser: default(string),
                    authPassword: default(string),
                    body: default(string),
                    bodyMatches: default(IDictionary<string, string>),
                    followRedirects: default(bool?),
                    headers: default(IDictionary<string, string>),
                    method: default(HttpMethod?),
                    payload: default(string));
                IEnumerable<MonitoringZoneId> monitoringZonesPoll = monitoringZones.Select(i => i.Id);
                TimeSpan? timeout = null;
                TimeSpan? period = null;
                string targetAlias = null;
                string targetHostname = "docs.rackspace.com";
                TargetResolverType resolverType = TargetResolverType.IPv4;
                IDictionary<string, string> metadata = null;
                NewCheckConfiguration checkConfiguration = new NewCheckConfiguration(checkLabel, checkTypeId, details, monitoringZonesPoll, timeout, period, targetAlias, targetHostname, resolverType, metadata);
                CheckId checkId = await provider.CreateCheckAsync(entityId, checkConfiguration, cancellationTokenSource.Token);
                Assert.IsNotNull(checkId);

                string checkLabel2 = CreateRandomCheckName();
                TargetResolverType resolverType2 = TargetResolverType.IPv6;
                NewCheckConfiguration checkConfiguration2 = new NewCheckConfiguration(checkLabel2, checkTypeId, details, monitoringZonesPoll, timeout, period, targetAlias, targetHostname, resolverType, metadata);
                CheckId checkId2 = await provider.CreateCheckAsync(entityId, checkConfiguration2, cancellationTokenSource.Token);
                Assert.IsNotNull(checkId2);

                Check[] checks = await ListAllChecksAsync(provider, entityId, null, cancellationTokenSource.Token);
                Assert.AreEqual(2, checks.Length);
                foreach (Check check in checks)
                    Console.WriteLine("Check {0} ({1})", check.Label, check.Id);

                await provider.RemoveEntityAsync(entityId, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestUpdateCheck()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string entityName = CreateRandomEntityName();
                NewEntityConfiguration configuration = new NewEntityConfiguration(entityName, null, null, null);
                EntityId entityId = await provider.CreateEntityAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(entityId);

                ReadOnlyCollection<MonitoringZone> monitoringZones = await provider.ListMonitoringZonesAsync(null, 1, cancellationTokenSource.Token);
                Assert.AreEqual(1, monitoringZones.Count);

                string checkLabel = CreateRandomCheckName();
                CheckTypeId checkTypeId = CheckTypeId.RemoteHttp;
                CheckDetails details = new HttpCheckDetails(
                    url: new Uri("http://docs.rackspace.com", UriKind.Absolute),
                    authUser: default(string),
                    authPassword: default(string),
                    body: default(string),
                    bodyMatches: default(IDictionary<string, string>),
                    followRedirects: default(bool?),
                    headers: default(IDictionary<string, string>),
                    method: default(HttpMethod?),
                    payload: default(string));
                IEnumerable<MonitoringZoneId> monitoringZonesPoll = monitoringZones.Select(i => i.Id);
                TimeSpan? timeout = null;
                TimeSpan? period = null;
                string targetAlias = null;
                string targetHostname = "docs.rackspace.com";
                TargetResolverType resolverType = TargetResolverType.IPv4;
                IDictionary<string, string> metadata = null;
                NewCheckConfiguration checkConfiguration = new NewCheckConfiguration(checkLabel, checkTypeId, details, monitoringZonesPoll, timeout, period, targetAlias, targetHostname, resolverType, metadata);
                CheckId checkId = await provider.CreateCheckAsync(entityId, checkConfiguration, cancellationTokenSource.Token);
                Assert.IsNotNull(checkId);

                Check check = await provider.GetCheckAsync(entityId, checkId, cancellationTokenSource.Token);
                Assert.IsNotNull(check);
                Assert.AreEqual(checkId, check.Id);
                Assert.AreEqual(checkLabel, check.Label);
                Assert.AreEqual(resolverType, check.ResolverType);
                Assert.IsInstanceOfType(check.Details, typeof(HttpCheckDetails));

                string checkLabel2 = CreateRandomCheckName();
                TargetResolverType resolverType2 = TargetResolverType.IPv6;
                UpdateCheckConfiguration updateCheckConfiguration = new UpdateCheckConfiguration(label: checkLabel2, resolverType: resolverType2);
                await provider.UpdateCheckAsync(entityId, checkId, updateCheckConfiguration, cancellationTokenSource.Token);

                Check updatedCheck = await provider.GetCheckAsync(entityId, checkId, cancellationTokenSource.Token);
                Assert.IsNotNull(updatedCheck);
                Assert.AreEqual(checkId, updatedCheck.Id);
                Assert.AreEqual(checkLabel2, updatedCheck.Label);
                Assert.AreEqual(resolverType2, updatedCheck.ResolverType);
                Assert.IsInstanceOfType(updatedCheck.Details, typeof(HttpCheckDetails));

                await provider.RemoveEntityAsync(entityId, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestListCheckTypes()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                CheckType[] checkTypes = await ListAllCheckTypesAsync(provider, null, cancellationTokenSource.Token);
                if (checkTypes.Length == 0)
                    Assert.Inconclusive("The service did not report any check types.");

                foreach (CheckType checkType in checkTypes)
                {
                    Console.WriteLine("Check Type '{0}' ({1})", checkType.Id, checkType.Type);
                    foreach (NotificationTypeField field in checkType.Fields)
                        Console.WriteLine("    {0}{1} // {2}", field.Name, (field.Optional ?? false) ? " (optional)" : string.Empty, field.Description);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestGetCheckType()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                CheckType[] checkTypes = await ListAllCheckTypesAsync(provider, null, cancellationTokenSource.Token);
                if (checkTypes.Length == 0)
                    Assert.Inconclusive("The service did not report any check types.");

                foreach (CheckType checkType in checkTypes)
                {
                    CheckType singleNotificationType = await provider.GetCheckTypeAsync(checkType.Id, cancellationTokenSource.Token);
                    Assert.IsNotNull(singleNotificationType);
                    Assert.AreEqual(checkType.Id, singleNotificationType.Id);
                    Assert.AreEqual(checkType.Type, singleNotificationType.Type);
                    if (checkType.Fields == null)
                    {
                        Assert.IsNull(singleNotificationType.Fields);
                    }
                    else
                    {
                        Assert.AreEqual(checkType.Fields.Count, singleNotificationType.Fields.Count);
                        for (int i = 0; i < checkType.Fields.Count; i++)
                        {
                            Assert.AreEqual(checkType.Fields[i].Name, singleNotificationType.Fields[i].Name);
                            Assert.AreEqual(checkType.Fields[i].Optional, singleNotificationType.Fields[i].Optional);
                            Assert.AreEqual(checkType.Fields[i].Description, singleNotificationType.Fields[i].Description);
                        }
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestListMetrics()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                bool foundMetrics = false;
                Entity[] entities = await ListAllEntitiesAsync(provider, null, cancellationTokenSource.Token);
                foreach (Entity entity in entities)
                {
                    Console.WriteLine("Entity '{0}'", entity.Label);

                    Check[] checks = await ListAllChecksAsync(provider, entity.Id, null, cancellationTokenSource.Token);
                    foreach (Check check in checks)
                    {
                        Console.WriteLine("  Check '{0}'", check.Label);

                        Metric[] metrics = await ListAllMetricsAsync(provider, entity.Id, check.Id, null, cancellationTokenSource.Token);
                        foundMetrics |= metrics.Any();
                        foreach (Metric metric in metrics)
                        {
                            Console.WriteLine("    Metric '{0}'", metric.Name);
                        }
                    }
                }

                if (!foundMetrics)
                    Assert.Inconclusive("The service did not report any metrics.");
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestGetDataPoints()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                bool foundMetrics = false;
                bool foundData = false;
                Entity[] entities = await ListAllEntitiesAsync(provider, null, cancellationTokenSource.Token);
                foreach (Entity entity in entities)
                {
                    Console.WriteLine("Entity '{0}'", entity.Label);

                    Check[] checks = await ListAllChecksAsync(provider, entity.Id, null, cancellationTokenSource.Token);
                    foreach (Check check in checks)
                    {
                        Console.WriteLine("  Check '{0}'", check.Label);

                        Metric[] metrics = await ListAllMetricsAsync(provider, entity.Id, check.Id, null, cancellationTokenSource.Token);
                        foundMetrics |= metrics.Any();
                        foreach (Metric metric in metrics)
                        {
                            int? points = null;
                            DataPointGranularity resolution = DataPointGranularity.Min240;
                            IEnumerable<DataPointStatistic> select = new[] { DataPointStatistic.NumPoints, DataPointStatistic.Average, DataPointStatistic.Variance, DataPointStatistic.Max };
                            DateTimeOffset from = DateTimeOffset.Now - TimeSpan.FromDays(1);
                            DateTimeOffset to = DateTimeOffset.Now;
                            DataPoint[] dataPoints = await provider.GetDataPointsAsync(entity.Id, check.Id, metric.Name, points, resolution, select, from, to, cancellationTokenSource.Token);
                            foundData |= dataPoints.Any();
                        }

                        if (foundData)
                            break;
                    }

                    if (foundData)
                        break;
                }

                if (!foundMetrics)
                    Assert.Inconclusive("The service did not report any metrics.");
                if (!foundData)
                    Assert.Inconclusive("The service did not report any data points for metrics within the past day.");
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestCreateAlarm()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string entityName = CreateRandomEntityName();
                NewEntityConfiguration configuration = new NewEntityConfiguration(entityName, null, null, null);
                EntityId entityId = await provider.CreateEntityAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(entityId);

                ReadOnlyCollection<MonitoringZone> monitoringZones = await provider.ListMonitoringZonesAsync(null, 1, cancellationTokenSource.Token);
                Assert.AreEqual(1, monitoringZones.Count);

                string checkLabel = CreateRandomCheckName();
                CheckTypeId checkTypeId = CheckTypeId.RemoteHttp;
                CheckDetails details = new HttpCheckDetails(
                    url: new Uri("http://docs.rackspace.com", UriKind.Absolute),
                    authUser: default(string),
                    authPassword: default(string),
                    body: default(string),
                    bodyMatches: default(IDictionary<string, string>),
                    followRedirects: default(bool?),
                    headers: default(IDictionary<string, string>),
                    method: default(HttpMethod?),
                    payload: default(string));
                IEnumerable<MonitoringZoneId> monitoringZonesPoll = monitoringZones.Select(i => i.Id);
                TimeSpan? timeout = null;
                TimeSpan? period = null;
                string targetAlias = null;
                string targetHostname = "docs.rackspace.com";
                TargetResolverType resolverType = TargetResolverType.IPv4;
                IDictionary<string, string> metadata = null;
                NewCheckConfiguration checkConfiguration = new NewCheckConfiguration(checkLabel, checkTypeId, details, monitoringZonesPoll, timeout, period, targetAlias, targetHostname, resolverType, metadata);
                CheckId checkId = await provider.CreateCheckAsync(entityId, checkConfiguration, cancellationTokenSource.Token);
                Assert.IsNotNull(checkId);

                string notificationPlanLabel = CreateRandomNotificationPlanName();
                NewNotificationPlanConfiguration notificationPlanConfiguration = new NewNotificationPlanConfiguration(notificationPlanLabel);
                NotificationPlanId notificationPlanId = await provider.CreateNotificationPlanAsync(notificationPlanConfiguration, cancellationTokenSource.Token);

                string alarmName = CreateRandomAlarmName();
                string criteria = null;
                bool? enabled = null;
                IDictionary<string, string> alarmMetadata = null;
                NewAlarmConfiguration alarmConfiguration = new NewAlarmConfiguration(checkId, notificationPlanId, criteria, enabled, alarmName, alarmMetadata);
                AlarmId alarmId = await provider.CreateAlarmAsync(entityId, alarmConfiguration, cancellationTokenSource.Token);

                Alarm alarm = await provider.GetAlarmAsync(entityId, alarmId, cancellationTokenSource.Token);
                Assert.IsNotNull(alarm);
                Assert.AreEqual(alarmId, alarm.Id);
                Assert.AreEqual(notificationPlanId, alarm.NotificationPlanId);
                Assert.AreEqual(alarmName, alarm.Label);
                Assert.AreEqual(true, alarm.Enabled);
                Assert.AreEqual(checkId, alarm.CheckId);

                await provider.RemoveAlarmAsync(entityId, alarmId, cancellationTokenSource.Token);

                await provider.RemoveNotificationPlanAsync(notificationPlanId, cancellationTokenSource.Token);

                await provider.RemoveCheckAsync(entityId, checkId, cancellationTokenSource.Token);

                await provider.RemoveEntityAsync(entityId, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestTestAlarm()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string entityName = CreateRandomEntityName();
                NewEntityConfiguration configuration = new NewEntityConfiguration(entityName, null, null, null);
                EntityId entityId = await provider.CreateEntityAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(entityId);

                ReadOnlyCollection<MonitoringZone> monitoringZones = await provider.ListMonitoringZonesAsync(null, 1, cancellationTokenSource.Token);
                Assert.AreEqual(1, monitoringZones.Count);

                string checkLabel = CreateRandomCheckName();
                CheckTypeId checkTypeId = CheckTypeId.RemoteHttp;
                CheckDetails details = new HttpCheckDetails(
                    url: new Uri("http://docs.rackspace.com", UriKind.Absolute),
                    authUser: default(string),
                    authPassword: default(string),
                    body: default(string),
                    bodyMatches: default(IDictionary<string, string>),
                    followRedirects: default(bool?),
                    headers: default(IDictionary<string, string>),
                    method: default(HttpMethod?),
                    payload: default(string));
                IEnumerable<MonitoringZoneId> monitoringZonesPoll = monitoringZones.Select(i => i.Id);
                TimeSpan? timeout = null;
                TimeSpan? period = null;
                string targetAlias = null;
                string targetHostname = "docs.rackspace.com";
                TargetResolverType resolverType = TargetResolverType.IPv4;
                IDictionary<string, string> metadata = null;
                NewCheckConfiguration checkConfiguration = new NewCheckConfiguration(checkLabel, checkTypeId, details, monitoringZonesPoll, timeout, period, targetAlias, targetHostname, resolverType, metadata);

                CheckData[] checkData = await provider.TestCheckAsync(entityId, checkConfiguration, null, cancellationTokenSource.Token);
                Assert.IsNotNull(checkData);
                Assert.IsTrue(checkData.Length > 0);

                string criteria = "if (metric[\"code\"] == \"404\") { return new AlarmStatus(CRITICAL, \"not found\"); } return new AlarmStatus(OK);";
                TestAlarmConfiguration testAlarmConfiguration = new TestAlarmConfiguration(criteria, checkData);
                AlarmData[] alarmData = await provider.TestAlarmAsync(entityId, testAlarmConfiguration, cancellationTokenSource.Token);

                foreach (AlarmData data in alarmData)
                {
                    Assert.AreEqual(AlarmState.OK, data.State);
                    Assert.AreEqual("Matched default return statement", data.Status);
                    Assert.IsTrue(data.Timestamp >= DateTimeOffset.UtcNow - TimeSpan.FromHours(1));
                    Assert.IsTrue(data.Timestamp <= DateTimeOffset.UtcNow + TimeSpan.FromHours(1));
                }

                await provider.RemoveEntityAsync(entityId, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestListAlarms()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string entityName = CreateRandomEntityName();
                NewEntityConfiguration configuration = new NewEntityConfiguration(entityName, null, null, null);
                EntityId entityId = await provider.CreateEntityAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(entityId);

                ReadOnlyCollection<MonitoringZone> monitoringZones = await provider.ListMonitoringZonesAsync(null, 1, cancellationTokenSource.Token);
                Assert.AreEqual(1, monitoringZones.Count);

                string checkLabel = CreateRandomCheckName();
                CheckTypeId checkTypeId = CheckTypeId.RemoteHttp;
                CheckDetails details = new HttpCheckDetails(
                    url: new Uri("http://docs.rackspace.com", UriKind.Absolute),
                    authUser: default(string),
                    authPassword: default(string),
                    body: default(string),
                    bodyMatches: default(IDictionary<string, string>),
                    followRedirects: default(bool?),
                    headers: default(IDictionary<string, string>),
                    method: default(HttpMethod?),
                    payload: default(string));
                IEnumerable<MonitoringZoneId> monitoringZonesPoll = monitoringZones.Select(i => i.Id);
                TimeSpan? timeout = null;
                TimeSpan? period = null;
                string targetAlias = null;
                string targetHostname = "docs.rackspace.com";
                TargetResolverType resolverType = TargetResolverType.IPv4;
                IDictionary<string, string> metadata = null;
                NewCheckConfiguration checkConfiguration = new NewCheckConfiguration(checkLabel, checkTypeId, details, monitoringZonesPoll, timeout, period, targetAlias, targetHostname, resolverType, metadata);
                CheckId checkId = await provider.CreateCheckAsync(entityId, checkConfiguration, cancellationTokenSource.Token);
                Assert.IsNotNull(checkId);

                string notificationPlanLabel = CreateRandomNotificationPlanName();
                NewNotificationPlanConfiguration notificationPlanConfiguration = new NewNotificationPlanConfiguration(notificationPlanLabel);
                NotificationPlanId notificationPlanId = await provider.CreateNotificationPlanAsync(notificationPlanConfiguration, cancellationTokenSource.Token);

                string[] alarmNames = { CreateRandomAlarmName(), CreateRandomAlarmName(), CreateRandomAlarmName() };
                string criteria = null;
                bool? enabled = null;
                IDictionary<string, string> alarmMetadata = null;

                List<AlarmId> alarmIds = new List<AlarmId>();
                foreach (string alarmName in alarmNames)
                {
                    NewAlarmConfiguration alarmConfiguration = new NewAlarmConfiguration(checkId, notificationPlanId, criteria, enabled, alarmName, alarmMetadata);
                    AlarmId alarmId = await provider.CreateAlarmAsync(entityId, alarmConfiguration, cancellationTokenSource.Token);
                    alarmIds.Add(alarmId);
                }

                Alarm[] alarms = await ListAllAlarmsAsync(provider, entityId, null, cancellationTokenSource.Token);
                Assert.IsTrue(alarms.Length >= 3);
                foreach (string alarmName in alarmNames)
                {
                    Alarm alarm = alarms.Single(i => i.Label.Equals(alarmName));
                    Assert.IsTrue(alarm.Created >= DateTimeOffset.UtcNow - TimeSpan.FromHours(1));
                    Assert.IsTrue(alarm.Created <= DateTimeOffset.UtcNow + TimeSpan.FromHours(1));
                    Assert.IsTrue(alarm.LastModified >= DateTimeOffset.UtcNow - TimeSpan.FromHours(1));
                    Assert.IsTrue(alarm.LastModified <= DateTimeOffset.UtcNow + TimeSpan.FromHours(1));
                    Assert.AreEqual(notificationPlanId, alarm.NotificationPlanId);
                    Assert.AreEqual(true, alarm.Enabled);
                    Assert.AreEqual(checkId, alarm.CheckId);

                    Alarm singleAlarm = await provider.GetAlarmAsync(entityId, alarm.Id, cancellationTokenSource.Token);
                    Assert.IsNotNull(singleAlarm);
                    Assert.AreEqual(alarmName, singleAlarm.Label);
                    Assert.AreEqual(alarm.CheckId, singleAlarm.CheckId);
                    Assert.AreEqual(alarm.Created, singleAlarm.Created);
                    Assert.AreEqual(alarm.Criteria, singleAlarm.Criteria);
                    Assert.AreEqual(alarm.Enabled, singleAlarm.Enabled);
                    Assert.AreEqual(alarm.Id, singleAlarm.Id);
                    Assert.AreEqual(alarm.Label, singleAlarm.Label);
                    Assert.IsTrue(alarm.LastModified <= singleAlarm.LastModified);
                    Assert.AreEqual(alarm.NotificationPlanId, singleAlarm.NotificationPlanId);
                }

                foreach (AlarmId alarmId in alarmIds)
                    await provider.RemoveAlarmAsync(entityId, alarmId, cancellationTokenSource.Token);

                await provider.RemoveNotificationPlanAsync(notificationPlanId, cancellationTokenSource.Token);

                await provider.RemoveCheckAsync(entityId, checkId, cancellationTokenSource.Token);

                await provider.RemoveEntityAsync(entityId, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestUpdateAlarm()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string entityName = CreateRandomEntityName();
                NewEntityConfiguration configuration = new NewEntityConfiguration(entityName, null, null, null);
                EntityId entityId = await provider.CreateEntityAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(entityId);

                ReadOnlyCollection<MonitoringZone> monitoringZones = await provider.ListMonitoringZonesAsync(null, 1, cancellationTokenSource.Token);
                Assert.AreEqual(1, monitoringZones.Count);

                string checkLabel = CreateRandomCheckName();
                CheckTypeId checkTypeId = CheckTypeId.RemoteHttp;
                CheckDetails details = new HttpCheckDetails(
                    url: new Uri("http://docs.rackspace.com", UriKind.Absolute),
                    authUser: default(string),
                    authPassword: default(string),
                    body: default(string),
                    bodyMatches: default(IDictionary<string, string>),
                    followRedirects: default(bool?),
                    headers: default(IDictionary<string, string>),
                    method: default(HttpMethod?),
                    payload: default(string));
                IEnumerable<MonitoringZoneId> monitoringZonesPoll = monitoringZones.Select(i => i.Id);
                TimeSpan? timeout = null;
                TimeSpan? period = null;
                string targetAlias = null;
                string targetHostname = "docs.rackspace.com";
                TargetResolverType resolverType = TargetResolverType.IPv4;
                IDictionary<string, string> metadata = null;
                NewCheckConfiguration checkConfiguration = new NewCheckConfiguration(checkLabel, checkTypeId, details, monitoringZonesPoll, timeout, period, targetAlias, targetHostname, resolverType, metadata);
                CheckId checkId = await provider.CreateCheckAsync(entityId, checkConfiguration, cancellationTokenSource.Token);
                Assert.IsNotNull(checkId);

                string notificationPlanLabel = CreateRandomNotificationPlanName();
                NewNotificationPlanConfiguration notificationPlanConfiguration = new NewNotificationPlanConfiguration(notificationPlanLabel);
                NotificationPlanId notificationPlanId = await provider.CreateNotificationPlanAsync(notificationPlanConfiguration, cancellationTokenSource.Token);

                string alarmName = CreateRandomAlarmName();
                string criteria = null;
                bool? enabled = null;
                IDictionary<string, string> alarmMetadata = null;
                NewAlarmConfiguration alarmConfiguration = new NewAlarmConfiguration(checkId, notificationPlanId, criteria, enabled, alarmName, alarmMetadata);
                AlarmId alarmId = await provider.CreateAlarmAsync(entityId, alarmConfiguration, cancellationTokenSource.Token);

                Alarm alarm = await provider.GetAlarmAsync(entityId, alarmId, cancellationTokenSource.Token);
                Assert.IsNotNull(alarm);
                Assert.AreEqual(alarmId, alarm.Id);
                Assert.AreEqual(notificationPlanId, alarm.NotificationPlanId);
                Assert.AreEqual(alarmName, alarm.Label);
                Assert.AreEqual(true, alarm.Enabled);
                Assert.AreEqual(checkId, alarm.CheckId);

                string updatedAlarmName = CreateRandomAlarmName();
                UpdateAlarmConfiguration updateAlarmConfiguration = new UpdateAlarmConfiguration(label: updatedAlarmName);
                await provider.UpdateAlarmAsync(entityId, alarmId, updateAlarmConfiguration, cancellationTokenSource.Token);

                Alarm updatedAlarm = await provider.GetAlarmAsync(entityId, alarmId, cancellationTokenSource.Token);
                Assert.IsNotNull(updatedAlarm);
                Assert.AreEqual(alarmId, updatedAlarm.Id);
                Assert.AreEqual(notificationPlanId, updatedAlarm.NotificationPlanId);
                Assert.AreEqual(updatedAlarmName, updatedAlarm.Label);
                Assert.AreEqual(true, updatedAlarm.Enabled);
                Assert.AreEqual(checkId, updatedAlarm.CheckId);

                await provider.RemoveAlarmAsync(entityId, alarmId, cancellationTokenSource.Token);

                await provider.RemoveNotificationPlanAsync(notificationPlanId, cancellationTokenSource.Token);

                await provider.RemoveCheckAsync(entityId, checkId, cancellationTokenSource.Token);

                await provider.RemoveEntityAsync(entityId, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestCreateNotificationPlan()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string label = CreateRandomNotificationPlanName();
                NewNotificationPlanConfiguration configuration = new NewNotificationPlanConfiguration(label);
                NotificationPlanId notificationPlanId = await provider.CreateNotificationPlanAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(notificationPlanId);

                NotificationPlan notificationPlan = await provider.GetNotificationPlanAsync(notificationPlanId, cancellationTokenSource.Token);
                Assert.IsNotNull(notificationPlan);
                Assert.AreEqual(notificationPlanId, notificationPlan.Id);
                Assert.AreEqual(label, notificationPlan.Label);

                await provider.RemoveNotificationPlanAsync(notificationPlanId, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestCreateNotificationPlanWithEmptyNotifications()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string label = CreateRandomNotificationPlanName();
                IEnumerable<NotificationId> emptyNotifications = Enumerable.Empty<NotificationId>();
                NewNotificationPlanConfiguration configuration = new NewNotificationPlanConfiguration(label, emptyNotifications, emptyNotifications, emptyNotifications);
                NotificationPlanId notificationPlanId = await provider.CreateNotificationPlanAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(notificationPlanId);

                NotificationPlan notificationPlan = await provider.GetNotificationPlanAsync(notificationPlanId, cancellationTokenSource.Token);
                Assert.IsNotNull(notificationPlan);
                Assert.AreEqual(notificationPlanId, notificationPlan.Id);
                Assert.AreEqual(label, notificationPlan.Label);
                CollectionAssert.AreEqual(emptyNotifications.ToArray(), notificationPlan.CriticalState);
                CollectionAssert.AreEqual(emptyNotifications.ToArray(), notificationPlan.WarningState);
                CollectionAssert.AreEqual(emptyNotifications.ToArray(), notificationPlan.OkState);

                await provider.RemoveNotificationPlanAsync(notificationPlanId, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestCreateNotificationPlanWithMetadata()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string label = CreateRandomNotificationPlanName();
                Dictionary<string, string> metadata =
                    new Dictionary<string, string>
                    {
                        { "Key 1", "Value 1" },
                        { "key 1", "value 1" },
                        { "Key ²", "Value ²" },
                    };
                NewNotificationPlanConfiguration configuration = new NewNotificationPlanConfiguration(label, metadata: metadata);
                NotificationPlanId notificationPlanId = await provider.CreateNotificationPlanAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(notificationPlanId);

                NotificationPlan notificationPlan = await provider.GetNotificationPlanAsync(notificationPlanId, cancellationTokenSource.Token);
                Assert.IsNotNull(notificationPlan);
                Assert.AreEqual(notificationPlanId, notificationPlan.Id);
                Assert.AreEqual(label, notificationPlan.Label);

                Assert.IsNotNull(notificationPlan.Metadata);
                Assert.AreEqual("Value 1", notificationPlan.Metadata["Key 1"]);
                Assert.AreEqual("value 1", notificationPlan.Metadata["key 1"]);
                Assert.AreEqual("Value ²", notificationPlan.Metadata["Key ²"]);

                await provider.RemoveNotificationPlanAsync(notificationPlanId, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestListNotificationPlans()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                NotificationPlan[] notificationPlans = await ListAllNotificationPlansAsync(provider, null, cancellationTokenSource.Token);
                if (notificationPlans.Length == 0)
                    Assert.Inconclusive("The service did not report any notification plans.");

                foreach (NotificationPlan notificationPlan in notificationPlans)
                    Console.WriteLine("Notification plan {0}", notificationPlan.Label);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestGetNotificationPlan()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                NotificationPlan[] notificationPlans = await ListAllNotificationPlansAsync(provider, null, cancellationTokenSource.Token);
                if (notificationPlans.Length == 0)
                    Assert.Inconclusive("The service did not report any notification plans.");

                foreach (NotificationPlan notificationPlan in notificationPlans)
                {
                    NotificationPlan singlePlan = await provider.GetNotificationPlanAsync(notificationPlan.Id, cancellationTokenSource.Token);
                    Assert.IsNotNull(singlePlan);
                    Assert.AreEqual(notificationPlan.Id, singlePlan.Id);
                    Assert.AreEqual(notificationPlan.Label, singlePlan.Label);
                    Assert.AreEqual(notificationPlan.Created, singlePlan.Created);
                    Assert.AreEqual(notificationPlan.LastModified, singlePlan.LastModified);
                    CollectionAssert.AreEqual(notificationPlan.CriticalState, singlePlan.CriticalState);
                    CollectionAssert.AreEqual(notificationPlan.WarningState, singlePlan.WarningState);
                    CollectionAssert.AreEqual(notificationPlan.OkState, singlePlan.OkState);
                    CollectionAssert.AreEqual(notificationPlan.Metadata, singlePlan.Metadata);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestUpdateNotificationPlan()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string label = CreateRandomNotificationPlanName();
                NewNotificationPlanConfiguration configuration = new NewNotificationPlanConfiguration(label);
                NotificationPlanId notificationPlanId = await provider.CreateNotificationPlanAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(notificationPlanId);

                NotificationPlan notificationPlan = await provider.GetNotificationPlanAsync(notificationPlanId, cancellationTokenSource.Token);
                Assert.IsNotNull(notificationPlan);
                Assert.AreEqual(notificationPlanId, notificationPlan.Id);
                Assert.AreEqual(label, notificationPlan.Label);

                string newLabel = CreateRandomNotificationPlanName();
                UpdateNotificationPlanConfiguration updateConfiguration = new UpdateNotificationPlanConfiguration(newLabel);
                await provider.UpdateNotificationPlanAsync(notificationPlanId, updateConfiguration, cancellationTokenSource.Token);

                NotificationPlan updatedNotificationPlan = await provider.GetNotificationPlanAsync(notificationPlanId, cancellationTokenSource.Token);
                Assert.IsNotNull(updatedNotificationPlan);
                Assert.AreEqual(notificationPlanId, updatedNotificationPlan.Id);
                Assert.AreEqual(newLabel, updatedNotificationPlan.Label);

                await provider.RemoveNotificationPlanAsync(notificationPlanId, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestUpdateNotificationPlanWithMetadata()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string label = CreateRandomNotificationPlanName();
                Dictionary<string, string> metadata =
                    new Dictionary<string, string>
                    {
                        { "Key 1", "Value 1" },
                        { "key 1", "value 1" },
                        { "Key ²", "Value ²" },
                    };
                NewNotificationPlanConfiguration configuration = new NewNotificationPlanConfiguration(label, metadata: metadata);
                NotificationPlanId notificationPlanId = await provider.CreateNotificationPlanAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(notificationPlanId);

                NotificationPlan notificationPlan = await provider.GetNotificationPlanAsync(notificationPlanId, cancellationTokenSource.Token);
                Assert.IsNotNull(notificationPlan);
                Assert.AreEqual(notificationPlanId, notificationPlan.Id);
                Assert.AreEqual(label, notificationPlan.Label);

                Assert.IsNotNull(notificationPlan.Metadata);
                Assert.AreEqual("Value 1", notificationPlan.Metadata["Key 1"]);
                Assert.AreEqual("value 1", notificationPlan.Metadata["key 1"]);
                Assert.AreEqual("Value ²", notificationPlan.Metadata["Key ²"]);

                // setting the label alone doesn't affect metadata
                string newLabel = CreateRandomNotificationPlanName();
                UpdateNotificationPlanConfiguration updateConfiguration = new UpdateNotificationPlanConfiguration(newLabel);
                await provider.UpdateNotificationPlanAsync(notificationPlanId, updateConfiguration, cancellationTokenSource.Token);

                NotificationPlan updatedNotificationPlan = await provider.GetNotificationPlanAsync(notificationPlanId, cancellationTokenSource.Token);
                Assert.IsNotNull(updatedNotificationPlan);
                Assert.AreEqual(notificationPlanId, updatedNotificationPlan.Id);
                Assert.AreEqual(newLabel, updatedNotificationPlan.Label);

                Assert.IsNotNull(notificationPlan.Metadata);
                Assert.AreEqual("Value 1", updatedNotificationPlan.Metadata["Key 1"]);
                Assert.AreEqual("value 1", updatedNotificationPlan.Metadata["key 1"]);
                Assert.AreEqual("Value ²", updatedNotificationPlan.Metadata["Key ²"]);

                // setting metadata replaces all metadata
                Dictionary<string, string> newMetadata =
                    new Dictionary<string, string>
                    {
                        { "Key 3", "Value ³" }
                    };
                updateConfiguration = new UpdateNotificationPlanConfiguration(metadata: newMetadata);
                await provider.UpdateNotificationPlanAsync(notificationPlanId, updateConfiguration, cancellationTokenSource.Token);

                updatedNotificationPlan = await provider.GetNotificationPlanAsync(notificationPlanId, cancellationTokenSource.Token);
                Assert.IsNotNull(updatedNotificationPlan);
                Assert.AreEqual(notificationPlanId, updatedNotificationPlan.Id);
                Assert.AreEqual(newLabel, updatedNotificationPlan.Label);

                Assert.IsNotNull(updatedNotificationPlan.Metadata);
                CollectionAssert.AreEqual(newMetadata, updatedNotificationPlan.Metadata);

                await provider.RemoveNotificationPlanAsync(notificationPlanId, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestListMonitoringZones()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                MonitoringZone[] monitoringZones = await ListAllMonitoringZonesAsync(provider, null, cancellationTokenSource.Token);
                if (monitoringZones.Length == 0)
                    Assert.Inconclusive("The provider did not return any monitoring zones.");

                foreach (MonitoringZone monitoringZone in monitoringZones)
                    Console.WriteLine("Monitoring zone '{0}'", monitoringZone.Label);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestGetMonitoringZone()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                MonitoringZone[] monitoringZones = await ListAllMonitoringZonesAsync(provider, null, cancellationTokenSource.Token);
                if (monitoringZones.Length == 0)
                    Assert.Inconclusive("The provider did not return any monitoring zones.");

                foreach (MonitoringZone monitoringZone in monitoringZones)
                {
                    MonitoringZone singleMonitoringZone = await provider.GetMonitoringZoneAsync(monitoringZone.Id, cancellationTokenSource.Token);
                    Assert.IsNotNull(singleMonitoringZone);
                    Assert.AreEqual(monitoringZone.Id, singleMonitoringZone.Id);
                    Assert.AreEqual(monitoringZone.CountryCode, singleMonitoringZone.CountryCode);
                    Assert.AreEqual(monitoringZone.Label, singleMonitoringZone.Label);
                    CollectionAssert.AreEqual(monitoringZone.SourceAddresses, singleMonitoringZone.SourceAddresses);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestTraceRouteFromMonitoringZone()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                MonitoringZone[] monitoringZones = await ListAllMonitoringZonesAsync(provider, null, cancellationTokenSource.Token);
                if (monitoringZones.Length == 0)
                    Assert.Inconclusive("The provider did not return any monitoring zones.");

                string target = "docs.rackspace.com";
                TraceRouteConfiguration configuration = new TraceRouteConfiguration(target, TargetResolverType.IPv4);
                foreach (MonitoringZone monitoringZone in monitoringZones)
                {
                    Console.WriteLine("Performing traceroute from monitoring zone '{0}' to {1}", monitoringZone.Label, target);
                    TraceRoute traceRoute = await provider.PerformTraceRouteFromMonitoringZoneAsync(monitoringZone.Id, configuration, cancellationTokenSource.Token);
                    Console.WriteLine("  Total Hops: {0}", traceRoute.Hops.Count);
                    for (int i = 0; i < traceRoute.Hops.Count; i++)
                    {
                        Assert.AreEqual(i + 1, traceRoute.Hops[i].Number);
                        Console.WriteLine("  {0}. {1}", traceRoute.Hops[i].Number, traceRoute.Hops[i].IPAddress);
                        foreach (TimeSpan responseTime in traceRoute.Hops[i].ResponseTimes)
                            Console.WriteLine("    {0}", responseTime.TotalSeconds);
                    }

                    break;
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestListAlarmNotificationHistory()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                bool foundHistoryItem = false;
                foreach (Entity entity in await ListAllEntitiesAsync(provider, null, cancellationTokenSource.Token))
                {
                    foreach (Alarm alarm in await ListAllAlarmsAsync(provider, entity.Id, null, cancellationTokenSource.Token))
                    {
                        foreach (CheckId checkId in await provider.DiscoverAlarmNotificationHistoryAsync(entity.Id, alarm.Id, cancellationTokenSource.Token))
                        {
                            AlarmNotificationHistoryItem[] alarmNotificationHistory = await ListAllAlarmNotificationHistoryAsync(provider, entity.Id, alarm.Id, checkId, null, null, null, cancellationTokenSource.Token);
                            foundHistoryItem |= alarmNotificationHistory.Any();
                            foreach (AlarmNotificationHistoryItem item in alarmNotificationHistory)
                                Console.WriteLine("Alarm notification history item '{0}'", item.Id);
                        }
                    }
                }

                if (!foundHistoryItem)
                    Assert.Inconclusive("The provider did not return any alarm notification history items.");
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestGetAlarmNotificationHistorySequential()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                bool foundHistoryItem = false;
                foreach (Entity entity in await ListAllEntitiesAsync(provider, null, cancellationTokenSource.Token))
                {
                    foreach (Alarm alarm in await ListAllAlarmsAsync(provider, entity.Id, null, cancellationTokenSource.Token))
                    {
                        foreach (CheckId checkId in await provider.DiscoverAlarmNotificationHistoryAsync(entity.Id, alarm.Id, cancellationTokenSource.Token))
                        {
                            AlarmNotificationHistoryItem[] alarmNotificationHistory = await ListAllAlarmNotificationHistoryAsync(provider, entity.Id, alarm.Id, checkId, null, null, null, cancellationTokenSource.Token);
                            foundHistoryItem |= alarmNotificationHistory.Any();
                            foreach (AlarmNotificationHistoryItem item in alarmNotificationHistory)
                            {
                                AlarmNotificationHistoryItem singleItem = await provider.GetAlarmNotificationHistoryAsync(entity.Id, alarm.Id, checkId, item.Id, cancellationTokenSource.Token);
                                Assert.IsNotNull(singleItem);
                                Assert.AreEqual(item.Id, singleItem.Id);
                                Assert.AreEqual(item.NotificationPlanId, singleItem.NotificationPlanId);
                                Assert.AreEqual(item.PreviousState, singleItem.PreviousState);
                                Assert.AreEqual(item.State, singleItem.State);
                                Assert.AreEqual(item.Status, singleItem.Status);
                                Assert.AreEqual(item.Timestamp, singleItem.Timestamp);
                                Assert.AreEqual(item.TransactionId, singleItem.TransactionId);
                                if (item.Results == null)
                                {
                                    Assert.IsNull(singleItem.Results);
                                }
                                else
                                {
                                    Assert.AreEqual(item.Results.Count, singleItem.Results.Count);
                                    for (int i = 0; i < item.Results.Count; i++)
                                    {
                                        Assert.AreEqual(item.Results[i].NotificationId, singleItem.Results[i].NotificationId);
                                        Assert.AreEqual(item.Results[i].NotificationTypeId, singleItem.Results[i].NotificationTypeId);
                                        Assert.AreEqual(item.Results[i].InProgress, singleItem.Results[i].InProgress);
                                        Assert.AreEqual(item.Results[i].Success, singleItem.Results[i].Success);
                                        Assert.AreEqual(item.Results[i].Message, singleItem.Results[i].Message);
                                    }
                                }
                            }
                        }
                    }
                }

                if (!foundHistoryItem)
                    Assert.Inconclusive("The provider did not return any alarm notification history items.");
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestGetAlarmNotificationHistory()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                // force authentication before starting the timer
                await provider.ListMonitoringZonesAsync(null, null, cancellationTokenSource.Token);

                Stopwatch stopwatch = Stopwatch.StartNew();

                List<Task<bool>> tasks = new List<Task<bool>>();
                foreach (Entity entity in await ListAllEntitiesAsync(provider, null, cancellationTokenSource.Token))
                {
                    tasks.Add(TestGetAlarmNotificationHistory(provider, entity, cancellationTokenSource.Token));
                }

                if (tasks.Count > 0)
                    await Task.Factory.ContinueWhenAll((Task[])tasks.ToArray(), TaskExtrasExtensions.PropagateExceptions);

                bool foundHistoryItem = tasks.Any(i => i.Result);

                Console.WriteLine("Elapsed time: {0}ms", stopwatch.ElapsedMilliseconds);

                if (!foundHistoryItem)
                    Assert.Inconclusive("The provider did not return any alarm notification history items.");
            }
        }

        private async Task<bool> TestGetAlarmNotificationHistory(IMonitoringService provider, Entity entity, CancellationToken cancellationToken)
        {
            List<Task<bool>> tasks = new List<Task<bool>>();
            foreach (Alarm alarm in await ListAllAlarmsAsync(provider, entity.Id, null, cancellationToken))
            {
                tasks.Add(TestGetAlarmNotificationHistory(provider, entity, alarm, cancellationToken));
            }

            if (tasks.Count > 0)
                await Task.Factory.ContinueWhenAll((Task[])tasks.ToArray(), TaskExtrasExtensions.PropagateExceptions);

            return tasks.Any(i => i.Result);
        }

        private async Task<bool> TestGetAlarmNotificationHistory(IMonitoringService provider, Entity entity, Alarm alarm, CancellationToken cancellationToken)
        {
            List<Task<bool>> tasks = new List<Task<bool>>();
            foreach (CheckId checkId in await provider.DiscoverAlarmNotificationHistoryAsync(entity.Id, alarm.Id, cancellationToken))
            {
                tasks.Add(TestGetAlarmNotificationHistory(provider, entity, alarm, checkId, cancellationToken));
            }

            if (tasks.Count > 0)
                await Task.Factory.ContinueWhenAll((Task[])tasks.ToArray(), TaskExtrasExtensions.PropagateExceptions);

            return tasks.Any(i => i.Result);
        }

        private async Task<bool> TestGetAlarmNotificationHistory(IMonitoringService provider, Entity entity, Alarm alarm, CheckId checkId, CancellationToken cancellationToken)
        {
            List<Task<bool>> tasks = new List<Task<bool>>();
            AlarmNotificationHistoryItem[] alarmNotificationHistory = await ListAllAlarmNotificationHistoryAsync(provider, entity.Id, alarm.Id, checkId, null, null, null, cancellationToken);
            foreach (AlarmNotificationHistoryItem item in alarmNotificationHistory)
            {
                tasks.Add(TestGetAlarmNotificationHistory(provider, entity, alarm, checkId, item, cancellationToken));
            }

            if (tasks.Count > 0)
                await Task.Factory.ContinueWhenAll((Task[])tasks.ToArray(), TaskExtrasExtensions.PropagateExceptions);

            return tasks.Any(i => i.Result);
        }

        private async Task<bool> TestGetAlarmNotificationHistory(IMonitoringService provider, Entity entity, Alarm alarm, CheckId checkId, AlarmNotificationHistoryItem item, CancellationToken cancellationToken)
        {
            AlarmNotificationHistoryItem singleItem = await provider.GetAlarmNotificationHistoryAsync(entity.Id, alarm.Id, checkId, item.Id, cancellationToken);
            Assert.IsNotNull(singleItem);
            Assert.AreEqual(item.Id, singleItem.Id);
            Assert.AreEqual(item.NotificationPlanId, singleItem.NotificationPlanId);
            Assert.AreEqual(item.PreviousState, singleItem.PreviousState);
            Assert.AreEqual(item.State, singleItem.State);
            Assert.AreEqual(item.Status, singleItem.Status);
            Assert.AreEqual(item.Timestamp, singleItem.Timestamp);
            Assert.AreEqual(item.TransactionId, singleItem.TransactionId);
            if (item.Results == null)
            {
                Assert.IsNull(singleItem.Results);
            }
            else
            {
                Assert.AreEqual(item.Results.Count, singleItem.Results.Count);
                for (int i = 0; i < item.Results.Count; i++)
                {
                    Assert.AreEqual(item.Results[i].NotificationId, singleItem.Results[i].NotificationId);
                    Assert.AreEqual(item.Results[i].NotificationTypeId, singleItem.Results[i].NotificationTypeId);
                    Assert.AreEqual(item.Results[i].InProgress, singleItem.Results[i].InProgress);
                    Assert.AreEqual(item.Results[i].Success, singleItem.Results[i].Success);
                    Assert.AreEqual(item.Results[i].Message, singleItem.Results[i].Message);
                }
            }

            return true;
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestCreateWebhookNotification()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string label = CreateRandomNotificationName();
                NotificationTypeId notificationTypeId = NotificationTypeId.Webhook;
                NotificationDetails notificationDetails = new WebhookNotificationDetails(new Uri("http://example.com"));
                NewNotificationConfiguration configuration = new NewNotificationConfiguration(label, notificationTypeId, notificationDetails);
                NotificationId notificationId = await provider.CreateNotificationAsync(configuration, cancellationTokenSource.Token);

                Notification notification = await provider.GetNotificationAsync(notificationId, cancellationTokenSource.Token);
                Assert.IsNotNull(notification);
                Assert.AreEqual(notificationId, notification.Id);
                Assert.AreEqual(label, notification.Label);
                Assert.AreEqual(configuration.Type, notification.Type);

                await provider.RemoveNotificationAsync(notificationId, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestCreateEmailNotification()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string label = CreateRandomNotificationName();
                NotificationTypeId notificationTypeId = NotificationTypeId.Email;
                NotificationDetails notificationDetails = new EmailNotificationDetails("sample@example.com");
                NewNotificationConfiguration configuration = new NewNotificationConfiguration(label, notificationTypeId, notificationDetails);
                NotificationId notificationId = await provider.CreateNotificationAsync(configuration, cancellationTokenSource.Token);

                Notification notification = await provider.GetNotificationAsync(notificationId, cancellationTokenSource.Token);
                Assert.IsNotNull(notification);
                Assert.AreEqual(notificationId, notification.Id);
                Assert.AreEqual(label, notification.Label);
                Assert.AreEqual(configuration.Type, notification.Type);

                await provider.RemoveNotificationAsync(notificationId, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestCreatePagerDutyNotification()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string label = CreateRandomNotificationName();
                NotificationTypeId notificationTypeId = NotificationTypeId.PagerDuty;
                NotificationDetails notificationDetails = new PagerDutyNotificationDetails("XXXXX");
                NewNotificationConfiguration configuration = new NewNotificationConfiguration(label, notificationTypeId, notificationDetails);
                NotificationId notificationId = await provider.CreateNotificationAsync(configuration, cancellationTokenSource.Token);

                Notification notification = await provider.GetNotificationAsync(notificationId, cancellationTokenSource.Token);
                Assert.IsNotNull(notification);
                Assert.AreEqual(notificationId, notification.Id);
                Assert.AreEqual(label, notification.Label);
                Assert.AreEqual(configuration.Type, notification.Type);

                await provider.RemoveNotificationAsync(notificationId, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestTestWebhookNotification()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string label = CreateRandomNotificationName();
                NotificationTypeId notificationTypeId = NotificationTypeId.Webhook;
                NotificationDetails notificationDetails = new WebhookNotificationDetails(new Uri("http://example.com"));
                NewNotificationConfiguration configuration = new NewNotificationConfiguration(label, notificationTypeId, notificationDetails);
                NotificationData notificationData = await provider.TestNotificationAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(notificationData);
                Assert.AreEqual("success", notificationData.Status);
                Assert.AreEqual("Success: Webhook successfully executed", notificationData.Message);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestTestEmailNotification()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string label = CreateRandomNotificationName();
                NotificationTypeId notificationTypeId = NotificationTypeId.Email;
                NotificationDetails notificationDetails = new EmailNotificationDetails("sample@example.com");
                NewNotificationConfiguration configuration = new NewNotificationConfiguration(label, notificationTypeId, notificationDetails);
                NotificationData notificationData = await provider.TestNotificationAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(notificationData);
                Assert.AreEqual("success", notificationData.Status);
                Assert.AreEqual("Email successfully sent", notificationData.Message);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestTestPagerDutyNotification()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string label = CreateRandomNotificationName();
                NotificationTypeId notificationTypeId = NotificationTypeId.PagerDuty;
                NotificationDetails notificationDetails = new PagerDutyNotificationDetails("XXXXX");
                NewNotificationConfiguration configuration = new NewNotificationConfiguration(label, notificationTypeId, notificationDetails);
                NotificationData notificationData = await provider.TestNotificationAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(notificationData);
                Assert.AreEqual("error", notificationData.Status);
                Assert.AreEqual("Unexpected status code \"400\". Expected one of: /2[0-9]{2}/", notificationData.Message);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestTestExistingWebhookNotification()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string label = CreateRandomNotificationName();
                NotificationTypeId notificationTypeId = NotificationTypeId.Webhook;
                NotificationDetails notificationDetails = new WebhookNotificationDetails(new Uri("http://example.com"));
                NewNotificationConfiguration configuration = new NewNotificationConfiguration(label, notificationTypeId, notificationDetails);
                NotificationId notificationId = await provider.CreateNotificationAsync(configuration, cancellationTokenSource.Token);

                NotificationData notificationData = await provider.TestExistingNotificationAsync(notificationId, cancellationTokenSource.Token);
                Assert.IsNotNull(notificationData);
                Assert.AreEqual("success", notificationData.Status);
                Assert.AreEqual("Success: Webhook successfully executed", notificationData.Message);

                await provider.RemoveNotificationAsync(notificationId, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestTestExistingEmailNotification()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string label = CreateRandomNotificationName();
                NotificationTypeId notificationTypeId = NotificationTypeId.Email;
                NotificationDetails notificationDetails = new EmailNotificationDetails("sample@example.com");
                NewNotificationConfiguration configuration = new NewNotificationConfiguration(label, notificationTypeId, notificationDetails);
                NotificationId notificationId = await provider.CreateNotificationAsync(configuration, cancellationTokenSource.Token);

                NotificationData notificationData = await provider.TestExistingNotificationAsync(notificationId, cancellationTokenSource.Token);
                Assert.IsNotNull(notificationData);
                Assert.AreEqual("success", notificationData.Status);
                Assert.AreEqual("Email successfully sent", notificationData.Message);

                await provider.RemoveNotificationAsync(notificationId, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestTestExistingPagerDutyNotification()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string label = CreateRandomNotificationName();
                NotificationTypeId notificationTypeId = NotificationTypeId.PagerDuty;
                NotificationDetails notificationDetails = new PagerDutyNotificationDetails("XXXXX");
                NewNotificationConfiguration configuration = new NewNotificationConfiguration(label, notificationTypeId, notificationDetails);
                NotificationId notificationId = await provider.CreateNotificationAsync(configuration, cancellationTokenSource.Token);

                NotificationData notificationData = await provider.TestExistingNotificationAsync(notificationId, cancellationTokenSource.Token);
                Assert.IsNotNull(notificationData);
                Assert.AreEqual("error", notificationData.Status);
                Assert.AreEqual("Unexpected status code \"400\". Expected one of: /2[0-9]{2}/", notificationData.Message);

                await provider.RemoveNotificationAsync(notificationId, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestListNotifications()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string label = CreateRandomNotificationName();
                NotificationTypeId notificationTypeId = NotificationTypeId.Webhook;
                NotificationDetails notificationDetails = new WebhookNotificationDetails(new Uri("http://example.com"));
                NewNotificationConfiguration configuration = new NewNotificationConfiguration(label, notificationTypeId, notificationDetails);
                NotificationId notificationId = await provider.CreateNotificationAsync(configuration, cancellationTokenSource.Token);

                Notification[] notifications = await ListAllNotificationsAsync(provider, null, cancellationTokenSource.Token);
                Assert.IsNotNull(notifications);
                Assert.IsTrue(notifications.Length > 0);

                foreach (Notification notification in notifications)
                    Console.WriteLine("Notification '{0}' ({1})", notification.Label, notification.Id);

                await provider.RemoveNotificationAsync(notificationId, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestGetNotification()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string label = CreateRandomNotificationName();
                NotificationTypeId notificationTypeId = NotificationTypeId.Webhook;
                NotificationDetails notificationDetails = new WebhookNotificationDetails(new Uri("http://example.com"));
                NewNotificationConfiguration configuration = new NewNotificationConfiguration(label, notificationTypeId, notificationDetails);
                NotificationId notificationId = await provider.CreateNotificationAsync(configuration, cancellationTokenSource.Token);

                Notification[] notifications = await ListAllNotificationsAsync(provider, null, cancellationTokenSource.Token);
                Assert.IsNotNull(notifications);
                Assert.IsTrue(notifications.Length > 0);

                foreach (Notification notification in notifications)
                {
                    Notification singleNotification = await provider.GetNotificationAsync(notification.Id, cancellationTokenSource.Token);
                    Assert.IsNotNull(singleNotification);
                    Assert.AreEqual(notification.Id, singleNotification.Id);
                    Assert.AreEqual(notification.Label, singleNotification.Label);
                    Assert.AreEqual(notification.Type, singleNotification.Type);

                    if (notification.Type != null)
                    {
                        NotificationType notificationType = await provider.GetNotificationTypeAsync(notification.Type, cancellationTokenSource.Token);
                        Assert.IsNotNull(notificationType);
                    }
                }

                await provider.RemoveNotificationAsync(notificationId, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestUpdateNotification()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string label = CreateRandomNotificationName();
                NotificationTypeId notificationTypeId = NotificationTypeId.Webhook;
                NotificationDetails notificationDetails = new WebhookNotificationDetails(new Uri("http://example.com"));
                NewNotificationConfiguration configuration = new NewNotificationConfiguration(label, notificationTypeId, notificationDetails);
                NotificationId notificationId = await provider.CreateNotificationAsync(configuration, cancellationTokenSource.Token);

                Notification notification = await provider.GetNotificationAsync(notificationId, cancellationTokenSource.Token);
                Assert.IsNotNull(notification);
                Assert.AreEqual(notificationId, notification.Id);
                Assert.AreEqual(label, notification.Label);
                Assert.AreEqual(configuration.Type, notification.Type);

                string updatedLabel = CreateRandomNotificationName();
                UpdateNotificationConfiguration updateConfiguration = new UpdateNotificationConfiguration(label: updatedLabel);
                await provider.UpdateNotificationAsync(notificationId, updateConfiguration, cancellationTokenSource.Token);

                Notification updateNotification = await provider.GetNotificationAsync(notificationId, cancellationTokenSource.Token);
                Assert.IsNotNull(updateNotification);
                Assert.AreEqual(notificationId, updateNotification.Id);
                Assert.AreEqual(updatedLabel, updateNotification.Label);
                Assert.AreEqual(configuration.Type, updateNotification.Type);

                await provider.RemoveNotificationAsync(notificationId, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestListNotificationTypes()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                NotificationType[] notifications = await ListAllNotificationTypesAsync(provider, null, cancellationTokenSource.Token);
                if (notifications.Length == 0)
                    Assert.Inconclusive("The service did not report any notification types.");

                foreach (NotificationType notificationType in notifications)
                {
                    Console.WriteLine("Notification '{0}'", notificationType.Id);
                    foreach (NotificationTypeField field in notificationType.Fields)
                        Console.WriteLine("    {0}{1} // {2}", field.Name, (field.Optional ?? false) ? " (optional)" : string.Empty, field.Description);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestGetNotificationType()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                NotificationType[] notifications = await ListAllNotificationTypesAsync(provider, null, cancellationTokenSource.Token);
                if (notifications.Length == 0)
                    Assert.Inconclusive("The service did not report any notification types.");

                foreach (NotificationType notificationType in notifications)
                {
                    NotificationType singleNotificationType = await provider.GetNotificationTypeAsync(notificationType.Id, cancellationTokenSource.Token);
                    Assert.IsNotNull(singleNotificationType);
                    Assert.AreEqual(notificationType.Id, singleNotificationType.Id);
                    if (notificationType.Fields == null)
                    {
                        Assert.IsNull(singleNotificationType.Fields);
                    }
                    else
                    {
                        Assert.AreEqual(notificationType.Fields.Count, singleNotificationType.Fields.Count);
                        for (int i = 0; i < notificationType.Fields.Count; i++)
                        {
                            Assert.AreEqual(notificationType.Fields[i].Name, singleNotificationType.Fields[i].Name);
                            Assert.AreEqual(notificationType.Fields[i].Optional, singleNotificationType.Fields[i].Optional);
                            Assert.AreEqual(notificationType.Fields[i].Description, singleNotificationType.Fields[i].Description);
                        }
                    }
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestListAlarmChangelogs()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                AlarmChangelog[] alarmChangelogs = await ListAllAlarmChangelogsAsync(provider, null, null, null, cancellationTokenSource.Token);
                if (alarmChangelogs.Length == 0)
                    Assert.Inconclusive("The service did not report any alarm changelogs.");

                foreach (AlarmChangelog alarmChangelog in alarmChangelogs)
                    Console.WriteLine("Alarm changelog '{0}'", alarmChangelog.Id);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestListAlarmChangelogsWithEntityFilter()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                AlarmChangelog[] alarmChangelogs = await ListAllAlarmChangelogsAsync(provider, null, null, null, cancellationTokenSource.Token);
                if (alarmChangelogs.Length == 0)
                    Assert.Inconclusive("The service did not report any alarm changelogs.");

                ILookup<EntityId, AlarmChangelog> lookup = alarmChangelogs.ToLookup(i => i.EntityId);
                foreach (IGrouping<EntityId, AlarmChangelog> group in lookup)
                {
                    AlarmChangelog[] groupAlarmChangelogs = group.ToArray();
                    AlarmChangelog[] entityAlarmChangelogs = await ListAllAlarmChangelogsAsync(provider, group.Key, null, null, null, cancellationTokenSource.Token);
                    Assert.AreEqual(groupAlarmChangelogs.Length, entityAlarmChangelogs.Length);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestListEntityOverviews()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                EntityOverview[] entityOverviews = await ListAllEntityOverviewsAsync(provider, null, cancellationTokenSource.Token);
                if (entityOverviews.Length == 0)
                    Assert.Inconclusive("The service did not report any overview views.");

                foreach (EntityOverview entity in entityOverviews)
                {
                    Console.WriteLine("Entity {0} ({1})", entity.Entity.Label, entity.Entity.Id);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestListEntityOverviewsWithEntityFilter()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                ReadOnlyCollection<Entity> entities = await provider.ListEntitiesAsync(null, 7, cancellationTokenSource.Token);
                if (entities.Count == 0)
                    Assert.Inconclusive("The service did not report any entities");
                Assert.IsTrue(entities.Count <= 7);

                Entity[] filteredEntities = entities.Skip(Math.Min(4, entities.Count - 1)).Take(3).ToArray();
                Console.WriteLine("Filtering result to the following {0} entities:", filteredEntities.Length);
                foreach (Entity entity in filteredEntities)
                    Console.WriteLine("    {0} ({1})", entity.Label, entity.Id);

                EntityOverview[] entityOverviews = await ListAllEntityOverviewsAsync(provider, null, filteredEntities.Select(i => i.Id), cancellationTokenSource.Token);
                if (entityOverviews.Length == 0)
                    Assert.Inconclusive("The service did not report any overview views.");

                Console.WriteLine();
                Console.WriteLine("Results:");
                foreach (EntityOverview entity in entityOverviews)
                {
                    Assert.IsNotNull(entity);
                    Assert.AreEqual(1, filteredEntities.Count(i => i.Id == entity.Entity.Id));
                    Console.WriteLine("    Entity {0} ({1})", entity.Entity.Label, entity.Entity.Id);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestListAlarmExamples()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                AlarmExample[] alarmExamples = await ListAllAlarmExamplesAsync(provider, null, cancellationTokenSource.Token);
                if (alarmExamples.Length == 0)
                    Assert.Inconclusive("The provider did not return any alarm examples.");

                foreach (AlarmExample alarmExample in alarmExamples)
                    Console.WriteLine(alarmExample.Label);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestGetAlarmExample()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                AlarmExample[] alarmExamples = await ListAllAlarmExamplesAsync(provider, null, cancellationTokenSource.Token);
                foreach (AlarmExample alarmExample in alarmExamples)
                {
                    AlarmExample example = await provider.GetAlarmExampleAsync(alarmExample.Id, cancellationTokenSource.Token);
                    Assert.IsNotNull(example);
                    Assert.AreEqual(alarmExample.Id, example.Id);
                    Assert.AreEqual(alarmExample.Label, example.Label);
                    Assert.AreEqual(alarmExample.Description, example.Description);
                    Assert.AreEqual(alarmExample.Criteria, example.Criteria);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestEvaluateAlarmExample()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                AlarmExample[] alarmExamples = await ListAllAlarmExamplesAsync(provider, null, cancellationTokenSource.Token);
                if (alarmExamples.Length == 0)
                    Assert.Inconclusive("The provider did not return any alarm examples.");

                List<Task> tasks = new List<Task>();
                foreach (AlarmExample alarmExample in alarmExamples)
                    tasks.Add(TestEvaluateAlarmExampleAsync(provider, alarmExample, cancellationTokenSource.Token));

                await Task.Factory.ContinueWhenAll(tasks.ToArray(), TaskExtrasExtensions.PropagateExceptions);
            }
        }

        private async Task TestEvaluateAlarmExampleAsync(IMonitoringService provider, AlarmExample alarmExample, CancellationToken cancellationToken)
        {
            string expected = alarmExample.Criteria;
            Dictionary<string, object> exampleParameters = new Dictionary<string, object>();
            foreach (var field in alarmExample.Fields)
            {
                object value;
                switch (field.Type)
                {
                case "string":
                    value = field.Name;
                    break;

                case "integer":
                case "whole number (may be zero padded)":
                    value = 3;
                    break;

                default:
                    throw new NotImplementedException(string.Format("Integration test support for fields of type '{0}' is not yet implemented.", field.Type));
                }

                expected = expected.Replace("${" + field.Name + "}", value.ToString());
                exampleParameters.Add(field.Name, value);
            }

            BoundAlarmExample boundAlarmExample = await provider.EvaluateAlarmExampleAsync(alarmExample.Id, exampleParameters, cancellationToken);
            Assert.AreEqual(expected, boundAlarmExample.BoundCriteria);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestListAgents()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                Agent[] agents = await ListAllAgentsAsync(provider, null, cancellationTokenSource.Token);
                if (agents.Length == 0)
                    Assert.Inconclusive("The service did not report any agents.");

                foreach (Agent agent in agents)
                    Console.WriteLine("Agent {0} last connected {1}", agent.Id, agent.LastConnected);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestGetAgent()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                Agent[] agents = await ListAllAgentsAsync(provider, null, cancellationTokenSource.Token);
                if (agents.Length == 0)
                    Assert.Inconclusive("The service did not report any agents.");

                foreach (Agent agent in agents)
                {
                    Agent singleAgent = await provider.GetAgentAsync(agent.Id, cancellationTokenSource.Token);
                    Assert.IsNotNull(singleAgent);
                    Assert.AreEqual(agent.Id, singleAgent.Id);
                    Assert.IsTrue(agent.LastConnected <= singleAgent.LastConnected);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestListAgentConnections()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                Agent[] agents = await ListAllAgentsAsync(provider, null, cancellationTokenSource.Token);
                if (agents.Length == 0)
                    Assert.Inconclusive("The service did not report any agents.");

                bool foundConnection = false;
                foreach (Agent agent in agents)
                {
                    Console.WriteLine("Connections for Agent {0}", agent.Id);
                    AgentConnection[] connections = await ListAllAgentConnectionsAsync(provider, agent.Id, null, cancellationTokenSource.Token);
                    foundConnection |= connections.Any();
                    foreach (AgentConnection connection in connections)
                    {
                        Assert.AreEqual(agent.Id, connection.AgentId);
                        Console.WriteLine("    {0}", connection.Id);
                    }
                }

                if (!foundConnection)
                    Assert.Inconclusive("The service did not report any agent connections.");
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestGetAgentConnection()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                Agent[] agents = await ListAllAgentsAsync(provider, null, cancellationTokenSource.Token);
                if (agents.Length == 0)
                    Assert.Inconclusive("The service did not report any agents.");

                bool foundConnection = false;
                foreach (Agent agent in agents)
                {
                    AgentConnection[] connections = await ListAllAgentConnectionsAsync(provider, agent.Id, null, cancellationTokenSource.Token);
                    foundConnection |= connections.Any();
                    foreach (AgentConnection connection in connections)
                    {
                        AgentConnection singleConnection = await provider.GetAgentConnectionAsync(agent.Id, connection.Id, cancellationTokenSource.Token);
                        Assert.IsNotNull(singleConnection);
                        Assert.AreEqual(connection.Id, singleConnection.Id);
                        Assert.AreEqual(connection.Guid, singleConnection.Guid);
                        Assert.AreEqual(connection.ProcessVersion, singleConnection.ProcessVersion);
                        Assert.AreEqual(connection.Endpoint, singleConnection.Endpoint);
                        Assert.AreEqual(connection.BundleVersion, singleConnection.BundleVersion);
                        Assert.AreEqual(connection.AgentAddress, singleConnection.AgentAddress);
                        Assert.AreEqual(connection.AgentId, singleConnection.AgentId);
                    }
                }

                if (!foundConnection)
                    Assert.Inconclusive("The service did not report any agent connections.");
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestCreateAgentToken()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string label = CreateRandomAgentTokenName();
                AgentTokenConfiguration configuration = new AgentTokenConfiguration(label);

                AgentTokenId agentTokenId = await provider.CreateAgentTokenAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(agentTokenId);

                AgentToken agentToken = await provider.GetAgentTokenAsync(agentTokenId, cancellationTokenSource.Token);
                Assert.IsNotNull(agentToken);
                Assert.AreEqual(agentTokenId, agentToken.Id);
                Assert.AreEqual(label, agentToken.Label);

                await provider.RemoveAgentTokenAsync(agentTokenId, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestListAgentTokens()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                AgentToken[] agentTokens = await ListAllAgentTokensAsync(provider, null, cancellationTokenSource.Token);
                if (agentTokens.Length == 0)
                    Assert.Inconclusive("The service did not report any agent tokens.");

                foreach (AgentToken agentToken in agentTokens)
                    Console.WriteLine("Agent Token {0} ({1})", agentToken.Label, agentToken.Id);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestUpdateAgentToken()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                string label = CreateRandomAgentTokenName();
                AgentTokenConfiguration configuration = new AgentTokenConfiguration(label);

                AgentTokenId agentTokenId = await provider.CreateAgentTokenAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(agentTokenId);

                AgentToken agentToken = await provider.GetAgentTokenAsync(agentTokenId, cancellationTokenSource.Token);
                Assert.IsNotNull(agentToken);
                Assert.AreEqual(agentTokenId, agentToken.Id);
                Assert.AreEqual(label, agentToken.Label);

                string updatedLabel = CreateRandomAgentTokenName();
                AgentTokenConfiguration updateConfiguration = new AgentTokenConfiguration(updatedLabel);
                await provider.UpdateAgentTokenAsync(agentTokenId, updateConfiguration, cancellationTokenSource.Token);

                AgentToken updatedAgentToken = await provider.GetAgentTokenAsync(agentTokenId, cancellationTokenSource.Token);
                Assert.IsNotNull(updatedAgentToken);
                Assert.AreEqual(updatedLabel, updatedAgentToken.Label);
                Assert.AreEqual(agentToken.Id, updatedAgentToken.Id);
                Assert.AreEqual(agentToken.Token, updatedAgentToken.Token);

                await provider.RemoveAgentTokenAsync(agentTokenId, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestGetAgentHostInformation()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                Agent[] agents = await ListAllAgentsAsync(provider, null, cancellationTokenSource.Token);
                if (agents.Length == 0)
                    Assert.Inconclusive("The service did not report any agents.");

                List<Task<string>> tasks = new List<Task<string>>();
                foreach (Agent agent in agents)
                    tasks.Add(TestGetAgentHostInformation(provider, agent.Id, cancellationTokenSource.Token));

                await Task.Factory.ContinueWhenAll((Task[])tasks.ToArray(), TaskExtrasExtensions.PropagateExceptions);

                foreach (Task<string> task in tasks)
                {
                    if (string.IsNullOrEmpty(task.Result))
                        continue;

                    Console.WriteLine(task.Result);
                }
            }
        }

        private async Task<string> TestGetAgentHostInformation(IMonitoringService provider, AgentId agentId, CancellationToken cancellationToken)
        {
            ReadOnlyCollection<AgentConnection> connectionCheck = await provider.ListAgentConnectionsAsync(agentId, null, 1, cancellationToken);
            if (connectionCheck.Count == 0)
                return string.Empty;

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("Agent host '{0}' information for agent {1}", HostInformationType.Cpus, agentId));
            HostInformation<JToken> information = await provider.GetAgentHostInformationAsync(agentId, HostInformationType.Cpus, cancellationToken);
            Assert.IsNotNull(information);
            foreach (CpuInformation cpuInformation in information.Info.ToObject<CpuInformation[]>())
                builder.AppendLine(string.Format("    {0} ({1} MHz)", cpuInformation.Model, cpuInformation.Frequency));

            return builder.ToString();
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestGetCpuInformation()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                Agent[] agents = await ListAllAgentsAsync(provider, null, cancellationTokenSource.Token);
                if (agents.Length == 0)
                    Assert.Inconclusive("The service did not report any agents.");

                List<Task<string>> tasks = new List<Task<string>>();
                foreach (Agent agent in agents)
                    tasks.Add(TestGetCpuInformation(provider, agent.Id, cancellationTokenSource.Token));

                await Task.Factory.ContinueWhenAll((Task[])tasks.ToArray(), TaskExtrasExtensions.PropagateExceptions);

                foreach (Task<string> task in tasks)
                {
                    if (string.IsNullOrEmpty(task.Result))
                        continue;

                    Console.WriteLine(task.Result);
                }
            }
        }

        private async Task<string> TestGetCpuInformation(IMonitoringService provider, AgentId agentId, CancellationToken cancellationToken)
        {
            ReadOnlyCollection<AgentConnection> connectionCheck = await provider.ListAgentConnectionsAsync(agentId, null, 1, cancellationToken);
            if (connectionCheck.Count == 0)
                return string.Empty;

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("CPU information for agent {0}", agentId));
            HostInformation<ReadOnlyCollection<CpuInformation>> information = await provider.GetCpuInformationAsync(agentId, cancellationToken);
            Assert.IsNotNull(information);
            foreach (CpuInformation cpuInformation in information.Info)
                builder.AppendLine(string.Format("    {0} ({1} MHz)", cpuInformation.Model, cpuInformation.Frequency));

            return builder.ToString();
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestGetDiskInformation()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                Agent[] agents = await ListAllAgentsAsync(provider, null, cancellationTokenSource.Token);
                if (agents.Length == 0)
                    Assert.Inconclusive("The service did not report any agents.");

                List<Task<string>> tasks = new List<Task<string>>();
                foreach (Agent agent in agents)
                    tasks.Add(TestGetDiskInformation(provider, agent.Id, cancellationTokenSource.Token));

                await Task.Factory.ContinueWhenAll((Task[])tasks.ToArray(), TaskExtrasExtensions.PropagateExceptions);

                foreach (Task<string> task in tasks)
                {
                    if (string.IsNullOrEmpty(task.Result))
                        continue;

                    Console.WriteLine(task.Result);
                }
            }
        }

        private async Task<string> TestGetDiskInformation(IMonitoringService provider, AgentId agentId, CancellationToken cancellationToken)
        {
            ReadOnlyCollection<AgentConnection> connectionCheck = await provider.ListAgentConnectionsAsync(agentId, null, 1, cancellationToken);
            if (connectionCheck.Count == 0)
                return string.Empty;

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("Disk information for agent {0}", agentId));
            HostInformation<ReadOnlyCollection<DiskInformation>> information = await provider.GetDiskInformationAsync(agentId, cancellationToken);
            Assert.IsNotNull(information);
            foreach (DiskInformation diskInformation in information.Info)
                builder.AppendLine(string.Format("    {0}", diskInformation.Name));

            return builder.ToString();
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestGetFilesystemInformation()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                Agent[] agents = await ListAllAgentsAsync(provider, null, cancellationTokenSource.Token);
                if (agents.Length == 0)
                    Assert.Inconclusive("The service did not report any agents.");

                List<Task<string>> tasks = new List<Task<string>>();
                foreach (Agent agent in agents)
                    tasks.Add(TestGetFilesystemInformation(provider, agent.Id, cancellationTokenSource.Token));

                await Task.Factory.ContinueWhenAll((Task[])tasks.ToArray(), TaskExtrasExtensions.PropagateExceptions);

                foreach (Task<string> task in tasks)
                {
                    if (string.IsNullOrEmpty(task.Result))
                        continue;

                    Console.WriteLine(task.Result);
                }
            }
        }

        private async Task<string> TestGetFilesystemInformation(IMonitoringService provider, AgentId agentId, CancellationToken cancellationToken)
        {
            ReadOnlyCollection<AgentConnection> connectionCheck = await provider.ListAgentConnectionsAsync(agentId, null, 1, cancellationToken);
            if (connectionCheck.Count == 0)
                return string.Empty;

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("Filesystem information for agent {0}", agentId));
            HostInformation<ReadOnlyCollection<FilesystemInformation>> information = await provider.GetFilesystemInformationAsync(agentId, cancellationToken);
            Assert.IsNotNull(information);
            foreach (FilesystemInformation filesystemInformation in information.Info)
                builder.AppendLine(string.Format("    {0}", filesystemInformation));

            return builder.ToString();
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestGetMemoryInformation()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                Agent[] agents = await ListAllAgentsAsync(provider, null, cancellationTokenSource.Token);
                if (agents.Length == 0)
                    Assert.Inconclusive("The service did not report any agents.");

                List<Task<string>> tasks = new List<Task<string>>();
                foreach (Agent agent in agents)
                    tasks.Add(TestGetMemoryInformation(provider, agent.Id, cancellationTokenSource.Token));

                await Task.Factory.ContinueWhenAll((Task[])tasks.ToArray(), TaskExtrasExtensions.PropagateExceptions);

                foreach (Task<string> task in tasks)
                {
                    if (string.IsNullOrEmpty(task.Result))
                        continue;

                    Console.WriteLine(task.Result);
                }
            }
        }

        private async Task<string> TestGetMemoryInformation(IMonitoringService provider, AgentId agentId, CancellationToken cancellationToken)
        {
            ReadOnlyCollection<AgentConnection> connectionCheck = await provider.ListAgentConnectionsAsync(agentId, null, 1, cancellationToken);
            if (connectionCheck.Count == 0)
                return string.Empty;

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("Memory information for agent {0}", agentId));
            HostInformation<MemoryInformation> information = await provider.GetMemoryInformationAsync(agentId, cancellationToken);
            Assert.IsNotNull(information);
            builder.AppendLine(string.Format("    {0}", information.Info));

            return builder.ToString();
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestGetNetworkInterfaceInformation()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                Agent[] agents = await ListAllAgentsAsync(provider, null, cancellationTokenSource.Token);
                if (agents.Length == 0)
                    Assert.Inconclusive("The service did not report any agents.");

                List<Task<string>> tasks = new List<Task<string>>();
                foreach (Agent agent in agents)
                    tasks.Add(TestGetNetworkInterfaceInformation(provider, agent.Id, cancellationTokenSource.Token));

                await Task.Factory.ContinueWhenAll((Task[])tasks.ToArray(), TaskExtrasExtensions.PropagateExceptions);

                foreach (Task<string> task in tasks)
                {
                    if (string.IsNullOrEmpty(task.Result))
                        continue;

                    Console.WriteLine(task.Result);
                }
            }
        }

        private async Task<string> TestGetNetworkInterfaceInformation(IMonitoringService provider, AgentId agentId, CancellationToken cancellationToken)
        {
            ReadOnlyCollection<AgentConnection> connectionCheck = await provider.ListAgentConnectionsAsync(agentId, null, 1, cancellationToken);
            if (connectionCheck.Count == 0)
                return string.Empty;

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("Network interface information for agent {0}", agentId));
            HostInformation<ReadOnlyCollection<NetworkInterfaceInformation>> information = await provider.GetNetworkInterfaceInformationAsync(agentId, cancellationToken);
            Assert.IsNotNull(information);
            foreach (NetworkInterfaceInformation networkInterfaceInformation in information.Info)
                builder.AppendLine(string.Format("    {0}", networkInterfaceInformation.Name));

            return builder.ToString();
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestGetProcessInformation()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                Agent[] agents = await ListAllAgentsAsync(provider, null, cancellationTokenSource.Token);
                if (agents.Length == 0)
                    Assert.Inconclusive("The service did not report any agents.");

                List<Task<string>> tasks = new List<Task<string>>();
                foreach (Agent agent in agents)
                    tasks.Add(TestGetProcessInformation(provider, agent.Id, cancellationTokenSource.Token));

                await Task.Factory.ContinueWhenAll((Task[])tasks.ToArray(), TaskExtrasExtensions.PropagateExceptions);

                foreach (Task<string> task in tasks)
                {
                    if (string.IsNullOrEmpty(task.Result))
                        continue;

                    Console.WriteLine(task.Result);
                }
            }
        }

        private async Task<string> TestGetProcessInformation(IMonitoringService provider, AgentId agentId, CancellationToken cancellationToken)
        {
            ReadOnlyCollection<AgentConnection> connectionCheck = await provider.ListAgentConnectionsAsync(agentId, null, 1, cancellationToken);
            if (connectionCheck.Count == 0)
                return string.Empty;

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("Process information for agent {0}", agentId));
            HostInformation<ReadOnlyCollection<ProcessInformation>> information = await provider.GetProcessInformationAsync(agentId, cancellationToken);
            Assert.IsNotNull(information);
            foreach (ProcessInformation processInformation in information.Info)
                builder.AppendLine(string.Format("    {0}", processInformation.Name));

            return builder.ToString();
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestGetSystemInformation()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                Agent[] agents = await ListAllAgentsAsync(provider, null, cancellationTokenSource.Token);
                if (agents.Length == 0)
                    Assert.Inconclusive("The service did not report any agents.");

                List<Task<string>> tasks = new List<Task<string>>();
                foreach (Agent agent in agents)
                    tasks.Add(TestGetSystemInformation(provider, agent.Id, cancellationTokenSource.Token));

                await Task.Factory.ContinueWhenAll((Task[])tasks.ToArray(), TaskExtrasExtensions.PropagateExceptions);

                foreach (Task<string> task in tasks)
                {
                    if (string.IsNullOrEmpty(task.Result))
                        continue;

                    Console.WriteLine(task.Result);
                }
            }
        }

        private async Task<string> TestGetSystemInformation(IMonitoringService provider, AgentId agentId, CancellationToken cancellationToken)
        {
            ReadOnlyCollection<AgentConnection> connectionCheck = await provider.ListAgentConnectionsAsync(agentId, null, 1, cancellationToken);
            if (connectionCheck.Count == 0)
                return string.Empty;

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("System information for agent {0}", agentId));
            HostInformation<SystemInformation> information = await provider.GetSystemInformationAsync(agentId, cancellationToken);
            Assert.IsNotNull(information);
            builder.AppendLine(string.Format("    {0}", information.Info.Name));

            return builder.ToString();
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestGetLoginInformation()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                Agent[] agents = await ListAllAgentsAsync(provider, null, cancellationTokenSource.Token);
                if (agents.Length == 0)
                    Assert.Inconclusive("The service did not report any agents.");

                List<Task<string>> tasks = new List<Task<string>>();
                foreach (Agent agent in agents)
                    tasks.Add(TestGetLoginInformation(provider, agent.Id, cancellationTokenSource.Token));

                await Task.Factory.ContinueWhenAll((Task[])tasks.ToArray(), TaskExtrasExtensions.PropagateExceptions);

                foreach (Task<string> task in tasks)
                {
                    if (string.IsNullOrEmpty(task.Result))
                        continue;

                    Console.WriteLine(task.Result);
                }
            }
        }

        private async Task<string> TestGetLoginInformation(IMonitoringService provider, AgentId agentId, CancellationToken cancellationToken)
        {
            ReadOnlyCollection<AgentConnection> connectionCheck = await provider.ListAgentConnectionsAsync(agentId, null, 1, cancellationToken);
            if (connectionCheck.Count == 0)
                return string.Empty;

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("Login information for agent {0}", agentId));
            HostInformation<ReadOnlyCollection<LoginInformation>> information = await provider.GetLoginInformationAsync(agentId, cancellationToken);
            Assert.IsNotNull(information);
            foreach (LoginInformation loginInformation in information.Info)
                builder.AppendLine(string.Format("    {0}@{1}", loginInformation.User, loginInformation.Host));

            return builder.ToString();
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Monitoring)]
        public async Task TestListAgentCheckTargets()
        {
            IMonitoringService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                Agent[] agents = await ListAllAgentsAsync(provider, null, cancellationTokenSource.Token);
                Task<ReadOnlyCollectionPage<AgentConnection, AgentConnectionId>>[] agentConnectionTasks = Array.ConvertAll(agents, agent => provider.ListAgentConnectionsAsync(agent.Id, null, 1, cancellationTokenSource.Token));
                await Task.Factory.ContinueWhenAll((Task[])agentConnectionTasks, TaskExtrasExtensions.PropagateExceptions);

                ISet<AgentId> connectedAgents = new HashSet<AgentId>();
                foreach (Task<ReadOnlyCollectionPage<AgentConnection, AgentConnectionId>> task in agentConnectionTasks)
                {
                    if (task.Result.Count == 0)
                        continue;

                    connectedAgents.Add(task.Result[0].AgentId);
                }

                CheckType[] checkTypes = await ListAllCheckTypesAsync(provider, null, cancellationTokenSource.Token);
                CheckType[] agentCheckTypes = checkTypes.Where(i => i.Id.IsAgent).ToArray();
                CheckType[] targetableAgentCheckTypes = agentCheckTypes.Where(i => i.Fields.Any(j => j.Name.Equals("target", StringComparison.OrdinalIgnoreCase))).ToArray();
                if (targetableAgentCheckTypes.Length == 0)
                    Assert.Inconclusive("The service did not report any targetable agent check types.");

                Entity[] entities = await ListAllEntitiesAsync(provider, null, cancellationTokenSource.Token);
                if (entities.Length == 0)
                    Assert.Inconclusive("The service did not report any entities.");

                List<Task> tasks = new List<Task>();
                foreach (Entity entity in entities)
                {
                    if (entity.AgentId == null || !connectedAgents.Contains(entity.AgentId))
                        continue;

                    tasks.Add(TestListAgentCheckTargets(provider, entity, targetableAgentCheckTypes.Select(i => i.Id), cancellationTokenSource.Token));
                }

                if (tasks.Count == 0)
                    Assert.Inconclusive("The service did not report any entities with connected agents.");

                await Task.Factory.ContinueWhenAll(tasks.ToArray(), TaskExtrasExtensions.PropagateExceptions);
            }
        }

        private async Task TestListAgentCheckTargets(IMonitoringService provider, Entity entity, IEnumerable<CheckTypeId> agentCheckTypes, CancellationToken cancellationToken)
        {
            List<Task> tasks = new List<Task>();
            foreach (CheckTypeId agentCheckType in agentCheckTypes)
                tasks.Add(TestListAgentCheckTargets(provider, entity, agentCheckType, cancellationToken));

            await Task.Factory.ContinueWhenAll(tasks.ToArray(), TaskExtrasExtensions.PropagateExceptions);
        }

        private async Task TestListAgentCheckTargets(IMonitoringService provider, Entity entity, CheckTypeId agentCheckType, CancellationToken cancellationToken)
        {
            CheckTarget[] agentCheckTargets = await ListAllAgentCheckTargetsAsync(provider, entity.Id, agentCheckType, null, cancellationToken);
            Console.WriteLine("Agent check targets for entity '{0}' ({1}) with agent check type '{2}'", entity.Label, entity.Id, agentCheckType);
            foreach (CheckTarget agentCheckTarget in agentCheckTargets)
                Console.WriteLine("    {0}: {1}", agentCheckTarget.Id, agentCheckTarget.Label);
        }

        protected static async Task<AlarmChangelog[]> ListAllAlarmChangelogsAsync(IMonitoringService service, int? blockSize, DateTimeOffset? from, DateTimeOffset? to, CancellationToken cancellationToken, IProgress<ReadOnlyCollectionPage<AlarmChangelog, AlarmChangelogId>> progress = null)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            List<AlarmChangelog> result = new List<AlarmChangelog>();
            AlarmChangelogId marker = null;

            do
            {
                ReadOnlyCollectionPage<AlarmChangelog, AlarmChangelogId> page = await service.ListAlarmChangelogsAsync(marker, blockSize, from, to, cancellationToken);
                if (progress != null)
                    progress.Report(page);

                result.AddRange(page);
                marker = page.NextMarker;
            } while (marker != null);

            return result.ToArray();
        }

        protected static async Task<AlarmChangelog[]> ListAllAlarmChangelogsAsync(IMonitoringService service, EntityId entityId, int? blockSize, DateTimeOffset? from, DateTimeOffset? to, CancellationToken cancellationToken, IProgress<ReadOnlyCollectionPage<AlarmChangelog, AlarmChangelogId>> progress = null)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            List<AlarmChangelog> result = new List<AlarmChangelog>();
            AlarmChangelogId marker = null;

            do
            {
                ReadOnlyCollectionPage<AlarmChangelog, AlarmChangelogId> page = await service.ListAlarmChangelogsAsync(entityId, marker, blockSize, from, to, cancellationToken);
                if (progress != null)
                    progress.Report(page);

                result.AddRange(page);
                marker = page.NextMarker;
            } while (marker != null);

            return result.ToArray();
        }

        protected static async Task<AlarmExample[]> ListAllAlarmExamplesAsync(IMonitoringService service, int? blockSize, CancellationToken cancellationToken, IProgress<ReadOnlyCollectionPage<AlarmExample, AlarmExampleId>> progress = null)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            List<AlarmExample> result = new List<AlarmExample>();
            AlarmExampleId marker = null;

            do
            {
                ReadOnlyCollectionPage<AlarmExample, AlarmExampleId> page = await service.ListAlarmExamplesAsync(marker, blockSize, cancellationToken);
                if (progress != null)
                    progress.Report(page);

                result.AddRange(page);
                marker = page.NextMarker;
            } while (marker != null);

            return result.ToArray();
        }

        protected static async Task<AlarmNotificationHistoryItem[]> ListAllAlarmNotificationHistoryAsync(IMonitoringService service, EntityId entityId, AlarmId alarmId, CheckId checkId, int? blockSize, DateTimeOffset? from, DateTimeOffset? to, CancellationToken cancellationToken, IProgress<ReadOnlyCollectionPage<AlarmNotificationHistoryItem, AlarmNotificationHistoryItemId>> progress = null)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            List<AlarmNotificationHistoryItem> result = new List<AlarmNotificationHistoryItem>();
            AlarmNotificationHistoryItemId marker = null;

            do
            {
                ReadOnlyCollectionPage<AlarmNotificationHistoryItem, AlarmNotificationHistoryItemId> page = await service.ListAlarmNotificationHistoryAsync(entityId, alarmId, checkId, marker, blockSize, from, to, cancellationToken);
                if (progress != null)
                    progress.Report(page);

                result.AddRange(page);
                marker = page.NextMarker;
            } while (marker != null);

            return result.ToArray();
        }

        protected static async Task<Audit[]> ListAllAuditsAsync(IMonitoringService service, int? blockSize, DateTimeOffset? from, DateTimeOffset? to, CancellationToken cancellationToken, IProgress<ReadOnlyCollectionPage<Audit, AuditId>> progress = null)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            List<Audit> result = new List<Audit>();
            AuditId marker = null;

            do
            {
                ReadOnlyCollectionPage<Audit, AuditId> page = await service.ListAuditsAsync(marker, blockSize, from, to, cancellationToken);
                if (progress != null)
                    progress.Report(page);

                result.AddRange(page);
                marker = page.NextMarker;
            } while (marker != null);

            return result.ToArray();
        }

        protected static async Task<Entity[]> ListAllEntitiesAsync(IMonitoringService service, int? blockSize, CancellationToken cancellationToken, IProgress<ReadOnlyCollectionPage<Entity, EntityId>> progress = null)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            List<Entity> result = new List<Entity>();
            EntityId marker = null;

            do
            {
                ReadOnlyCollectionPage<Entity, EntityId> page = await service.ListEntitiesAsync(marker, blockSize, cancellationToken);
                if (progress != null)
                    progress.Report(page);

                result.AddRange(page);
                marker = page.NextMarker;
            } while (marker != null);

            return result.ToArray();
        }

        protected static async Task<EntityOverview[]> ListAllEntityOverviewsAsync(IMonitoringService service, int? blockSize, CancellationToken cancellationToken, IProgress<ReadOnlyCollectionPage<EntityOverview, EntityId>> progress = null)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            List<EntityOverview> result = new List<EntityOverview>();
            EntityId marker = null;

            do
            {
                ReadOnlyCollectionPage<EntityOverview, EntityId> page = await service.ListEntityOverviewsAsync(marker, blockSize, cancellationToken);
                if (progress != null)
                    progress.Report(page);

                result.AddRange(page);
                marker = page.NextMarker;
            } while (marker != null);

            return result.ToArray();
        }

        protected static async Task<EntityOverview[]> ListAllEntityOverviewsAsync(IMonitoringService service, int? blockSize, IEnumerable<EntityId> entityIdFilter, CancellationToken cancellationToken, IProgress<ReadOnlyCollectionPage<EntityOverview, EntityId>> progress = null)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            List<EntityOverview> result = new List<EntityOverview>();
            EntityId marker = null;

            do
            {
                ReadOnlyCollectionPage<EntityOverview, EntityId> page = await service.ListEntityOverviewsAsync(marker, blockSize, entityIdFilter, cancellationToken);
                if (progress != null)
                    progress.Report(page);

                result.AddRange(page);
                marker = page.NextMarker;
            } while (marker != null);

            return result.ToArray();
        }

        protected static async Task<Alarm[]> ListAllAlarmsAsync(IMonitoringService service, EntityId entityId, int? blockSize, CancellationToken cancellationToken, IProgress<ReadOnlyCollectionPage<Alarm, AlarmId>> progress = null)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            List<Alarm> result = new List<Alarm>();
            AlarmId marker = null;

            do
            {
                ReadOnlyCollectionPage<Alarm, AlarmId> page = await service.ListAlarmsAsync(entityId, marker, blockSize, cancellationToken);
                if (progress != null)
                    progress.Report(page);

                result.AddRange(page);
                marker = page.NextMarker;
            } while (marker != null);

            return result.ToArray();
        }

        protected static async Task<Check[]> ListAllChecksAsync(IMonitoringService service, EntityId entityId, int? blockSize, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            if (entityId == null)
                throw new ArgumentNullException("entityId");

            List<Check> result = new List<Check>();
            CheckId marker = null;

            do
            {
                ReadOnlyCollectionPage<Check, CheckId> page = await service.ListChecksAsync(entityId, marker, blockSize, cancellationToken);
                result.AddRange(page);
                marker = page.NextMarker;
            } while (marker != null);

            return result.ToArray();
        }

        protected static async Task<CheckType[]> ListAllCheckTypesAsync(IMonitoringService service, int? blockSize, CancellationToken cancellationToken, IProgress<ReadOnlyCollectionPage<CheckType, CheckTypeId>> progress = null)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            List<CheckType> result = new List<CheckType>();
            CheckTypeId marker = null;

            do
            {
                ReadOnlyCollectionPage<CheckType, CheckTypeId> page = await service.ListCheckTypesAsync(marker, blockSize, cancellationToken);
                if (progress != null)
                    progress.Report(page);

                result.AddRange(page);
                marker = page.NextMarker;
            } while (marker != null);

            return result.ToArray();
        }

        protected static async Task<Metric[]> ListAllMetricsAsync(IMonitoringService service, EntityId entityId, CheckId checkId, int? blockSize, CancellationToken cancellationToken, IProgress<ReadOnlyCollectionPage<Metric, MetricName>> progress = null)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            List<Metric> result = new List<Metric>();
            MetricName marker = null;

            do
            {
                ReadOnlyCollectionPage<Metric, MetricName> page = await service.ListMetricsAsync(entityId, checkId, marker, blockSize, cancellationToken);
                if (progress != null)
                    progress.Report(page);

                result.AddRange(page);
                marker = page.NextMarker;
            } while (marker != null);

            return result.ToArray();
        }

        protected static async Task<MonitoringZone[]> ListAllMonitoringZonesAsync(IMonitoringService service, int? blockSize, CancellationToken cancellationToken, IProgress<ReadOnlyCollectionPage<MonitoringZone, MonitoringZoneId>> progress = null)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            List<MonitoringZone> result = new List<MonitoringZone>();
            MonitoringZoneId marker = null;

            do
            {
                ReadOnlyCollectionPage<MonitoringZone, MonitoringZoneId> page = await service.ListMonitoringZonesAsync(marker, blockSize, cancellationToken);
                if (progress != null)
                    progress.Report(page);

                result.AddRange(page);
                marker = page.NextMarker;
            } while (marker != null);

            return result.ToArray();
        }

        protected static async Task<NotificationPlan[]> ListAllNotificationPlansAsync(IMonitoringService service, int? blockSize, CancellationToken cancellationToken, IProgress<ReadOnlyCollectionPage<NotificationPlan, NotificationPlanId>> progress = null)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            List<NotificationPlan> result = new List<NotificationPlan>();
            NotificationPlanId marker = null;

            do
            {
                ReadOnlyCollectionPage<NotificationPlan, NotificationPlanId> page = await service.ListNotificationPlansAsync(marker, blockSize, cancellationToken);
                if (progress != null)
                    progress.Report(page);

                result.AddRange(page);
                marker = page.NextMarker;
            } while (marker != null);

            return result.ToArray();
        }

        protected static async Task<Notification[]> ListAllNotificationsAsync(IMonitoringService service, int? blockSize, CancellationToken cancellationToken, IProgress<ReadOnlyCollectionPage<Notification, NotificationId>> progress = null)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            List<Notification> result = new List<Notification>();
            NotificationId marker = null;

            do
            {
                ReadOnlyCollectionPage<Notification, NotificationId> page = await service.ListNotificationsAsync(marker, blockSize, cancellationToken);
                if (progress != null)
                    progress.Report(page);

                result.AddRange(page);
                marker = page.NextMarker;
            } while (marker != null);

            return result.ToArray();
        }

        protected static async Task<NotificationType[]> ListAllNotificationTypesAsync(IMonitoringService service, int? blockSize, CancellationToken cancellationToken, IProgress<ReadOnlyCollectionPage<NotificationType, NotificationTypeId>> progress = null)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            List<NotificationType> result = new List<NotificationType>();
            NotificationTypeId marker = null;

            do
            {
                ReadOnlyCollectionPage<NotificationType, NotificationTypeId> page = await service.ListNotificationTypesAsync(marker, blockSize, cancellationToken);
                if (progress != null)
                    progress.Report(page);

                result.AddRange(page);
                marker = page.NextMarker;
            } while (marker != null);

            return result.ToArray();
        }

        protected static async Task<Agent[]> ListAllAgentsAsync(IMonitoringService service, int? blockSize, CancellationToken cancellationToken, IProgress<ReadOnlyCollectionPage<Agent, AgentId>> progress = null)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            List<Agent> result = new List<Agent>();
            AgentId marker = null;

            do
            {
                ReadOnlyCollectionPage<Agent, AgentId> page = await service.ListAgentsAsync(marker, blockSize, cancellationToken);
                if (progress != null)
                    progress.Report(page);

                result.AddRange(page);
                marker = page.NextMarker;
            } while (marker != null);

            return result.ToArray();
        }

        protected static async Task<AgentConnection[]> ListAllAgentConnectionsAsync(IMonitoringService service, AgentId agentId, int? blockSize, CancellationToken cancellationToken, IProgress<ReadOnlyCollectionPage<AgentConnection, AgentConnectionId>> progress = null)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            List<AgentConnection> result = new List<AgentConnection>();
            AgentConnectionId marker = null;

            do
            {
                ReadOnlyCollectionPage<AgentConnection, AgentConnectionId> page = await service.ListAgentConnectionsAsync(agentId, marker, blockSize, cancellationToken);
                if (progress != null)
                    progress.Report(page);

                result.AddRange(page);
                marker = page.NextMarker;
            } while (marker != null);

            return result.ToArray();
        }

        protected static async Task<AgentToken[]> ListAllAgentTokensAsync(IMonitoringService service, int? blockSize, CancellationToken cancellationToken, IProgress<ReadOnlyCollectionPage<AgentToken, AgentTokenId>> progress = null)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            List<AgentToken> result = new List<AgentToken>();
            AgentTokenId marker = null;

            do
            {
                ReadOnlyCollectionPage<AgentToken, AgentTokenId> page = await service.ListAgentTokensAsync(marker, blockSize, cancellationToken);
                if (progress != null)
                    progress.Report(page);

                result.AddRange(page);
                marker = page.NextMarker;
            } while (marker != null);

            return result.ToArray();
        }

        protected static async Task<CheckTarget[]> ListAllAgentCheckTargetsAsync(IMonitoringService service, EntityId entityId, CheckTypeId agentCheckType, int? blockSize, CancellationToken cancellationToken, IProgress<ReadOnlyCollectionPage<CheckTarget, CheckTargetId>> progress = null)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            List<CheckTarget> result = new List<CheckTarget>();
            CheckTargetId marker = null;

            do
            {
                ReadOnlyCollectionPage<CheckTarget, CheckTargetId> page = await service.ListAgentCheckTargetsAsync(entityId, agentCheckType, marker, blockSize, cancellationToken);
                if (progress != null)
                    progress.Report(page);

                result.AddRange(page);
                marker = page.NextMarker;
            } while (marker != null);

            return result.ToArray();
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
        /// Creates a random entity name with the proper prefix for integration testing.
        /// </summary>
        /// <returns>A unique, randomly-generated entity name.</returns>
        internal static string CreateRandomEntityName()
        {
            return TestEntityPrefix + Path.GetRandomFileName();
        }

        /// <summary>
        /// Creates a random check name with the proper prefix for integration testing.
        /// </summary>
        /// <returns>A unique, randomly-generated check name.</returns>
        internal static string CreateRandomCheckName()
        {
            return Path.GetRandomFileName();
        }

        /// <summary>
        /// Creates a random alarm name with the proper prefix for integration testing.
        /// </summary>
        /// <returns>A unique, randomly-generated alarm name.</returns>
        internal static string CreateRandomAlarmName()
        {
            return Path.GetRandomFileName();
        }

        /// <summary>
        /// Creates a random notification plan name with the proper prefix for integration testing.
        /// </summary>
        /// <returns>A unique, randomly-generated notification plan name.</returns>
        internal static string CreateRandomNotificationPlanName()
        {
            return TestNotificationPlanPrefix + Path.GetRandomFileName();
        }

        /// <summary>
        /// Creates a random notification name with the proper prefix for integration testing.
        /// </summary>
        /// <returns>A unique, randomly-generated notification name.</returns>
        internal static string CreateRandomNotificationName()
        {
            return TestNotificationPrefix + Path.GetRandomFileName();
        }

        /// <summary>
        /// Creates a random agent token name with the proper prefix for integration testing.
        /// </summary>
        /// <returns>A unique, randomly-generated agent token name.</returns>
        internal static string CreateRandomAgentTokenName()
        {
            return TestAgentTokenPrefix + Path.GetRandomFileName();
        }

        /// <summary>
        /// Creates an instance of <see cref="IMonitoringService"/> for testing using
        /// the <see cref="OpenstackNetSetings.TestIdentity"/>.
        /// </summary>
        /// <returns>An instance of <see cref="IMonitoringService"/> for integration testing.</returns>
        internal static IMonitoringService CreateProvider()
        {
            var provider = new TestCloudMonitoringProvider(Bootstrapper.Settings.TestIdentity, Bootstrapper.Settings.DefaultRegion, null);
            provider.BeforeAsyncWebRequest +=
                (sender, e) =>
                {
                    Console.Error.WriteLine("{0} (Request) {1} {2}", DateTime.Now, e.Request.Method, e.Request.RequestUri);
                };
            provider.AfterAsyncWebResponse +=
                (sender, e) =>
                {
                    Console.Error.WriteLine("{0} (Result {1}) {2}", DateTime.Now, e.Response.StatusCode, e.Response.ResponseUri);
                };

            provider.ConnectionLimit = 20;
            return provider;
        }

        internal class TestCloudMonitoringProvider : CloudMonitoringProvider
        {
            public TestCloudMonitoringProvider(CloudIdentity defaultIdentity, string defaultRegion, IIdentityProvider identityProvider)
                : base(defaultIdentity, defaultRegion, identityProvider)
            {
            }

            protected override byte[] EncodeRequestBodyImpl<TBody>(HttpWebRequest request, TBody body)
            {
                byte[] encoded = base.EncodeRequestBodyImpl<TBody>(request, body);
                Console.Error.WriteLine("<== " + Encoding.UTF8.GetString(encoded));
                return encoded;
            }

            protected override Tuple<HttpWebResponse, string> ReadResultImpl(Task<WebResponse> task, CancellationToken cancellationToken)
            {
                try
                {
                    Tuple<HttpWebResponse, string> result = base.ReadResultImpl(task, cancellationToken);
                    if (!string.IsNullOrEmpty(result.Item2))
                        Console.Error.WriteLine("==> " + result.Item2);

                    return result;
                }
                catch (WebException ex)
                {
                    if (ex.Response != null && ex.Response.ContentLength != 0)
                        Console.Error.WriteLine("==> " + ex.Message);

                    throw;
                }
            }
        }
    }
}
