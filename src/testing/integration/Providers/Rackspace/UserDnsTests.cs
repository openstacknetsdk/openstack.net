namespace Net.OpenStack.Testing.Integration.Providers.Rackspace
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using net.openstack.Core;
    using net.openstack.Core.Collections;
    using net.openstack.Core.Domain;
    using net.openstack.Core.Providers;
    using net.openstack.Providers.Rackspace;
    using net.openstack.Providers.Rackspace.Objects.Dns;
    using net.openstack.Providers.Rackspace.Objects.LoadBalancers;
    using Newtonsoft.Json;
    using Encoding = System.Text.Encoding;
    using Path = System.IO.Path;

    [TestClass]
    public class UserDnsTests
    {
        /// <summary>
        /// Domains created by these unit tests have this prefix in the name, allowing
        /// them to be identified and cleaned up following a failed test.
        /// </summary>
        private static readonly string TestDomainPrefix = "UnitTestDomain-";

        /// <summary>
        /// This method can be used to clean up domains created during integration testing.
        /// </summary>
        /// <remarks>
        /// The Cloud DNS integration tests generally delete domains created during the
        /// tests, but test failures may lead to unused domains gathering on the system.
        /// This method searches for all domains matching the "integration testing"
        /// pattern (i.e., domains whose name starts with <see cref="TestDomainPrefix"/>),
        /// and attempts to delete them.
        /// <para>
        /// The deletion requests are sent in parallel, so a single deletion failure will
        /// not prevent the method from cleaning up other queues that can be successfully
        /// deleted. Note that the system does not increase the
        /// <see cref="ProviderBase{TProvider}.ConnectionLimit"/>, so the underlying REST
        /// requests may be pipelined if the number of domains to delete exceeds the
        /// default system connection limit.
        /// </para>
        /// </remarks>
        [TestMethod]
        [TestCategory(TestCategories.Cleanup)]
        public async Task CleanupTestDomains()
        {
            const int BatchSize = 10;

            IDnsService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(60))))
            {
                Func<DnsDomain, bool> domainFilter =
                    domain =>
                    {
                        if (domain.Name.StartsWith(TestDomainPrefix, StringComparison.OrdinalIgnoreCase))
                            return true;
                        else if (domain.Name.IndexOf('.' + TestDomainPrefix, StringComparison.OrdinalIgnoreCase) >= 0)
                            return true;

                        return false;
                    };

                DnsDomain[] allDomains = (await ListAllDomainsAsync(provider, null, null, cancellationTokenSource.Token)).Where(domainFilter).ToArray();

                List<Task> deleteTasks = new List<Task>();
                for (int i = 0; i < allDomains.Length; i += BatchSize)
                {
                    for (int j = i; j < i + BatchSize && j < allDomains.Length; j++)
                        Console.WriteLine("Deleting domain: {0}", allDomains[j].Name);

                    deleteTasks.Add(provider.RemoveDomainsAsync(allDomains.Skip(i).Take(BatchSize).Select(domain => domain.Id), true, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null));
                }

                Task.WaitAll(deleteTasks.ToArray());
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Dns)]
        public async Task TestListLimits()
        {
            IDnsService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(10))))
            {
                DnsServiceLimits limits = await provider.ListLimitsAsync(cancellationTokenSource.Token);
                Assert.IsNotNull(limits);
                Assert.IsNotNull(limits.RateLimits);
                Assert.IsNotNull(limits.AbsoluteLimits);

                Console.WriteLine(await JsonConvert.SerializeObjectAsync(limits, Formatting.Indented));
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Dns)]
        public async Task TestListLimitTypes()
        {
            IDnsService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(10))))
            {
                IEnumerable<LimitType> limitTypes = await provider.ListLimitTypesAsync(cancellationTokenSource.Token);
                Assert.IsNotNull(limitTypes);

                if (!limitTypes.Any())
                    Assert.Inconclusive("No limit types were returned by the server");

                foreach (var limitType in limitTypes)
                    Console.WriteLine(limitType.Name);

                Assert.IsTrue(limitTypes.Contains(LimitType.Rate));
                Assert.IsTrue(limitTypes.Contains(LimitType.Domain));
                Assert.IsTrue(limitTypes.Contains(LimitType.DomainRecord));
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Dns)]
        public async Task TestListSpecificLimit()
        {
            IDnsService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(10))))
            {
                IEnumerable<LimitType> limitTypes = await provider.ListLimitTypesAsync(cancellationTokenSource.Token);
                Assert.IsNotNull(limitTypes);

                if (!limitTypes.Any())
                    Assert.Inconclusive("No limit types were returned by the server");

                foreach (var limitType in limitTypes)
                {
                    Console.WriteLine();
                    Console.WriteLine("Limit Type: {0}", limitType);
                    Console.WriteLine();
                    DnsServiceLimits limits = await provider.ListLimitsAsync(limitType, cancellationTokenSource.Token);
                    Console.WriteLine(await JsonConvert.SerializeObjectAsync(limits, Formatting.Indented));
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Dns)]
        public async Task TestListDomains()
        {
            IDnsService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(30))))
            {
                ReadOnlyCollection<DnsDomain> domains = await ListAllDomainsAsync(provider, null, null, cancellationTokenSource.Token);
                Assert.IsNotNull(domains);

                if (!domains.Any())
                    Assert.Inconclusive("No domains were returned by the server");

                foreach (var domain in domains)
                {
                    Console.WriteLine();
                    Console.WriteLine("Domain: {0} ({1})", domain.Name, domain.Id);
                    Console.WriteLine();
                    Console.WriteLine(await JsonConvert.SerializeObjectAsync(domain, Formatting.Indented));
                }
            }
        }

        /// <summary>
        /// This tests the behavior of the <see cref="IDnsService.CreateDomainsAsync"/> and
        /// <see cref="IDnsService.RemoveDomainsAsync"/> when performed on a single domain.
        /// </summary>
        /// <returns>A <see cref="Task"/> object representing the asynchronous operation.</returns>
        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Dns)]
        public async Task TestCreateDomain()
        {
            string domainName = CreateRandomDomainName();

            IDnsService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(60))))
            {
                DnsConfiguration configuration = new DnsConfiguration(
                    new DnsDomainConfiguration(
                        name: domainName,
                        timeToLive: default(TimeSpan?),
                        emailAddress: "admin@" + domainName,
                        comment: "Integration test domain",
                        records: new DnsDomainRecordConfiguration[] { },
                        subdomains: new DnsSubdomainConfiguration[] { }));

                DnsJob<DnsDomains> createResponse = await provider.CreateDomainsAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                IEnumerable<DnsDomain> createdDomains = Enumerable.Empty<DnsDomain>();
                if (createResponse.Status == DnsJobStatus.Error)
                {
                    Console.WriteLine(createResponse.Error.ToString(Formatting.Indented));
                    Assert.Fail();
                }
                else
                {
                    Console.WriteLine(JsonConvert.SerializeObject(createResponse.Response, Formatting.Indented));
                    createdDomains = createResponse.Response.Domains;
                }

                ReadOnlyCollection<DnsDomain> domains = await ListAllDomainsAsync(provider, domainName, null, cancellationTokenSource.Token);
                Assert.IsNotNull(domains);

                if (!domains.Any())
                    Assert.Inconclusive("No domains were returned by the server");

                foreach (var domain in domains)
                {
                    Console.WriteLine();
                    Console.WriteLine("Domain: {0} ({1})", domain.Name, domain.Id);
                    Console.WriteLine();
                    Console.WriteLine(await JsonConvert.SerializeObjectAsync(domain, Formatting.Indented));
                }

                DnsJob deleteResponse = await provider.RemoveDomainsAsync(createdDomains.Select(i => i.Id), false, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                if (deleteResponse.Status == DnsJobStatus.Error)
                {
                    Console.WriteLine(deleteResponse.Error.ToString(Formatting.Indented));
                    Assert.Fail("Failed to delete temporary domain created during the integration test.");
                }
            }
        }

        /// <summary>
        /// This tests the behavior of the <see cref="IDnsService.UpdateDomainsAsync"/> when performed
        /// on a single domain.
        /// </summary>
        /// <returns>A <see cref="Task"/> object representing the asynchronous operation.</returns>
        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Dns)]
        public async Task TestUpdateDomain()
        {
            string domainName = CreateRandomDomainName();

            IDnsService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(60))))
            {
                DnsConfiguration configuration = new DnsConfiguration(
                    new DnsDomainConfiguration(
                        name: domainName,
                        timeToLive: default(TimeSpan?),
                        emailAddress: "admin@" + domainName,
                        comment: "Integration test domain",
                        records: new DnsDomainRecordConfiguration[] { },
                        subdomains: new DnsSubdomainConfiguration[] { }));

                DnsJob<DnsDomains> createResponse = await provider.CreateDomainsAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                IEnumerable<DnsDomain> createdDomains = Enumerable.Empty<DnsDomain>();
                if (createResponse.Status == DnsJobStatus.Error)
                {
                    Console.WriteLine(createResponse.Error.ToString(Formatting.Indented));
                    Assert.Fail();
                }
                else
                {
                    Console.WriteLine(JsonConvert.SerializeObject(createResponse.Response, Formatting.Indented));
                    createdDomains = createResponse.Response.Domains;
                }

                ReadOnlyCollection<DnsDomain> domains = await ListAllDomainsAsync(provider, domainName, null, cancellationTokenSource.Token);
                Assert.IsNotNull(domains);

                if (!domains.Any())
                    Assert.Inconclusive("No domains were returned by the server");

                DnsUpdateConfiguration updateConfiguration = new DnsUpdateConfiguration(
                    new DnsDomainUpdateConfiguration(domains[0], comment: "Integration test domain 2"));
                await provider.UpdateDomainsAsync(updateConfiguration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                DnsJob deleteResponse = await provider.RemoveDomainsAsync(createdDomains.Select(i => i.Id), false, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                if (deleteResponse.Status == DnsJobStatus.Error)
                {
                    Console.WriteLine(deleteResponse.Error.ToString(Formatting.Indented));
                    Assert.Fail("Failed to delete temporary domain created during the integration test.");
                }
            }
        }

        /// <summary>
        /// This tests the behavior of the <see cref="IDnsService.CloneDomainsAsync"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> object representing the asynchronous operation.</returns>
        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Dns)]
        public async Task TestCloneDomain()
        {
            string domainName = CreateRandomDomainName();
            string clonedName = CreateRandomDomainName();

            IDnsService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(60))))
            {
                List<DomainId> domainsToRemove = new List<DomainId>();

                DnsConfiguration configuration = new DnsConfiguration(
                    new DnsDomainConfiguration(
                        name: domainName,
                        timeToLive: default(TimeSpan?),
                        emailAddress: "admin@" + domainName,
                        comment: "Integration test domain",
                        records: new DnsDomainRecordConfiguration[] { },
                        subdomains: new DnsSubdomainConfiguration[] { }));

                DnsJob<DnsDomains> createResponse = await provider.CreateDomainsAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                if (createResponse.Status == DnsJobStatus.Error)
                {
                    Console.WriteLine(createResponse.Error.ToString(Formatting.Indented));
                    Assert.Fail();
                }
                else
                {
                    Console.WriteLine(JsonConvert.SerializeObject(createResponse.Response, Formatting.Indented));
                    domainsToRemove.AddRange(createResponse.Response.Domains.Select(i => i.Id));
                }

                DnsJob<DnsDomains> cloneResponse = await provider.CloneDomainAsync(createResponse.Response.Domains[0].Id, clonedName, true, true, true, true, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                if (cloneResponse.Status == DnsJobStatus.Error)
                {
                    Console.WriteLine(cloneResponse.Error.ToString(Formatting.Indented));
                    Assert.Fail();
                }
                else
                {
                    Console.WriteLine(JsonConvert.SerializeObject(cloneResponse.Response, Formatting.Indented));
                    domainsToRemove.AddRange(cloneResponse.Response.Domains.Select(i => i.Id));
                }

                DnsJob deleteResponse = await provider.RemoveDomainsAsync(domainsToRemove, false, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                if (deleteResponse.Status == DnsJobStatus.Error)
                {
                    Console.WriteLine(deleteResponse.Error.ToString(Formatting.Indented));
                    Assert.Fail("Failed to delete temporary domain created during the integration test.");
                }
            }
        }

        /// <summary>
        /// This tests the behavior of the <see cref="IDnsService.ExportDomainAsync"/> and <see cref="IDnsService.ImportDomainAsync"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> object representing the asynchronous operation.</returns>
        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Dns)]
        public async Task TestDomainExportImport()
        {
            string domainName = CreateRandomDomainName();

            IDnsService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(60))))
            {
                List<DomainId> domainsToRemove = new List<DomainId>();

                DnsConfiguration configuration = new DnsConfiguration(
                    new DnsDomainConfiguration(
                        name: domainName,
                        timeToLive: default(TimeSpan?),
                        emailAddress: "admin@" + domainName,
                        comment: "Integration test domain",
                        records: new DnsDomainRecordConfiguration[] { },
                        subdomains: new DnsSubdomainConfiguration[] { }));

                DnsJob<DnsDomains> createResponse = await provider.CreateDomainsAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                if (createResponse.Status == DnsJobStatus.Error)
                {
                    Console.WriteLine(createResponse.Error.ToString(Formatting.Indented));
                    Assert.Fail("Failed to create a test domain.");
                }
                else
                {
                    Console.WriteLine(JsonConvert.SerializeObject(createResponse.Response, Formatting.Indented));
                    domainsToRemove.AddRange(createResponse.Response.Domains.Select(i => i.Id));
                }

                DnsJob<ExportedDomain> exportedDomain = await provider.ExportDomainAsync(createResponse.Response.Domains[0].Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                if (exportedDomain.Status == DnsJobStatus.Error)
                {
                    Console.WriteLine(exportedDomain.Error.ToString(Formatting.Indented));
                    Assert.Fail("Failed to export test domain.");
                }

                Assert.AreEqual(DnsJobStatus.Completed, exportedDomain.Status);
                Assert.IsNotNull(exportedDomain.Response);

                Console.WriteLine("Exported domain:");
                Console.WriteLine(JsonConvert.SerializeObject(exportedDomain.Response, Formatting.Indented));
                Console.WriteLine();
                Console.WriteLine("Formatted exported output:");
                Console.WriteLine(exportedDomain.Response.Contents);

                Assert.IsNotNull(exportedDomain.Response.Id);
                Assert.IsNotNull(exportedDomain.Response.AccountId);
                Assert.IsFalse(string.IsNullOrEmpty(exportedDomain.Response.Contents));
                Assert.IsNotNull(exportedDomain.Response.ContentType);

                DnsJob deleteResponse = await provider.RemoveDomainsAsync(domainsToRemove, false, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                if (deleteResponse.Status == DnsJobStatus.Error)
                {
                    Console.WriteLine(deleteResponse.Error.ToString(Formatting.Indented));
                    Assert.Fail("Failed to delete temporary domain created during the integration test.");
                }

                domainsToRemove.Clear();

                SerializedDomain serializedDomain =
                    new SerializedDomain(
                        RemoveDefaultNameserverEntries(exportedDomain.Response.Contents),
                        exportedDomain.Response.ContentType);
                DnsJob<DnsDomains> importedDomain = await provider.ImportDomainAsync(new[] { serializedDomain }, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                if (importedDomain.Status == DnsJobStatus.Error)
                {
                    Console.WriteLine(importedDomain.Error.ToString(Formatting.Indented));
                    Assert.Fail("Failed to import the test domain.");
                }
                else
                {
                    Console.WriteLine(JsonConvert.SerializeObject(importedDomain.Response, Formatting.Indented));
                    domainsToRemove.AddRange(importedDomain.Response.Domains.Select(i => i.Id));
                }

                deleteResponse = await provider.RemoveDomainsAsync(domainsToRemove, false, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                if (deleteResponse.Status == DnsJobStatus.Error)
                {
                    Console.WriteLine(deleteResponse.Error.ToString(Formatting.Indented));
                    Assert.Fail("Failed to delete temporary domain created during the integration test.");
                }
            }
        }

        /// <summary>
        /// This method removes the default NS entries from an exported domain in BIND 9 format.
        /// Attempting to import a domain containing these entries results in an error due to
        /// conflicting DNS entries.
        /// </summary>
        /// <param name="bind9Content">The exported DNS domain in BIND 9 format, which may contain default NS records.</param>
        /// <returns>The serialized domain with the default NS records removed.</returns>
        private static string RemoveDefaultNameserverEntries(string bind9Content)
        {
            Regex expression = new Regex(@"\n.*\tNS\tdns[12]\.stabletransit\.com.*");
            return expression.Replace(bind9Content, string.Empty);
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Dns)]
        public async Task TestCreateSubdomain()
        {
            string domainName = CreateRandomDomainName();
            string subdomainName = "www";

            IDnsService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(60))))
            {
                DnsConfiguration configuration = new DnsConfiguration(
                    new DnsDomainConfiguration(
                        name: domainName,
                        timeToLive: default(TimeSpan?),
                        emailAddress: "admin@" + domainName,
                        comment: "Integration test domain",
                        records: new DnsDomainRecordConfiguration[] { },
                        subdomains: new DnsSubdomainConfiguration[]
                            {
                                new DnsSubdomainConfiguration(
                                    emailAddress: string.Format("sub-admin@{0}.{1}", subdomainName, domainName),
                                    name: string.Format("{0}.{1}", subdomainName, domainName),
                                    comment: "Integration test subdomain")
                            }));

                DnsJob<DnsDomains> createResponse = await provider.CreateDomainsAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                IEnumerable<DnsDomain> createdDomains = Enumerable.Empty<DnsDomain>();
                if (createResponse.Status == DnsJobStatus.Error)
                {
                    Console.WriteLine(createResponse.Error.ToString(Formatting.Indented));
                    Assert.Fail();
                }
                else
                {
                    Console.WriteLine(JsonConvert.SerializeObject(createResponse.Response, Formatting.Indented));
                    createdDomains = createResponse.Response.Domains;
                }

                ReadOnlyCollection<DnsDomain> domains = await ListAllDomainsAsync(provider, domainName, null, cancellationTokenSource.Token);
                Assert.IsNotNull(domains);

                if (!domains.Any())
                    Assert.Inconclusive("No domains were returned by the server");

                foreach (var domain in domains)
                {
                    Console.WriteLine();
                    Console.WriteLine("Domain: {0} ({1})", domain.Name, domain.Id);
                    Console.WriteLine();
                    Console.WriteLine(await JsonConvert.SerializeObjectAsync(domain, Formatting.Indented));
                }

                DomainId domainId = createResponse.Response.Domains[0].Id;
                ReadOnlyCollection<DnsSubdomain> subdomains = await ListAllSubdomainsAsync(provider, domainId, null, cancellationTokenSource.Token);
                Assert.IsNotNull(subdomains);
                Assert.AreEqual(1, subdomains.Count);
                foreach (var subdomain in subdomains)
                {
                    Console.WriteLine();
                    Console.WriteLine("Subdomain: {0} ({1})", subdomain.Name, subdomain.Id);
                    Console.WriteLine();
                    Console.WriteLine(await JsonConvert.SerializeObjectAsync(subdomain, Formatting.Indented));
                }

                DnsJob deleteResponse = await provider.RemoveDomainsAsync(createdDomains.Select(i => i.Id), true, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                if (deleteResponse.Status == DnsJobStatus.Error)
                {
                    Console.WriteLine(deleteResponse.Error.ToString(Formatting.Indented));
                    Assert.Fail("Failed to delete temporary domain created during the integration test.");
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Dns)]
        public async Task TestCreateRecords()
        {
            string domainName = CreateRandomDomainName();

            IDnsService provider = CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(60))))
            {
                DnsConfiguration configuration = new DnsConfiguration(
                    new DnsDomainConfiguration(
                        name: domainName,
                        timeToLive: default(TimeSpan?),
                        emailAddress: "admin@" + domainName,
                        comment: "Integration test domain",
                        records: new DnsDomainRecordConfiguration[] { },
                        subdomains: new DnsSubdomainConfiguration[] { }));

                DnsJob<DnsDomains> createResponse = await provider.CreateDomainsAsync(configuration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                IEnumerable<DnsDomain> createdDomains = Enumerable.Empty<DnsDomain>();
                if (createResponse.Status == DnsJobStatus.Error)
                {
                    Console.WriteLine(createResponse.Error.ToString(Formatting.Indented));
                    Assert.Fail();
                }
                else
                {
                    Console.WriteLine(JsonConvert.SerializeObject(createResponse.Response, Formatting.Indented));
                    createdDomains = createResponse.Response.Domains;
                }

                ReadOnlyCollection<DnsDomain> domains = await ListAllDomainsAsync(provider, domainName, null, cancellationTokenSource.Token);
                Assert.IsNotNull(domains);
                Assert.AreEqual(1, domains.Count);

                foreach (var domain in domains)
                {
                    Console.WriteLine();
                    Console.WriteLine("Domain: {0} ({1})", domain.Name, domain.Id);
                    Console.WriteLine();
                    Console.WriteLine(await JsonConvert.SerializeObjectAsync(domain, Formatting.Indented));
                }

                string originalData = "127.0.0.1";
                string updatedData = "192.168.0.1";
                string originalCommentValue = "Integration test record";
                string updatedCommentValue = "Integration test record 2";
                TimeSpan? originalTimeToLive;
                TimeSpan? updatedTimeToLive = TimeSpan.FromMinutes(12);

                DomainId domainId = createResponse.Response.Domains[0].Id;
                DnsDomainRecordConfiguration[] recordConfigurations =
                    {
                        new DnsDomainRecordConfiguration(
                            type: DnsRecordType.A,
                            name: string.Format("www.{0}", domainName),
                            data: originalData,
                            timeToLive: null,
                            comment: originalCommentValue,
                            priority: null)
                    };
                DnsJob<DnsRecordsList> recordsResponse = await provider.AddRecordsAsync(domainId, recordConfigurations, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                Assert.IsNotNull(recordsResponse.Response);
                Assert.IsNotNull(recordsResponse.Response.Records);
                DnsRecord[] records = recordsResponse.Response.Records.ToArray();
                Assert.AreEqual(recordConfigurations.Length, records.Length);
                originalTimeToLive = records[0].TimeToLive;
                Assert.AreNotEqual(originalTimeToLive, updatedTimeToLive);

                Assert.AreEqual(originalData, records[0].Data);
                Assert.AreEqual(originalTimeToLive, records[0].TimeToLive);
                Assert.AreEqual(originalCommentValue, records[0].Comment);

                foreach (var record in records)
                {
                    Console.WriteLine();
                    Console.WriteLine("Record: {0} ({1})", record.Name, record.Id);
                    Console.WriteLine();
                    Console.WriteLine(await JsonConvert.SerializeObjectAsync(record, Formatting.Indented));
                }

                DnsDomainRecordUpdateConfiguration recordUpdateConfiguration = new DnsDomainRecordUpdateConfiguration(records[0], records[0].Name, comment: updatedCommentValue);
                await provider.UpdateRecordsAsync(domainId, new[] { recordUpdateConfiguration }, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                DnsRecord updatedRecord = await provider.ListRecordDetailsAsync(domainId, records[0].Id, cancellationTokenSource.Token);
                Assert.IsNotNull(updatedRecord);
                Assert.AreEqual(originalData, updatedRecord.Data);
                Assert.AreEqual(originalTimeToLive, updatedRecord.TimeToLive);
                Assert.AreEqual(updatedCommentValue, updatedRecord.Comment);

                recordUpdateConfiguration = new DnsDomainRecordUpdateConfiguration(records[0], records[0].Name, timeToLive: updatedTimeToLive);
                await provider.UpdateRecordsAsync(domainId, new[] { recordUpdateConfiguration }, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                updatedRecord = await provider.ListRecordDetailsAsync(domainId, records[0].Id, cancellationTokenSource.Token);
                Assert.IsNotNull(updatedRecord);
                Assert.AreEqual(originalData, updatedRecord.Data);
                Assert.AreEqual(updatedTimeToLive, updatedRecord.TimeToLive);
                Assert.AreEqual(updatedCommentValue, updatedRecord.Comment);

                recordUpdateConfiguration = new DnsDomainRecordUpdateConfiguration(records[0], records[0].Name, data: updatedData);
                await provider.UpdateRecordsAsync(domainId, new[] { recordUpdateConfiguration }, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                updatedRecord = await provider.ListRecordDetailsAsync(domainId, records[0].Id, cancellationTokenSource.Token);
                Assert.IsNotNull(updatedRecord);
                Assert.AreEqual(updatedData, updatedRecord.Data);
                Assert.AreEqual(updatedTimeToLive, updatedRecord.TimeToLive);
                Assert.AreEqual(updatedCommentValue, updatedRecord.Comment);

                await provider.RemoveRecordsAsync(domainId, new[] { records[0].Id }, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                DnsJob deleteResponse = await provider.RemoveDomainsAsync(createdDomains.Select(i => i.Id), false, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                if (deleteResponse.Status == DnsJobStatus.Error)
                {
                    Console.WriteLine(deleteResponse.Error.ToString(Formatting.Indented));
                    Assert.Fail("Failed to delete temporary domain created during the integration test.");
                }
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.User)]
        [TestCategory(TestCategories.Dns)]
        public async Task TestCreatePtrRecords()
        {
            string domainName = CreateRandomDomainName();
            string loadBalancerName = UserLoadBalancerTests.CreateRandomLoadBalancerName();

            IDnsService provider = CreateProvider();
            ILoadBalancerService loadBalancerProvider = UserLoadBalancerTests.CreateProvider();
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TestTimeout(TimeSpan.FromSeconds(60))))
            {
                IEnumerable<LoadBalancingProtocol> protocols = await loadBalancerProvider.ListProtocolsAsync(cancellationTokenSource.Token);
                LoadBalancingProtocol httpProtocol = protocols.First(i => i.Name.Equals("HTTP", StringComparison.OrdinalIgnoreCase));

                LoadBalancerConfiguration loadBalancerConfiguration = new LoadBalancerConfiguration(
                    name: loadBalancerName,
                    nodes: null,
                    protocol: httpProtocol,
                    virtualAddresses: new[] { new LoadBalancerVirtualAddress(LoadBalancerVirtualAddressType.Public) },
                    algorithm: LoadBalancingAlgorithm.RoundRobin);
                LoadBalancer loadBalancer = await loadBalancerProvider.CreateLoadBalancerAsync(loadBalancerConfiguration, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                Assert.IsNotNull(loadBalancer.VirtualAddresses);
                Assert.IsTrue(loadBalancer.VirtualAddresses.Count > 0);

                string originalData = loadBalancer.VirtualAddresses[0].Address.ToString();
                string originalName = string.Format("www.{0}", domainName);
                string updatedName = string.Format("www2.{0}", domainName);
                string originalCommentValue = "Integration test record";
                string updatedCommentValue = "Integration test record 2";
                TimeSpan? originalTimeToLive;
                TimeSpan? updatedTimeToLive = TimeSpan.FromMinutes(12);

                DnsDomainRecordConfiguration[] recordConfigurations =
                    {
                        new DnsDomainRecordConfiguration(
                            type: DnsRecordType.Ptr,
                            name: string.Format("www.{0}", domainName),
                            data: originalData,
                            timeToLive: null,
                            comment: originalCommentValue,
                            priority: null)
                    };
                string serviceName = "cloudLoadBalancers";
                Uri deviceResourceUri = await ((UserLoadBalancerTests.TestCloudLoadBalancerProvider)loadBalancerProvider).GetDeviceResourceUri(loadBalancer, cancellationTokenSource.Token);
                DnsJob<DnsRecordsList> recordsResponse = await provider.AddPtrRecordsAsync(serviceName, deviceResourceUri, recordConfigurations, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                Assert.IsNotNull(recordsResponse.Response);
                Assert.IsNotNull(recordsResponse.Response.Records);
                DnsRecord[] records = recordsResponse.Response.Records.ToArray();
                Assert.AreEqual(recordConfigurations.Length, records.Length);
                originalTimeToLive = records[0].TimeToLive;
                Assert.AreNotEqual(originalTimeToLive, updatedTimeToLive);

                Assert.AreEqual(originalData, records[0].Data);
                Assert.AreEqual(originalTimeToLive, records[0].TimeToLive);
                Assert.AreEqual(originalCommentValue, records[0].Comment);

                foreach (var record in records)
                {
                    Console.WriteLine();
                    Console.WriteLine("Record: {0} ({1})", record.Name, record.Id);
                    Console.WriteLine();
                    Console.WriteLine(await JsonConvert.SerializeObjectAsync(record, Formatting.Indented));
                }

                // update the comment and verify nothing else changed
                DnsDomainRecordUpdateConfiguration recordUpdateConfiguration = new DnsDomainRecordUpdateConfiguration(records[0], originalName, originalData, comment: updatedCommentValue);
                await provider.UpdatePtrRecordsAsync(serviceName, deviceResourceUri, new[] { recordUpdateConfiguration }, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                DnsRecord updatedRecord = await provider.ListPtrRecordDetailsAsync(serviceName, deviceResourceUri, records[0].Id, cancellationTokenSource.Token);
                Assert.IsNotNull(updatedRecord);
                Assert.AreEqual(originalData, updatedRecord.Data);
                Assert.AreEqual(originalName, updatedRecord.Name);
                Assert.AreEqual(originalTimeToLive, updatedRecord.TimeToLive);
                Assert.AreEqual(updatedCommentValue, updatedRecord.Comment);

                // update the TTL and verify nothing else changed
                recordUpdateConfiguration = new DnsDomainRecordUpdateConfiguration(records[0], originalName, originalData, timeToLive: updatedTimeToLive);
                await provider.UpdatePtrRecordsAsync(serviceName, deviceResourceUri, new[] { recordUpdateConfiguration }, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                updatedRecord = await provider.ListPtrRecordDetailsAsync(serviceName, deviceResourceUri, records[0].Id, cancellationTokenSource.Token);
                Assert.IsNotNull(updatedRecord);
                Assert.AreEqual(originalData, updatedRecord.Data);
                Assert.AreEqual(originalName, updatedRecord.Name);
                Assert.AreEqual(updatedTimeToLive, updatedRecord.TimeToLive);
                Assert.AreEqual(updatedCommentValue, updatedRecord.Comment);

                // update the name and verify nothing else changed
                recordUpdateConfiguration = new DnsDomainRecordUpdateConfiguration(records[0], updatedName, originalData);
                await provider.UpdatePtrRecordsAsync(serviceName, deviceResourceUri, new[] { recordUpdateConfiguration }, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
                updatedRecord = await provider.ListPtrRecordDetailsAsync(serviceName, deviceResourceUri, records[0].Id, cancellationTokenSource.Token);
                Assert.IsNotNull(updatedRecord);
                Assert.AreEqual(originalData, updatedRecord.Data);
                Assert.AreEqual(updatedName, updatedRecord.Name);
                Assert.AreEqual(updatedTimeToLive, updatedRecord.TimeToLive);
                Assert.AreEqual(updatedCommentValue, updatedRecord.Comment);

                // remove the PTR record
                // TODO: verify result?
                await provider.RemovePtrRecordsAsync(serviceName, deviceResourceUri, loadBalancer.VirtualAddresses[0].Address, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);

                /* Cleanup
                 */
                await loadBalancerProvider.RemoveLoadBalancerAsync(loadBalancer.Id, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null);
            }
        }

        /// <summary>
        /// Gets all existing domains through a series of asynchronous operations,
        /// each of which requests a subset of the available domains.
        /// </summary>
        /// <param name="provider">The DNS service.</param>
        /// <param name="domainName">If specified, the list will be filtered to only include the specified domain and its subdomains (if any exist).</param>
        /// <param name="limit">The maximum number of domains to return in a single page.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <param name="progress">An optional callback object to receive progress notifications. If this is <see langword="null"/>, no progress notifications are sent.</param>
        /// <returns>
        /// A <see cref="Task"/> object representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property will return a collection of <see cref="DnsDomain"/> objects
        /// representing the requested domains.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="provider"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="limit"/> is less than or equal to 0.</exception>
        private static async Task<ReadOnlyCollection<DnsDomain>> ListAllDomainsAsync(IDnsService provider, string domainName, int? limit, CancellationToken cancellationToken, net.openstack.Core.IProgress<ReadOnlyCollectionPage<DnsDomain>> progress = null)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (limit <= 0)
                throw new ArgumentOutOfRangeException("limit");

            return await (await provider.ListDomainsAsync(domainName, null, limit, cancellationToken)).Item1.GetAllPagesAsync(cancellationToken, progress);
        }

        /// <summary>
        /// Gets all subdomains associated with a domain through a series of asynchronous operations,
        /// each of which requests a subset of the available subdomains.
        /// </summary>
        /// <param name="provider">The DNS service.</param>
        /// <param name="domainId">The top-level domain ID. This is obtained from <see cref="DnsDomain.Id">DnsDomain.Id</see>.</param>
        /// <param name="limit">The maximum number of domains to return in a single page.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <param name="progress">An optional callback object to receive progress notifications. If this is <see langword="null"/>, no progress notifications are sent.</param>
        /// <returns>
        /// A <see cref="Task"/> object representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property will return a collection of <see cref="DnsDomain"/> objects
        /// representing the requested domains.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="provider"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="limit"/> is less than or equal to 0.</exception>
        private static async Task<ReadOnlyCollection<DnsSubdomain>> ListAllSubdomainsAsync(IDnsService provider, DomainId domainId, int? limit, CancellationToken cancellationToken, net.openstack.Core.IProgress<ReadOnlyCollectionPage<DnsSubdomain>> progress = null)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (limit <= 0)
                throw new ArgumentOutOfRangeException("limit");

            return await (await provider.ListSubdomainsAsync(domainId, null, limit, cancellationToken)).Item1.GetAllPagesAsync(cancellationToken, progress);
        }

        private TimeSpan TestTimeout(TimeSpan timeout)
        {
            if (Debugger.IsAttached)
                return TimeSpan.FromDays(1);

            return timeout;
        }

        internal static string CreateRandomDomainName()
        {
            return TestDomainPrefix + Path.GetRandomFileName().Replace('.', '-') + ".com";
        }

        internal static IDnsService CreateProvider()
        {
            CloudDnsProvider provider = new TestCloudDnsProvider(Bootstrapper.Settings.TestIdentity, Bootstrapper.Settings.DefaultRegion, false, null);
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

            return provider;
        }

        private class TestCloudDnsProvider : CloudDnsProvider
        {
            public TestCloudDnsProvider(CloudIdentity defaultIdentity, string defaultRegion, bool internalUrl, IIdentityProvider identityProvider)
                : base(defaultIdentity, defaultRegion, internalUrl, identityProvider)
            {
            }

            protected override byte[] EncodeRequestBodyImpl<TBody>(HttpWebRequest request, TBody body)
            {
                return TestHelpers.EncodeRequestBody(request, body, base.EncodeRequestBodyImpl);
            }

            protected override Tuple<HttpWebResponse, string> ReadResultImpl(Task<WebResponse> task, CancellationToken cancellationToken)
            {
                return TestHelpers.ReadResult(task, cancellationToken, base.ReadResultImpl);
            }
        }
    }
}
