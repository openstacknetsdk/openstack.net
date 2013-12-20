namespace Net.OpenStack.Testing.Integration.Providers.Rackspace
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using net.openstack.Core;
    using net.openstack.Core.Domain;
    using net.openstack.Core.Providers;
    using net.openstack.Providers.Rackspace;
    using net.openstack.Providers.Rackspace.Objects.AutoScale;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using CancellationToken = System.Threading.CancellationToken;
    using CancellationTokenSource = System.Threading.CancellationTokenSource;
    using CloudIdentity = net.openstack.Core.Domain.CloudIdentity;
    using HttpWebRequest = System.Net.HttpWebRequest;
    using HttpWebResponse = System.Net.HttpWebResponse;
    using IIdentityProvider = net.openstack.Core.Providers.IIdentityProvider;
    using Path = System.IO.Path;
    using StreamReader = System.IO.StreamReader;
    using WebException = System.Net.WebException;
    using WebRequest = System.Net.WebRequest;
    using WebResponse = System.Net.WebResponse;

    [TestClass]
    public class UserAutoScaleTests
    {
        /// <summary>
        /// The prefix to use for names of scaling groups created during integration testing.
        /// </summary>
        public static readonly string TestScalingGroupPrefix = "UnitTestGroup-";

        [TestMethod]
        [TestCategory(TestCategories.Cleanup)]
        public async Task CleanupScalingGroups()
        {
            IAutoScaleService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                List<Task> cleanupTasks = new List<Task>();
                ScalingGroup[] groups = await ListAllScalingGroupsAsync(provider, null, cancellationTokenSource.Token);
                foreach (ScalingGroup group in groups)
                {
                    if (group == null || group.State == null || group.State.Name == null)
                        continue;

                    if (!group.State.Name.StartsWith(TestScalingGroupPrefix, StringComparison.OrdinalIgnoreCase))
                        continue;

                    Console.WriteLine("Removing scaling group '{0}' ({1})", group.State.Name, group.Id);

                    Task prepTask;
                    if (group.State.ActiveCapacity > 0)
                        prepTask = provider.SetGroupConfigurationAsync(group.Id, new GroupConfiguration(group.State.Name, TimeSpan.FromSeconds(60), 0, 0, new JObject()), cancellationTokenSource.Token);
                    else
                        prepTask = InternalTaskExtensions.CompletedTask();

                    cleanupTasks.Add(prepTask.ContinueWith(task => provider.DeleteGroupAsync(group.Id, false, cancellationTokenSource.Token)).Unwrap());
                }

                if (cleanupTasks.Count > 0)
                    await Task.Factory.ContinueWhenAll(cleanupTasks.ToArray(), TaskExtrasExtensions.PropagateExceptions);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestListScalingGroups()
        {
            IAutoScaleService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                ScalingGroup[] scalingGroups = await ListAllScalingGroupsAsync(provider, null, cancellationTokenSource.Token);
                if (scalingGroups.Length == 0)
                    Assert.Inconclusive("The service did not report any scaling groups.");

                foreach (ScalingGroup scalingGroup in scalingGroups)
                    Console.WriteLine("Scaling group '{0}' ({1})", scalingGroup.State.Name, scalingGroup.Id);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestCreateGroup()
        {
            IAutoScaleService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                FlavorId flavorId = await GetDefaultFlavorIdAsync(provider, cancellationTokenSource.Token);
                ImageId imageId = await GetDefaultImageIdAsync(provider, cancellationTokenSource.Token);

                string groupName = CreateRandomScalingGroupName();
                string serverName = UserComputeTests.UnitTestServerPrefix + groupName + '-';
                GroupConfiguration groupConfiguration = new GroupConfiguration(name: groupName, cooldown: TimeSpan.FromSeconds(60), minEntities: 0, maxEntities: 0, metadata: new JObject());
                LaunchConfiguration launchConfiguration = new ServerLaunchConfiguration(new ServerLaunchArguments(new ServerArgument(flavorId, imageId, serverName, null, null)));
                PolicyConfiguration[] policyConfigurations = { };
                ScalingGroupConfiguration configuration = new ScalingGroupConfiguration(groupConfiguration, launchConfiguration, policyConfigurations);
                ScalingGroup scalingGroup = await provider.CreateGroupAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(scalingGroup);
                Assert.IsNotNull(scalingGroup.Id);

                await provider.DeleteGroupAsync(scalingGroup.Id, false, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestCreateGroupWithEntities()
        {
            IAutoScaleService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                FlavorId flavorId = await GetDefaultFlavorIdAsync(provider, cancellationTokenSource.Token);
                ImageId imageId = await GetDefaultImageIdAsync(provider, cancellationTokenSource.Token);

                string groupName = CreateRandomScalingGroupName();
                string serverName = UserComputeTests.UnitTestServerPrefix + groupName + '-';
                GroupConfiguration groupConfiguration = new GroupConfiguration(name: groupName, cooldown: TimeSpan.FromSeconds(60), minEntities: 1, maxEntities: 1, metadata: new JObject());
                LaunchConfiguration launchConfiguration = new ServerLaunchConfiguration(new ServerLaunchArguments(new ServerArgument(flavorId, imageId, serverName, null, null)));
                PolicyConfiguration[] policyConfigurations = { };
                ScalingGroupConfiguration configuration = new ScalingGroupConfiguration(groupConfiguration, launchConfiguration, policyConfigurations);
                ScalingGroup scalingGroup = await provider.CreateGroupAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(scalingGroup);
                Assert.IsNotNull(scalingGroup.Id);

                GroupConfiguration updatedConfiguration = new GroupConfiguration(name: groupName, cooldown: TimeSpan.FromSeconds(240), minEntities: 0, maxEntities: 0, metadata: new JObject());
                await provider.SetGroupConfigurationAsync(scalingGroup.Id, updatedConfiguration, cancellationTokenSource.Token);

                await provider.DeleteGroupAsync(scalingGroup.Id, false, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestGetGroup()
        {
            IAutoScaleService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                ScalingGroup[] scalingGroups = await ListAllScalingGroupsAsync(provider, null, cancellationTokenSource.Token);
                if (scalingGroups.Length == 0)
                    Assert.Inconclusive("The service did not report any scaling groups.");

                foreach (ScalingGroup scalingGroup in scalingGroups)
                {
                    ScalingGroup singleGroup = await provider.GetGroupAsync(scalingGroup.Id, cancellationTokenSource.Token);
                    Assert.IsNotNull(singleGroup);
                    Assert.AreEqual(scalingGroup.Id, singleGroup.Id);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestGetGroupState()
        {
            IAutoScaleService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                ScalingGroup[] scalingGroups = await ListAllScalingGroupsAsync(provider, null, cancellationTokenSource.Token);
                if (scalingGroups.Length == 0)
                    Assert.Inconclusive("The service did not report any scaling groups.");

                foreach (ScalingGroup scalingGroup in scalingGroups)
                {
                    GroupState groupState = await provider.GetGroupStateAsync(scalingGroup.Id, cancellationTokenSource.Token);
                    Assert.IsNotNull(groupState);
                    Assert.AreEqual(scalingGroup.State.ActiveCapacity, groupState.ActiveCapacity);
                    Assert.AreEqual(scalingGroup.State.DesiredCapacity, groupState.DesiredCapacity);
                    Assert.AreEqual(scalingGroup.State.Name, groupState.Name);
                    Assert.AreEqual(scalingGroup.State.Paused, groupState.Paused);
                    Assert.AreEqual(scalingGroup.State.PendingCapacity, groupState.PendingCapacity);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestPauseGroup()
        {
            IAutoScaleService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                FlavorId flavorId = await GetDefaultFlavorIdAsync(provider, cancellationTokenSource.Token);
                ImageId imageId = await GetDefaultImageIdAsync(provider, cancellationTokenSource.Token);

                string groupName = CreateRandomScalingGroupName();
                string serverName = UserComputeTests.UnitTestServerPrefix + groupName + '-';
                GroupConfiguration groupConfiguration = new GroupConfiguration(name: groupName, cooldown: TimeSpan.FromSeconds(60), minEntities: 0, maxEntities: 0, metadata: new JObject());
                LaunchConfiguration launchConfiguration = new ServerLaunchConfiguration(new ServerLaunchArguments(new ServerArgument(flavorId, imageId, serverName, null, null)));
                PolicyConfiguration[] policyConfigurations = { };
                ScalingGroupConfiguration configuration = new ScalingGroupConfiguration(groupConfiguration, launchConfiguration, policyConfigurations);
                ScalingGroup scalingGroup = await provider.CreateGroupAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(scalingGroup);

                await provider.PauseGroupAsync(scalingGroup.Id, cancellationTokenSource.Token);
                await provider.PauseGroupAsync(scalingGroup.Id, cancellationTokenSource.Token);
                await provider.ResumeGroupAsync(scalingGroup.Id, cancellationTokenSource.Token);
                await provider.ResumeGroupAsync(scalingGroup.Id, cancellationTokenSource.Token);

                await provider.DeleteGroupAsync(scalingGroup.Id, false, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestGetGroupConfiguration()
        {
            IAutoScaleService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                ScalingGroup[] scalingGroups = await ListAllScalingGroupsAsync(provider, null, cancellationTokenSource.Token);
                if (scalingGroups.Length == 0)
                    Assert.Inconclusive("The service did not report any scaling groups.");

                foreach (ScalingGroup scalingGroup in scalingGroups)
                {
                    GroupConfiguration groupConfiguration = await provider.GetGroupConfigurationAsync(scalingGroup.Id, cancellationTokenSource.Token);
                    Assert.IsNotNull(groupConfiguration);
                    Assert.AreEqual(scalingGroup.State.Name, groupConfiguration.Name);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestSetGroupConfiguration()
        {
            IAutoScaleService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                FlavorId flavorId = await GetDefaultFlavorIdAsync(provider, cancellationTokenSource.Token);
                ImageId imageId = await GetDefaultImageIdAsync(provider, cancellationTokenSource.Token);

                string groupName = CreateRandomScalingGroupName();
                string serverName = UserComputeTests.UnitTestServerPrefix + groupName + '-';
                GroupConfiguration groupConfiguration = new GroupConfiguration(name: groupName, cooldown: TimeSpan.FromSeconds(60), minEntities: 0, maxEntities: 1, metadata: new JObject());
                LaunchConfiguration launchConfiguration = new ServerLaunchConfiguration(new ServerLaunchArguments(new ServerArgument(flavorId, imageId, serverName, null, null)));
                PolicyConfiguration[] policyConfigurations = { };
                ScalingGroupConfiguration configuration = new ScalingGroupConfiguration(groupConfiguration, launchConfiguration, policyConfigurations);
                ScalingGroup scalingGroup = await provider.CreateGroupAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(scalingGroup);

                GroupConfiguration updatedConfiguration = new GroupConfiguration(name: groupName, cooldown: TimeSpan.FromSeconds(240), minEntities: 0, maxEntities: 0, metadata: new JObject());
                await provider.SetGroupConfigurationAsync(scalingGroup.Id, updatedConfiguration, cancellationTokenSource.Token);

                await provider.DeleteGroupAsync(scalingGroup.Id, false, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestGetLaunchConfiguration()
        {
            IAutoScaleService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                ScalingGroup[] scalingGroups = await ListAllScalingGroupsAsync(provider, null, cancellationTokenSource.Token);
                if (scalingGroups.Length == 0)
                    Assert.Inconclusive("The service did not report any scaling groups.");

                foreach (ScalingGroup scalingGroup in scalingGroups)
                {
                    LaunchConfiguration launchConfiguration = await provider.GetLaunchConfigurationAsync(scalingGroup.Id, cancellationTokenSource.Token);
                    Assert.IsNotNull(launchConfiguration);
                    Assert.IsNotNull(launchConfiguration.LaunchType);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestSetLaunchConfiguration()
        {
            IAutoScaleService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                FlavorId flavorId = await GetDefaultFlavorIdAsync(provider, cancellationTokenSource.Token);
                ImageId imageId = await GetDefaultImageIdAsync(provider, cancellationTokenSource.Token);

                string groupName = CreateRandomScalingGroupName();
                string serverName = UserComputeTests.UnitTestServerPrefix + groupName + '-';
                GroupConfiguration groupConfiguration = new GroupConfiguration(name: groupName, cooldown: TimeSpan.FromSeconds(60), minEntities: 0, maxEntities: 1, metadata: new JObject());
                LaunchConfiguration launchConfiguration = new ServerLaunchConfiguration(new ServerLaunchArguments(new ServerArgument(flavorId, imageId, serverName, null, null)));
                PolicyConfiguration[] policyConfigurations = { };
                ScalingGroupConfiguration configuration = new ScalingGroupConfiguration(groupConfiguration, launchConfiguration, policyConfigurations);
                ScalingGroup scalingGroup = await provider.CreateGroupAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(scalingGroup);

                Personality[] personality = { new Personality("/usr/lib/myfile.txt", "Stuff", Encoding.UTF8) };
                LaunchConfiguration updatedConfiguration = new ServerLaunchConfiguration(new ServerLaunchArguments(new ServerArgument(flavorId, imageId, serverName, null, personality)));
                await provider.SetLaunchConfigurationAsync(scalingGroup.Id, updatedConfiguration, cancellationTokenSource.Token);

                await provider.DeleteGroupAsync(scalingGroup.Id, false, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestListPolicies()
        {
            IAutoScaleService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                ScalingGroup[] scalingGroups = await ListAllScalingGroupsAsync(provider, null, cancellationTokenSource.Token);
                if (scalingGroups.Length == 0)
                    Assert.Inconclusive("The service did not report any scaling groups.");

                bool foundPolicy = false;
                foreach (ScalingGroup scalingGroup in scalingGroups)
                {
                    Console.WriteLine("Scaling group '{0}' ({1})", scalingGroup.State.Name, scalingGroup.Id);
                    Policy[] policies = await ListAllPoliciesAsync(provider, scalingGroup.Id, null, cancellationTokenSource.Token);
                    foreach (Policy policy in policies)
                    {
                        foundPolicy = true;
                        Console.WriteLine("    Policy '{0}' ({1})", policy.Name, policy.Id);
                    }
                }

                if (!foundPolicy)
                    Assert.Inconclusive("The service did not report any policies.");
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestCreatePolicy()
        {
            IAutoScaleService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                FlavorId flavorId = await GetDefaultFlavorIdAsync(provider, cancellationTokenSource.Token);
                ImageId imageId = await GetDefaultImageIdAsync(provider, cancellationTokenSource.Token);

                string groupName = CreateRandomScalingGroupName();
                string serverName = UserComputeTests.UnitTestServerPrefix + groupName + '-';
                GroupConfiguration groupConfiguration = new GroupConfiguration(name: groupName, cooldown: TimeSpan.FromSeconds(60), minEntities: 0, maxEntities: 0, metadata: new JObject());
                LaunchConfiguration launchConfiguration = new ServerLaunchConfiguration(new ServerLaunchArguments(new ServerArgument(flavorId, imageId, serverName, null, null)));
                PolicyConfiguration[] policyConfigurations = { };
                ScalingGroupConfiguration configuration = new ScalingGroupConfiguration(groupConfiguration, launchConfiguration, policyConfigurations);
                ScalingGroup scalingGroup = await provider.CreateGroupAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(scalingGroup);
                Assert.IsNotNull(scalingGroup.Id);

                PolicyConfiguration policyConfiguration;
                Policy policy;

                /* Desired Capacity
                 */
                policyConfiguration = PolicyConfiguration.Capacity("Capacity 0 Policy", 0, TimeSpan.FromSeconds(60));
                Assert.AreEqual(PolicyType.Webhook, policyConfiguration.PolicyType);
                policy = await provider.CreatePolicyAsync(scalingGroup.Id, policyConfiguration, cancellationTokenSource.Token);
                Assert.IsNotNull(policy);
                Assert.AreEqual(policyConfiguration.Change, policy.Change);
                Assert.AreEqual(policyConfiguration.ChangePercent, policy.ChangePercent);
                Assert.AreEqual(policyConfiguration.Cooldown, policy.Cooldown);
                Assert.AreEqual(policyConfiguration.DesiredCapacity, policy.DesiredCapacity);
                Assert.AreEqual(policyConfiguration.Name, policy.Name);
                Assert.AreEqual(policyConfiguration.PolicyType, policy.PolicyType);

                await provider.DeletePolicyAsync(scalingGroup.Id, policy.Id, cancellationTokenSource.Token);

                /* Incremental Change
                 */
                policyConfiguration = PolicyConfiguration.IncrementalChange("Incremental Change -1 Policy", -1, TimeSpan.FromSeconds(60));
                Assert.AreEqual(PolicyType.Webhook, policyConfiguration.PolicyType);
                policy = await provider.CreatePolicyAsync(scalingGroup.Id, policyConfiguration, cancellationTokenSource.Token);
                Assert.IsNotNull(policy);
                Assert.AreEqual(policyConfiguration.Change, policy.Change);
                Assert.AreEqual(policyConfiguration.ChangePercent, policy.ChangePercent);
                Assert.AreEqual(policyConfiguration.Cooldown, policy.Cooldown);
                Assert.AreEqual(policyConfiguration.DesiredCapacity, policy.DesiredCapacity);
                Assert.AreEqual(policyConfiguration.Name, policy.Name);
                Assert.AreEqual(policyConfiguration.PolicyType, policy.PolicyType);

                await provider.DeletePolicyAsync(scalingGroup.Id, policy.Id, cancellationTokenSource.Token);

                /* Percentage Change
                 */
                policyConfiguration = PolicyConfiguration.PercentageChange("Percentage Change -50% Policy", -50.0, TimeSpan.FromSeconds(60));
                Assert.AreEqual(PolicyType.Webhook, policyConfiguration.PolicyType);
                policy = await provider.CreatePolicyAsync(scalingGroup.Id, policyConfiguration, cancellationTokenSource.Token);
                Assert.IsNotNull(policy);
                Assert.AreEqual(policyConfiguration.Change, policy.Change);
                Assert.AreEqual(policyConfiguration.ChangePercent, policy.ChangePercent);
                Assert.AreEqual(policyConfiguration.Cooldown, policy.Cooldown);
                Assert.AreEqual(policyConfiguration.DesiredCapacity, policy.DesiredCapacity);
                Assert.AreEqual(policyConfiguration.Name, policy.Name);
                Assert.AreEqual(policyConfiguration.PolicyType, policy.PolicyType);

                await provider.DeletePolicyAsync(scalingGroup.Id, policy.Id, cancellationTokenSource.Token);

                /* Percentage Change At Time
                 */
                policyConfiguration = PolicyConfiguration.PercentageChangeAtTime("Percentage Change -50% At Time Policy", -50.0, TimeSpan.FromSeconds(60), DateTimeOffset.UtcNow.AddMinutes(30));
                Assert.AreEqual(PolicyType.Schedule, policyConfiguration.PolicyType);
                policy = await provider.CreatePolicyAsync(scalingGroup.Id, policyConfiguration, cancellationTokenSource.Token);
                Assert.IsNotNull(policy);
                Assert.AreEqual(policyConfiguration.Change, policy.Change);
                Assert.AreEqual(policyConfiguration.ChangePercent, policy.ChangePercent);
                Assert.AreEqual(policyConfiguration.Cooldown, policy.Cooldown);
                Assert.AreEqual(policyConfiguration.DesiredCapacity, policy.DesiredCapacity);
                Assert.AreEqual(policyConfiguration.Name, policy.Name);
                Assert.AreEqual(policyConfiguration.PolicyType, policy.PolicyType);

                await provider.DeletePolicyAsync(scalingGroup.Id, policy.Id, cancellationTokenSource.Token);

                /* Cleanup
                 */
                await provider.DeleteGroupAsync(scalingGroup.Id, false, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestCreatePolicyBasicFormatHours()
        {
            await TestCreatePolicyWithTimeFormat("yyyyMMdd'T'HH'Z'");
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestCreatePolicyBasicFormatMinutes()
        {
            await TestCreatePolicyWithTimeFormat("yyyyMMdd'T'HHmm'Z'");
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestCreatePolicyBasicFormatSeconds()
        {
            await TestCreatePolicyWithTimeFormat("yyyyMMdd'T'HHmmss'Z'");
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestCreatePolicyBasicFormatFractionalHours()
        {
            await TestCreatePolicyWithTimeFormat("yyyyMMdd'T'HH'.'fffffff'Z'");
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestCreatePolicyBasicFormatFractionalMinutes()
        {
            await TestCreatePolicyWithTimeFormat("yyyyMMdd'T'HHmm'.'fffffff'Z'");
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestCreatePolicyBasicFormatFractionalTenthsMicroseconds()
        {
            await TestCreatePolicyWithTimeFormat("yyyyMMdd'T'HHmmss'.'fffffff'Z'");
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestCreatePolicyExtendedFormatHours()
        {
            await TestCreatePolicyWithTimeFormat("yyyy'-'MM'-'dd'T'HH'Z'");
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestCreatePolicyExtendedFormatMinutes()
        {
            await TestCreatePolicyWithTimeFormat("yyyy'-'MM'-'dd'T'HH':'mm'Z'");
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestCreatePolicyExtendedFormatSeconds()
        {
            await TestCreatePolicyWithTimeFormat("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestCreatePolicyExtendedFormatFractionalHours()
        {
            await TestCreatePolicyWithTimeFormat("yyyy'-'MM'-'dd'T'HH'.'fffffff'Z'");
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestCreatePolicyExtendedFormatFractionalMinutes()
        {
            await TestCreatePolicyWithTimeFormat("yyyy'-'MM'-'dd'T'HH':'mm'.'fffffff'Z'");
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestCreatePolicyExtendedFormatFractionalTenthsMicroseconds()
        {
            await TestCreatePolicyWithTimeFormat("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffff'Z'");
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestCreatePolicyExtendedFormatEndOfDay()
        {
            await TestCreatePolicyWithTimeFormat("yyyy'-'MM'-'dd'T24:00:00Z'");
        }

        private async Task TestCreatePolicyWithTimeFormat(string timeFormat)
        {
            IAutoScaleService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                FlavorId flavorId = await GetDefaultFlavorIdAsync(provider, cancellationTokenSource.Token);
                ImageId imageId = await GetDefaultImageIdAsync(provider, cancellationTokenSource.Token);

                string groupName = CreateRandomScalingGroupName();
                string serverName = UserComputeTests.UnitTestServerPrefix + groupName + '-';
                GroupConfiguration groupConfiguration = new GroupConfiguration(name: groupName, cooldown: TimeSpan.FromSeconds(60), minEntities: 0, maxEntities: 0, metadata: new JObject());
                LaunchConfiguration launchConfiguration = new ServerLaunchConfiguration(new ServerLaunchArguments(new ServerArgument(flavorId, imageId, serverName, null, null)));
                PolicyConfiguration[] policyConfigurations = { };
                ScalingGroupConfiguration configuration = new ScalingGroupConfiguration(groupConfiguration, launchConfiguration, policyConfigurations);
                ScalingGroup scalingGroup = await provider.CreateGroupAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(scalingGroup);
                Assert.IsNotNull(scalingGroup.Id);

                PolicyConfiguration policyConfiguration;
                Policy policy;

                /* Percentage Change At Time
                 */
                policyConfiguration = PolicyConfiguration.PercentageChangeAtTimeInternal("Percentage Change -50% At Time Policy", -50.0, TimeSpan.FromSeconds(60), DateTimeOffset.UtcNow.AddMinutes(30).ToString(timeFormat, CultureInfo.InvariantCulture));
                Assert.AreEqual(PolicyType.Schedule, policyConfiguration.PolicyType);
                policy = await provider.CreatePolicyAsync(scalingGroup.Id, policyConfiguration, cancellationTokenSource.Token);
                Assert.IsNotNull(policy);
                Assert.AreEqual(policyConfiguration.Change, policy.Change);
                Assert.AreEqual(policyConfiguration.ChangePercent, policy.ChangePercent);
                Assert.AreEqual(policyConfiguration.Cooldown, policy.Cooldown);
                Assert.AreEqual(policyConfiguration.DesiredCapacity, policy.DesiredCapacity);
                Assert.AreEqual(policyConfiguration.Name, policy.Name);
                Assert.AreEqual(policyConfiguration.PolicyType, policy.PolicyType);

                await provider.DeletePolicyAsync(scalingGroup.Id, policy.Id, cancellationTokenSource.Token);

                /* Cleanup
                 */
                await provider.DeleteGroupAsync(scalingGroup.Id, false, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestGetPolicy()
        {
            IAutoScaleService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                FlavorId flavorId = await GetDefaultFlavorIdAsync(provider, cancellationTokenSource.Token);
                ImageId imageId = await GetDefaultImageIdAsync(provider, cancellationTokenSource.Token);

                string groupName = CreateRandomScalingGroupName();
                string serverName = UserComputeTests.UnitTestServerPrefix + groupName + '-';
                GroupConfiguration groupConfiguration = new GroupConfiguration(name: groupName, cooldown: TimeSpan.FromSeconds(60), minEntities: 0, maxEntities: 0, metadata: new JObject());
                LaunchConfiguration launchConfiguration = new ServerLaunchConfiguration(new ServerLaunchArguments(new ServerArgument(flavorId, imageId, serverName, null, null)));
                PolicyConfiguration[] policyConfigurations = { };
                ScalingGroupConfiguration configuration = new ScalingGroupConfiguration(groupConfiguration, launchConfiguration, policyConfigurations);
                ScalingGroup scalingGroup = await provider.CreateGroupAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(scalingGroup);
                Assert.IsNotNull(scalingGroup.Id);

                PolicyConfiguration policyConfiguration;
                Policy policy;
                Policy singlePolicy;

                /* Desired Capacity
                 */
                policyConfiguration = PolicyConfiguration.Capacity("Capacity 0 Policy", 0, TimeSpan.FromSeconds(60));
                Assert.AreEqual(PolicyType.Webhook, policyConfiguration.PolicyType);
                policy = await provider.CreatePolicyAsync(scalingGroup.Id, policyConfiguration, cancellationTokenSource.Token);
                Assert.IsNotNull(policy);
                Assert.IsNotNull(policy.Id);
                Assert.AreEqual(policyConfiguration.Change, policy.Change);
                Assert.AreEqual(policyConfiguration.ChangePercent, policy.ChangePercent);
                Assert.AreEqual(policyConfiguration.Cooldown, policy.Cooldown);
                Assert.AreEqual(policyConfiguration.DesiredCapacity, policy.DesiredCapacity);
                Assert.AreEqual(policyConfiguration.Name, policy.Name);
                Assert.AreEqual(policyConfiguration.PolicyType, policy.PolicyType);

                singlePolicy = await provider.GetPolicyAsync(scalingGroup.Id, policy.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(singlePolicy);
                Assert.AreEqual(policy.Id, singlePolicy.Id);
                Assert.AreEqual(policy.Change, singlePolicy.Change);
                Assert.AreEqual(policy.ChangePercent, singlePolicy.ChangePercent);
                Assert.AreEqual(policy.Cooldown, singlePolicy.Cooldown);
                Assert.AreEqual(policy.DesiredCapacity, singlePolicy.DesiredCapacity);
                Assert.AreEqual(policy.Name, singlePolicy.Name);
                Assert.AreEqual(policy.PolicyType, singlePolicy.PolicyType);
                Assert.AreEqual(policy.Id, singlePolicy.Id);
                Assert.AreEqual(policy.Id, singlePolicy.Id);
                Assert.AreEqual(policy.Id, singlePolicy.Id);

                await provider.DeletePolicyAsync(scalingGroup.Id, policy.Id, cancellationTokenSource.Token);

                /* Incremental Change
                 */
                policyConfiguration = PolicyConfiguration.IncrementalChange("Incremental Change -1 Policy", -1, TimeSpan.FromSeconds(60));
                Assert.AreEqual(PolicyType.Webhook, policyConfiguration.PolicyType);
                policy = await provider.CreatePolicyAsync(scalingGroup.Id, policyConfiguration, cancellationTokenSource.Token);
                Assert.IsNotNull(policy);
                Assert.IsNotNull(policy.Id);
                Assert.AreEqual(policyConfiguration.Change, policy.Change);
                Assert.AreEqual(policyConfiguration.ChangePercent, policy.ChangePercent);
                Assert.AreEqual(policyConfiguration.Cooldown, policy.Cooldown);
                Assert.AreEqual(policyConfiguration.DesiredCapacity, policy.DesiredCapacity);
                Assert.AreEqual(policyConfiguration.Name, policy.Name);
                Assert.AreEqual(policyConfiguration.PolicyType, policy.PolicyType);

                singlePolicy = await provider.GetPolicyAsync(scalingGroup.Id, policy.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(singlePolicy);
                Assert.AreEqual(policy.Id, singlePolicy.Id);
                Assert.AreEqual(policy.Change, singlePolicy.Change);
                Assert.AreEqual(policy.ChangePercent, singlePolicy.ChangePercent);
                Assert.AreEqual(policy.Cooldown, singlePolicy.Cooldown);
                Assert.AreEqual(policy.DesiredCapacity, singlePolicy.DesiredCapacity);
                Assert.AreEqual(policy.Name, singlePolicy.Name);
                Assert.AreEqual(policy.PolicyType, singlePolicy.PolicyType);
                Assert.AreEqual(policy.Id, singlePolicy.Id);
                Assert.AreEqual(policy.Id, singlePolicy.Id);
                Assert.AreEqual(policy.Id, singlePolicy.Id);

                await provider.DeletePolicyAsync(scalingGroup.Id, policy.Id, cancellationTokenSource.Token);

                /* Percentage Change
                 */
                policyConfiguration = PolicyConfiguration.PercentageChange("Percentage Change -50% Policy", -50.0, TimeSpan.FromSeconds(60));
                Assert.AreEqual(PolicyType.Webhook, policyConfiguration.PolicyType);
                policy = await provider.CreatePolicyAsync(scalingGroup.Id, policyConfiguration, cancellationTokenSource.Token);
                Assert.IsNotNull(policy);
                Assert.IsNotNull(policy.Id);
                Assert.AreEqual(policyConfiguration.Change, policy.Change);
                Assert.AreEqual(policyConfiguration.ChangePercent, policy.ChangePercent);
                Assert.AreEqual(policyConfiguration.Cooldown, policy.Cooldown);
                Assert.AreEqual(policyConfiguration.DesiredCapacity, policy.DesiredCapacity);
                Assert.AreEqual(policyConfiguration.Name, policy.Name);
                Assert.AreEqual(policyConfiguration.PolicyType, policy.PolicyType);

                singlePolicy = await provider.GetPolicyAsync(scalingGroup.Id, policy.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(singlePolicy);
                Assert.AreEqual(policy.Id, singlePolicy.Id);
                Assert.AreEqual(policy.Change, singlePolicy.Change);
                Assert.AreEqual(policy.ChangePercent, singlePolicy.ChangePercent);
                Assert.AreEqual(policy.Cooldown, singlePolicy.Cooldown);
                Assert.AreEqual(policy.DesiredCapacity, singlePolicy.DesiredCapacity);
                Assert.AreEqual(policy.Name, singlePolicy.Name);
                Assert.AreEqual(policy.PolicyType, singlePolicy.PolicyType);
                Assert.AreEqual(policy.Id, singlePolicy.Id);
                Assert.AreEqual(policy.Id, singlePolicy.Id);
                Assert.AreEqual(policy.Id, singlePolicy.Id);

                await provider.DeletePolicyAsync(scalingGroup.Id, policy.Id, cancellationTokenSource.Token);

                /* Percentage Change At Time
                 */
                policyConfiguration = PolicyConfiguration.PercentageChangeAtTime("Percentage Change -50% At Time Policy", -50.0, TimeSpan.FromSeconds(60), DateTimeOffset.UtcNow.AddMinutes(30));
                Assert.AreEqual(PolicyType.Schedule, policyConfiguration.PolicyType);
                policy = await provider.CreatePolicyAsync(scalingGroup.Id, policyConfiguration, cancellationTokenSource.Token);
                Assert.IsNotNull(policy);
                Assert.IsNotNull(policy.Id);
                Assert.AreEqual(policyConfiguration.Change, policy.Change);
                Assert.AreEqual(policyConfiguration.ChangePercent, policy.ChangePercent);
                Assert.AreEqual(policyConfiguration.Cooldown, policy.Cooldown);
                Assert.AreEqual(policyConfiguration.DesiredCapacity, policy.DesiredCapacity);
                Assert.AreEqual(policyConfiguration.Name, policy.Name);
                Assert.AreEqual(policyConfiguration.PolicyType, policy.PolicyType);

                singlePolicy = await provider.GetPolicyAsync(scalingGroup.Id, policy.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(singlePolicy);
                Assert.AreEqual(policy.Id, singlePolicy.Id);
                Assert.AreEqual(policy.Change, singlePolicy.Change);
                Assert.AreEqual(policy.ChangePercent, singlePolicy.ChangePercent);
                Assert.AreEqual(policy.Cooldown, singlePolicy.Cooldown);
                Assert.AreEqual(policy.DesiredCapacity, singlePolicy.DesiredCapacity);
                Assert.AreEqual(policy.Name, singlePolicy.Name);
                Assert.AreEqual(policy.PolicyType, singlePolicy.PolicyType);
                Assert.AreEqual(policy.Id, singlePolicy.Id);
                Assert.AreEqual(policy.Id, singlePolicy.Id);
                Assert.AreEqual(policy.Id, singlePolicy.Id);

                await provider.DeletePolicyAsync(scalingGroup.Id, policy.Id, cancellationTokenSource.Token);

                /* Cleanup
                 */
                await provider.DeleteGroupAsync(scalingGroup.Id, false, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestSetPolicy()
        {
            IAutoScaleService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                FlavorId flavorId = await GetDefaultFlavorIdAsync(provider, cancellationTokenSource.Token);
                ImageId imageId = await GetDefaultImageIdAsync(provider, cancellationTokenSource.Token);

                string groupName = CreateRandomScalingGroupName();
                string serverName = UserComputeTests.UnitTestServerPrefix + groupName + '-';
                GroupConfiguration groupConfiguration = new GroupConfiguration(name: groupName, cooldown: TimeSpan.FromSeconds(60), minEntities: 0, maxEntities: 1, metadata: new JObject());
                LaunchConfiguration launchConfiguration = new ServerLaunchConfiguration(new ServerLaunchArguments(new ServerArgument(flavorId, imageId, serverName, null, null)));
                PolicyConfiguration[] policyConfigurations = { };
                ScalingGroupConfiguration configuration = new ScalingGroupConfiguration(groupConfiguration, launchConfiguration, policyConfigurations);
                ScalingGroup scalingGroup = await provider.CreateGroupAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(scalingGroup);
                Assert.IsNotNull(scalingGroup.Id);

                PolicyConfiguration policyConfiguration;
                Policy policy;

                /* Desired Capacity
                 */
                policyConfiguration = PolicyConfiguration.Capacity("Capacity 1 Policy", 1, TimeSpan.FromSeconds(60));
                Assert.AreEqual(PolicyType.Webhook, policyConfiguration.PolicyType);
                policy = await provider.CreatePolicyAsync(scalingGroup.Id, policyConfiguration, cancellationTokenSource.Token);

                await provider.SetPolicyAsync(scalingGroup.Id, policy.Id, policyConfiguration, cancellationTokenSource.Token);

                await provider.DeletePolicyAsync(scalingGroup.Id, policy.Id, cancellationTokenSource.Token);

                /* Incremental Change
                 */
                policyConfiguration = PolicyConfiguration.IncrementalChange("Incremental Change -1 Policy", -1, TimeSpan.FromSeconds(60));
                Assert.AreEqual(PolicyType.Webhook, policyConfiguration.PolicyType);
                policy = await provider.CreatePolicyAsync(scalingGroup.Id, policyConfiguration, cancellationTokenSource.Token);

                await provider.SetPolicyAsync(scalingGroup.Id, policy.Id, policyConfiguration, cancellationTokenSource.Token);

                await provider.DeletePolicyAsync(scalingGroup.Id, policy.Id, cancellationTokenSource.Token);

                /* Percentage Change
                 */
                policyConfiguration = PolicyConfiguration.PercentageChange("Percentage Change -50% Policy", -50.0, TimeSpan.FromSeconds(60));
                Assert.AreEqual(PolicyType.Webhook, policyConfiguration.PolicyType);
                policy = await provider.CreatePolicyAsync(scalingGroup.Id, policyConfiguration, cancellationTokenSource.Token);

                await provider.SetPolicyAsync(scalingGroup.Id, policy.Id, policyConfiguration, cancellationTokenSource.Token);

                await provider.DeletePolicyAsync(scalingGroup.Id, policy.Id, cancellationTokenSource.Token);

                /* Percentage Change At Time
                 */
                policyConfiguration = PolicyConfiguration.PercentageChangeAtTime("Percentage Change -50% At Time Policy", -50.0, TimeSpan.FromSeconds(60), DateTimeOffset.UtcNow.AddMinutes(30));
                Assert.AreEqual(PolicyType.Schedule, policyConfiguration.PolicyType);
                policy = await provider.CreatePolicyAsync(scalingGroup.Id, policyConfiguration, cancellationTokenSource.Token);

                await provider.SetPolicyAsync(scalingGroup.Id, policy.Id, policyConfiguration, cancellationTokenSource.Token);

                await provider.DeletePolicyAsync(scalingGroup.Id, policy.Id, cancellationTokenSource.Token);

                /* Cleanup
                 */
                await provider.DeleteGroupAsync(scalingGroup.Id, false, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestExecutePolicy()
        {
            IAutoScaleService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                FlavorId flavorId = await GetDefaultFlavorIdAsync(provider, cancellationTokenSource.Token);
                ImageId imageId = await GetDefaultImageIdAsync(provider, cancellationTokenSource.Token);

                string groupName = CreateRandomScalingGroupName();
                string serverName = UserComputeTests.UnitTestServerPrefix + groupName + '-';
                GroupConfiguration groupConfiguration = new GroupConfiguration(name: groupName, cooldown: TimeSpan.Zero, minEntities: 0, maxEntities: 1, metadata: new JObject());
                LaunchConfiguration launchConfiguration = new ServerLaunchConfiguration(new ServerLaunchArguments(new ServerArgument(flavorId, imageId, serverName, null, null)));
                PolicyConfiguration[] policyConfigurations = { };
                ScalingGroupConfiguration configuration = new ScalingGroupConfiguration(groupConfiguration, launchConfiguration, policyConfigurations);
                ScalingGroup scalingGroup = await provider.CreateGroupAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(scalingGroup);
                Assert.IsNotNull(scalingGroup.Id);

                PolicyConfiguration policyConfiguration;
                Policy policy;

                /* Desired Capacity
                 */
                policyConfiguration = PolicyConfiguration.Capacity("Capacity 1 Policy", 1, TimeSpan.Zero);
                Assert.AreEqual(PolicyType.Webhook, policyConfiguration.PolicyType);
                policy = await provider.CreatePolicyAsync(scalingGroup.Id, policyConfiguration, cancellationTokenSource.Token);

                await provider.ExecutePolicyAsync(scalingGroup.Id, policy.Id, cancellationTokenSource.Token);

                await provider.DeletePolicyAsync(scalingGroup.Id, policy.Id, cancellationTokenSource.Token);

                /* Incremental Change
                 */
                policyConfiguration = PolicyConfiguration.IncrementalChange("Incremental Change -1 Policy", -1, TimeSpan.Zero);
                Assert.AreEqual(PolicyType.Webhook, policyConfiguration.PolicyType);
                policy = await provider.CreatePolicyAsync(scalingGroup.Id, policyConfiguration, cancellationTokenSource.Token);

                await provider.ExecutePolicyAsync(scalingGroup.Id, policy.Id, cancellationTokenSource.Token);

                await provider.DeletePolicyAsync(scalingGroup.Id, policy.Id, cancellationTokenSource.Token);

                /* Percentage Change
                 */
                policyConfiguration = PolicyConfiguration.PercentageChange("Percentage Change +50% Policy", 50.0, TimeSpan.Zero);
                Assert.AreEqual(PolicyType.Webhook, policyConfiguration.PolicyType);
                policy = await provider.CreatePolicyAsync(scalingGroup.Id, policyConfiguration, cancellationTokenSource.Token);

                await provider.ExecutePolicyAsync(scalingGroup.Id, policy.Id, cancellationTokenSource.Token);

                await provider.DeletePolicyAsync(scalingGroup.Id, policy.Id, cancellationTokenSource.Token);

                /* Percentage Change At Time
                 */
                policyConfiguration = PolicyConfiguration.PercentageChangeAtTime("Percentage Change -50% At Time Policy", -50.0, TimeSpan.Zero, DateTimeOffset.UtcNow.AddMinutes(30));
                Assert.AreEqual(PolicyType.Schedule, policyConfiguration.PolicyType);
                policy = await provider.CreatePolicyAsync(scalingGroup.Id, policyConfiguration, cancellationTokenSource.Token);

                await provider.ExecutePolicyAsync(scalingGroup.Id, policy.Id, cancellationTokenSource.Token);

                await provider.DeletePolicyAsync(scalingGroup.Id, policy.Id, cancellationTokenSource.Token);

                /* Cleanup
                 */
                await provider.DeleteGroupAsync(scalingGroup.Id, false, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestExecuteAnonymousWebhook()
        {
            IAutoScaleService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                FlavorId flavorId = await GetDefaultFlavorIdAsync(provider, cancellationTokenSource.Token);
                ImageId imageId = await GetDefaultImageIdAsync(provider, cancellationTokenSource.Token);

                string groupName = CreateRandomScalingGroupName();
                string serverName = UserComputeTests.UnitTestServerPrefix + groupName + '-';
                GroupConfiguration groupConfiguration = new GroupConfiguration(name: groupName, cooldown: TimeSpan.Zero, minEntities: 0, maxEntities: 1, metadata: new JObject());
                LaunchConfiguration launchConfiguration = new ServerLaunchConfiguration(new ServerLaunchArguments(new ServerArgument(flavorId, imageId, serverName, null, null)));
                PolicyConfiguration[] policyConfigurations = { PolicyConfiguration.Capacity("Capacity 1 Policy", 1, TimeSpan.Zero) };
                ScalingGroupConfiguration configuration = new ScalingGroupConfiguration(groupConfiguration, launchConfiguration, policyConfigurations);
                ScalingGroup scalingGroup = await provider.CreateGroupAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(scalingGroup);
                Assert.IsNotNull(scalingGroup.Id);
                Assert.AreEqual(0, scalingGroup.State.DesiredCapacity);

                // create a webhook
                NewWebhookConfiguration webhookConfiguration = new NewWebhookConfiguration("Test Webhook");
                Webhook webhook = await provider.CreateWebhookAsync(scalingGroup.Id, scalingGroup.ScalingPolicies[0].Id, webhookConfiguration, cancellationTokenSource.Token);

                // execute the webhook anonymously
                Link capabilityLink = webhook.Links.FirstOrDefault(i => string.Equals(i.Rel, "capability", StringComparison.OrdinalIgnoreCase));
                Assert.IsNotNull(capabilityLink);
                Uri capability = new Uri(capabilityLink.Href, UriKind.Absolute);
                HttpWebRequest request = WebRequest.CreateHttp(capability);
                request.Method = "POST";
                Console.Error.WriteLine("{0} (Request) {1} {2}", DateTime.Now, request.Method, request.RequestUri);
                WebResponse response = await request.GetResponseAsync(cancellationTokenSource.Token);
                Console.Error.WriteLine("{0} (Result {1}) {2}", DateTime.Now, ((HttpWebResponse)response).StatusCode, response.ResponseUri);
                foreach (string header in response.Headers)
                    Console.Error.WriteLine(string.Format("{0}: {1}", header, response.Headers[header]));
                string result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                if (!string.IsNullOrEmpty(result))
                    Console.Error.WriteLine("==> " + result);

                // verify successful execution
                GroupState groupState = await provider.GetGroupStateAsync(scalingGroup.Id, cancellationTokenSource.Token);
                Assert.AreEqual(1, groupState.DesiredCapacity);

                /* Cleanup
                 */
                await provider.SetGroupConfigurationAsync(scalingGroup.Id, new GroupConfiguration(name: groupName, cooldown: TimeSpan.Zero, minEntities: 0, maxEntities: 0, metadata: new JObject()), cancellationTokenSource.Token);
                await provider.DeleteGroupAsync(scalingGroup.Id, false, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestListWebhooks()
        {
            IAutoScaleService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                ScalingGroup[] scalingGroups = await ListAllScalingGroupsAsync(provider, null, cancellationTokenSource.Token);
                if (scalingGroups.Length == 0)
                    Assert.Inconclusive("The service did not report any scaling groups.");

                bool foundPolicy = false;
                bool foundWebhook = false;
                foreach (ScalingGroup scalingGroup in scalingGroups)
                {
                    Console.WriteLine("Scaling group '{0}' ({1})", scalingGroup.State.Name, scalingGroup.Id);
                    Policy[] policies = await ListAllPoliciesAsync(provider, scalingGroup.Id, null, cancellationTokenSource.Token);
                    foreach (Policy policy in policies)
                    {
                        foundPolicy = true;
                        Console.WriteLine("    Policy '{0}' ({1})", policy.Name, policy.Id);
                        Webhook[] webhooks = await ListAllWebhooksAsync(provider, scalingGroup.Id, policy.Id, null, cancellationTokenSource.Token);
                        foreach (Webhook webhook in webhooks)
                        {
                            foundWebhook = true;
                            Console.WriteLine("        Webhook '{0}' ({1})", webhook.Name, webhook.Id);
                        }
                    }
                }

                if (!foundPolicy)
                    Assert.Inconclusive("The service did not report any policies.");
                if (!foundWebhook)
                    Assert.Inconclusive("The service did not report any webhooks.");
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestCreateWebhook()
        {
            IAutoScaleService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                FlavorId flavorId = await GetDefaultFlavorIdAsync(provider, cancellationTokenSource.Token);
                ImageId imageId = await GetDefaultImageIdAsync(provider, cancellationTokenSource.Token);

                string groupName = CreateRandomScalingGroupName();
                string serverName = UserComputeTests.UnitTestServerPrefix + groupName + '-';
                GroupConfiguration groupConfiguration = new GroupConfiguration(name: groupName, cooldown: TimeSpan.Zero, minEntities: 0, maxEntities: 1, metadata: new JObject());
                LaunchConfiguration launchConfiguration = new ServerLaunchConfiguration(new ServerLaunchArguments(new ServerArgument(flavorId, imageId, serverName, null, null)));
                PolicyConfiguration[] policyConfigurations = { PolicyConfiguration.Capacity("Capacity 1 Policy", 1, TimeSpan.Zero) };
                ScalingGroupConfiguration configuration = new ScalingGroupConfiguration(groupConfiguration, launchConfiguration, policyConfigurations);
                ScalingGroup scalingGroup = await provider.CreateGroupAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(scalingGroup);
                Assert.IsNotNull(scalingGroup.Id);
                Assert.AreEqual(0, scalingGroup.State.DesiredCapacity);

                // create a webhook
                NewWebhookConfiguration webhookConfiguration = new NewWebhookConfiguration("Test Webhook");
                Webhook webhook = await provider.CreateWebhookAsync(scalingGroup.Id, scalingGroup.ScalingPolicies[0].Id, webhookConfiguration, cancellationTokenSource.Token);
                Assert.IsNotNull(webhook);
                Assert.IsNotNull(webhook.Id);

                // delete the webhook
                await provider.DeleteWebhookAsync(scalingGroup.Id, scalingGroup.ScalingPolicies[0].Id, webhook.Id, cancellationTokenSource.Token);

                /* Cleanup
                 */
                await provider.DeleteGroupAsync(scalingGroup.Id, false, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestCreateWebhookRange()
        {
            IAutoScaleService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                FlavorId flavorId = await GetDefaultFlavorIdAsync(provider, cancellationTokenSource.Token);
                ImageId imageId = await GetDefaultImageIdAsync(provider, cancellationTokenSource.Token);

                string groupName = CreateRandomScalingGroupName();
                string serverName = UserComputeTests.UnitTestServerPrefix + groupName + '-';
                GroupConfiguration groupConfiguration = new GroupConfiguration(name: groupName, cooldown: TimeSpan.Zero, minEntities: 0, maxEntities: 1, metadata: new JObject());
                LaunchConfiguration launchConfiguration = new ServerLaunchConfiguration(new ServerLaunchArguments(new ServerArgument(flavorId, imageId, serverName, null, null)));
                PolicyConfiguration[] policyConfigurations = { PolicyConfiguration.Capacity("Capacity 1 Policy", 1, TimeSpan.Zero) };
                ScalingGroupConfiguration configuration = new ScalingGroupConfiguration(groupConfiguration, launchConfiguration, policyConfigurations);
                ScalingGroup scalingGroup = await provider.CreateGroupAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(scalingGroup);
                Assert.IsNotNull(scalingGroup.Id);
                Assert.AreEqual(0, scalingGroup.State.DesiredCapacity);

                // create a webhook
                NewWebhookConfiguration[] webhookConfigurations =
                    {
                        new NewWebhookConfiguration("Test Webhook 1"),
                        new NewWebhookConfiguration("Test Webhook 2"),
                        new NewWebhookConfiguration("Test Webhook 3")
                    };
                ReadOnlyCollection<Webhook> webhooks = await provider.CreateWebhookRangeAsync(scalingGroup.Id, scalingGroup.ScalingPolicies[0].Id, webhookConfigurations, cancellationTokenSource.Token);
                Assert.IsNotNull(webhooks);
                Assert.AreEqual(webhookConfigurations.Length, webhooks.Count);

                /* Cleanup
                 */
                await provider.DeleteGroupAsync(scalingGroup.Id, false, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestGetWebhook()
        {
            IAutoScaleService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                FlavorId flavorId = await GetDefaultFlavorIdAsync(provider, cancellationTokenSource.Token);
                ImageId imageId = await GetDefaultImageIdAsync(provider, cancellationTokenSource.Token);

                string groupName = CreateRandomScalingGroupName();
                string serverName = UserComputeTests.UnitTestServerPrefix + groupName + '-';
                GroupConfiguration groupConfiguration = new GroupConfiguration(name: groupName, cooldown: TimeSpan.Zero, minEntities: 0, maxEntities: 1, metadata: new JObject());
                LaunchConfiguration launchConfiguration = new ServerLaunchConfiguration(new ServerLaunchArguments(new ServerArgument(flavorId, imageId, serverName, null, null)));
                PolicyConfiguration[] policyConfigurations = { PolicyConfiguration.Capacity("Capacity 1 Policy", 1, TimeSpan.Zero) };
                ScalingGroupConfiguration configuration = new ScalingGroupConfiguration(groupConfiguration, launchConfiguration, policyConfigurations);
                ScalingGroup scalingGroup = await provider.CreateGroupAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(scalingGroup);
                Assert.IsNotNull(scalingGroup.Id);
                Assert.AreEqual(0, scalingGroup.State.DesiredCapacity);

                // create a webhook
                NewWebhookConfiguration webhookConfiguration = new NewWebhookConfiguration("Test Webhook");
                Webhook webhook = await provider.CreateWebhookAsync(scalingGroup.Id, scalingGroup.ScalingPolicies[0].Id, webhookConfiguration, cancellationTokenSource.Token);
                Assert.IsNotNull(webhook);
                Assert.IsNotNull(webhook.Id);

                // get the webhook
                Webhook singleWebhook = await provider.GetWebhookAsync(scalingGroup.Id, scalingGroup.ScalingPolicies[0].Id, webhook.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(singleWebhook);
                Assert.AreEqual(webhook.Id, singleWebhook.Id);
                Assert.AreEqual(webhook.Name, singleWebhook.Name);

                // delete the webhook
                await provider.DeleteWebhookAsync(scalingGroup.Id, scalingGroup.ScalingPolicies[0].Id, webhook.Id, cancellationTokenSource.Token);

                /* Cleanup
                 */
                await provider.DeleteGroupAsync(scalingGroup.Id, false, cancellationTokenSource.Token);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.AutoScale)]
        public async Task TestUpdateWebhook()
        {
            IAutoScaleService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(300))))
            {
                FlavorId flavorId = await GetDefaultFlavorIdAsync(provider, cancellationTokenSource.Token);
                ImageId imageId = await GetDefaultImageIdAsync(provider, cancellationTokenSource.Token);

                string groupName = CreateRandomScalingGroupName();
                string serverName = UserComputeTests.UnitTestServerPrefix + groupName + '-';
                GroupConfiguration groupConfiguration = new GroupConfiguration(name: groupName, cooldown: TimeSpan.Zero, minEntities: 0, maxEntities: 1, metadata: new JObject());
                LaunchConfiguration launchConfiguration = new ServerLaunchConfiguration(new ServerLaunchArguments(new ServerArgument(flavorId, imageId, serverName, null, null)));
                PolicyConfiguration[] policyConfigurations = { PolicyConfiguration.Capacity("Capacity 1 Policy", 1, TimeSpan.Zero) };
                ScalingGroupConfiguration configuration = new ScalingGroupConfiguration(groupConfiguration, launchConfiguration, policyConfigurations);
                ScalingGroup scalingGroup = await provider.CreateGroupAsync(configuration, cancellationTokenSource.Token);
                Assert.IsNotNull(scalingGroup);
                Assert.IsNotNull(scalingGroup.Id);
                Assert.AreEqual(0, scalingGroup.State.DesiredCapacity);

                // create a webhook
                NewWebhookConfiguration webhookConfiguration = new NewWebhookConfiguration("Test Webhook");
                Webhook webhook = await provider.CreateWebhookAsync(scalingGroup.Id, scalingGroup.ScalingPolicies[0].Id, webhookConfiguration, cancellationTokenSource.Token);
                Assert.IsNotNull(webhook);
                Assert.IsNotNull(webhook.Id);

                // update the webhook name
                string updatedName = "Updated Webhook";
                await provider.UpdateWebhookAsync(scalingGroup.Id, scalingGroup.ScalingPolicies[0].Id, webhook.Id, new UpdateWebhookConfiguration(updatedName), cancellationTokenSource.Token);

                // verify the update
                Webhook singleWebhook = await provider.GetWebhookAsync(scalingGroup.Id, scalingGroup.ScalingPolicies[0].Id, webhook.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(singleWebhook);
                Assert.AreEqual(webhook.Id, singleWebhook.Id);
                Assert.AreEqual(updatedName, singleWebhook.Name);
                Assert.AreEqual(0, singleWebhook.Metadata.Count);

                // update the webhook metadata
                Dictionary<string, string> updatedMetadata = new Dictionary<string, string> { { "My metadata", "Value ­²" } };
                await provider.UpdateWebhookAsync(scalingGroup.Id, scalingGroup.ScalingPolicies[0].Id, webhook.Id, new UpdateWebhookConfiguration(updatedMetadata), cancellationTokenSource.Token);

                // verify the update
                singleWebhook = await provider.GetWebhookAsync(scalingGroup.Id, scalingGroup.ScalingPolicies[0].Id, webhook.Id, cancellationTokenSource.Token);
                Assert.IsNotNull(singleWebhook);
                Assert.AreEqual(webhook.Id, singleWebhook.Id);
                Assert.AreEqual(updatedName, singleWebhook.Name);
                CollectionAssert.AreEqual(updatedMetadata, singleWebhook.Metadata);

                // delete the webhook
                await provider.DeleteWebhookAsync(scalingGroup.Id, scalingGroup.ScalingPolicies[0].Id, webhook.Id, cancellationTokenSource.Token);

                /* Cleanup
                 */
                await provider.DeleteGroupAsync(scalingGroup.Id, false, cancellationTokenSource.Token);
            }
        }

        protected static async Task<ScalingGroup[]> ListAllScalingGroupsAsync(IAutoScaleService service, int? blockSize, CancellationToken cancellationToken, net.openstack.Core.IProgress<ReadOnlyCollection<ScalingGroup>> progress = null)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            List<ScalingGroup> result = new List<ScalingGroup>();
            ScalingGroupId marker = null;

            do
            {
                ReadOnlyCollection<ScalingGroup> page = await service.ListScalingGroupsAsync(marker, blockSize, cancellationToken);
                if (progress != null)
                    progress.Report(page);

                result.AddRange(page);
                marker = page.Count > 0 ? page[page.Count - 1].Id : null;
            } while (marker != null);

            return result.ToArray();
        }

        protected static async Task<Policy[]> ListAllPoliciesAsync(IAutoScaleService service, ScalingGroupId groupId, int? blockSize, CancellationToken cancellationToken, net.openstack.Core.IProgress<ReadOnlyCollection<Policy>> progress = null)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            List<Policy> result = new List<Policy>();
            PolicyId marker = null;

            do
            {
                ReadOnlyCollection<Policy> page = await service.ListPoliciesAsync(groupId, marker, blockSize, cancellationToken);
                if (progress != null)
                    progress.Report(page);

                result.AddRange(page);
                marker = page.Count > 0 ? page[page.Count - 1].Id : null;
            } while (marker != null);

            return result.ToArray();
        }

        protected static async Task<Webhook[]> ListAllWebhooksAsync(IAutoScaleService service, ScalingGroupId groupId, PolicyId policyId, int? blockSize, CancellationToken cancellationToken, net.openstack.Core.IProgress<ReadOnlyCollection<Webhook>> progress = null)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            List<Webhook> result = new List<Webhook>();
            WebhookId marker = null;

            do
            {
                ReadOnlyCollection<Webhook> page = await service.ListWebhooksAsync(groupId, policyId, marker, blockSize, cancellationToken);
                if (progress != null)
                    progress.Report(page);

                result.AddRange(page);
                marker = page.Count > 0 ? page[page.Count - 1].Id : null;
            } while (marker != null);

            return result.ToArray();
        }

        private Task<FlavorId> GetDefaultFlavorIdAsync(IAutoScaleService provider, CancellationToken cancellationToken)
        {
            IComputeProvider computeProvider = Bootstrapper.CreateComputeProvider();
            return Task.Run(
                () =>
                {
                    FlavorDetails[] flavors = computeProvider.ListFlavorsWithDetails().OrderBy(i => i.RAMInMB).ThenBy(i => i.DiskSizeInGB).ToArray();

                    // first, try to find a performance flavor
                    foreach (FlavorDetails flavor in flavors)
                    {
                        if (flavor.Id.Contains("performance"))
                            return new FlavorId(flavor.Id);
                    }

                    // otherwise find the smallest
                    return new FlavorId(flavors[0].Id);
                }, cancellationToken);
        }

        private Task<ImageId> GetDefaultImageIdAsync(IAutoScaleService provider, CancellationToken cancellationToken)
        {
            IComputeProvider computeProvider = Bootstrapper.CreateComputeProvider();
            return Task.Run(
                () =>
                {
                    ServerImage[] images = computeProvider.ListImagesWithDetails().ToArray();

                    // first, try to find a CentOS 6.4 image
                    foreach (ServerImage image in images)
                    {
                        if (image.Name.Contains(UserServerTests.TestImageNameSubstring))
                            return new ImageId(image.Id);
                    }

                    // otherwise find the first CentOS image
                    return new ImageId(images.First(i => i.Name.Contains("CentOS")).Id);
                }, cancellationToken);
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
        /// Creates a random scaling group name with the proper prefix for integration testing.
        /// </summary>
        /// <returns>A unique, randomly-generated scaling group name.</returns>
        internal static string CreateRandomScalingGroupName()
        {
            return TestScalingGroupPrefix + Path.GetRandomFileName();
        }

        /// <summary>
        /// Creates an instance of <see cref="IAutoScaleService"/> for testing using
        /// the <see cref="OpenstackNetSetings.TestIdentity"/>.
        /// </summary>
        /// <returns>An instance of <see cref="IAutoScaleService"/> for integration testing.</returns>
        internal static IAutoScaleService CreateProvider()
        {
            var provider = new TestCloudAutoScaleProvider(Bootstrapper.Settings.TestIdentity, Bootstrapper.Settings.DefaultRegion, null);
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

        internal class TestCloudAutoScaleProvider : CloudAutoScaleProvider
        {
            public TestCloudAutoScaleProvider(CloudIdentity defaultIdentity, string defaultRegion, IIdentityProvider identityProvider)
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
                    LogResult(result.Item1, result.Item2, true);
                    return result;
                }
                catch (WebException ex)
                {
                    HttpWebResponse response = ex.Response as HttpWebResponse;
                    if (response != null && response.ContentLength != 0)
                        LogResult(response, ex.Message, true);

                    throw;
                }
            }

            private void LogResult(HttpWebResponse response, string rawBody, bool reformat)
            {
                foreach (string header in response.Headers)
                {
                    Console.Error.WriteLine(string.Format("{0}: {1}", header, response.Headers[header]));
                }

                if (!string.IsNullOrEmpty(rawBody))
                {
                    if (reformat)
                    {
                        object parsed = JsonConvert.DeserializeObject(rawBody);
                        Console.Error.WriteLine("==> " + JsonConvert.SerializeObject(parsed, Formatting.Indented));
                    }
                    else
                    {
                        Console.Error.WriteLine("==> " + rawBody);
                    }
                }
            }
        }
    }
}
