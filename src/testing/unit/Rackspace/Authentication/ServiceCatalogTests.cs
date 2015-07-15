using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Moq;
using net.openstack.Core.Domain;
using net.openstack.Providers.Rackspace;
using Newtonsoft.Json;
using OpenStack.Authentication;
using Xunit;

namespace OpenStack.Rackspace.Authentication
{
    public class ServiceCatalogTests
    {
        [Theory]
        [MemberData("ListServiceTypes")]
        public async void WhenUsingRackspaceIdentityProvider_EveryServiceTypeCanResolveAnEndpoint(ServiceType serviceType)
        {
            UserAccess userAccess = JsonConvert.DeserializeObject<UserAccess>(UserAccessJson);
            var hpIdentityProvider = new Mock<CloudIdentityProvider>(new CloudIdentity()){CallBase = true};
            hpIdentityProvider.Setup(x => x.GetUserAccess(It.IsAny<CloudIdentity>(), It.IsAny<bool>())).Returns(userAccess);
            IAuthenticationProvider authenticationProvider = hpIdentityProvider.Object;

            string result = await authenticationProvider.GetEndpoint(serviceType, "DFW", false, CancellationToken.None);
            Assert.NotNull(result);
        }

        public static IEnumerable<object[]> ListServiceTypes()
        {
            return Enum.GetValues(typeof (ServiceType)).Cast<ServiceType>().Select(x => new object[] {x});
        }

        // Sample from https://wiki.openstack.org/wiki/API_Working_Group/Current_Design/Service_Catalog#HP_Public_Cloud
        private const string UserAccessJson = @"{
    ""serviceCatalog"": [
      {
        ""endpoints"": [
          {
            ""publicURL"": ""https://cdn5.clouddrive.com/v1/MossoCloudFS_ID"",
            ""region"": ""IAD"",
            ""tenantId"": ""MossoCloudFS_ID""
          },
          {
            ""publicURL"": ""https://cdn4.clouddrive.com/v1/MossoCloudFS_ID"",
            ""region"": ""SYD"",
            ""tenantId"": ""MossoCloudFS_ID""
          },
          {
            ""publicURL"": ""https://cdn1.clouddrive.com/v1/MossoCloudFS_ID"",
            ""region"": ""DFW"",
            ""tenantId"": ""MossoCloudFS_ID""
          },
          {
            ""publicURL"": ""https://cdn6.clouddrive.com/v1/MossoCloudFS_ID"",
            ""region"": ""HKG"",
            ""tenantId"": ""MossoCloudFS_ID""
          },
          {
            ""publicURL"": ""https://cdn2.clouddrive.com/v1/MossoCloudFS_ID"",
            ""region"": ""ORD"",
            ""tenantId"": ""MossoCloudFS_ID""
          }
        ],
        ""name"": ""cloudFilesCDN"",
        ""type"": ""rax:object-cdn""
      },
      {
        ""endpoints"": [
          {
            ""internalURL"": ""https://snet-storage101.iad3.clouddrive.com/v1/MossoCloudFS_ID"",
            ""publicURL"": ""https://storage101.iad3.clouddrive.com/v1/MossoCloudFS_ID"",
            ""region"": ""IAD"",
            ""tenantId"": ""MossoCloudFS_ID""
          },
          {
            ""internalURL"": ""https://snet-storage101.syd2.clouddrive.com/v1/MossoCloudFS_ID"",
            ""publicURL"": ""https://storage101.syd2.clouddrive.com/v1/MossoCloudFS_ID"",
            ""region"": ""SYD"",
            ""tenantId"": ""MossoCloudFS_ID""
          },
          {
            ""internalURL"": ""https://snet-storage101.dfw1.clouddrive.com/v1/MossoCloudFS_ID"",
            ""publicURL"": ""https://storage101.dfw1.clouddrive.com/v1/MossoCloudFS_ID"",
            ""region"": ""DFW"",
            ""tenantId"": ""MossoCloudFS_ID""
          },
          {
            ""internalURL"": ""https://snet-storage101.hkg1.clouddrive.com/v1/MossoCloudFS_ID"",
            ""publicURL"": ""https://storage101.hkg1.clouddrive.com/v1/MossoCloudFS_ID"",
            ""region"": ""HKG"",
            ""tenantId"": ""MossoCloudFS_ID""
          },
          {
            ""internalURL"": ""https://snet-storage101.ord1.clouddrive.com/v1/MossoCloudFS_ID"",
            ""publicURL"": ""https://storage101.ord1.clouddrive.com/v1/MossoCloudFS_ID"",
            ""region"": ""ORD"",
            ""tenantId"": ""MossoCloudFS_ID""
          }
        ],
        ""name"": ""cloudFiles"",
        ""type"": ""object-store""
      },
      {
        ""endpoints"": [
          {
            ""publicURL"": ""https://syd.blockstorage.api.rackspacecloud.com/v1/TENANT_ID"",
            ""region"": ""SYD"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://dfw.blockstorage.api.rackspacecloud.com/v1/TENANT_ID"",
            ""region"": ""DFW"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://ord.blockstorage.api.rackspacecloud.com/v1/TENANT_ID"",
            ""region"": ""ORD"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://iad.blockstorage.api.rackspacecloud.com/v1/TENANT_ID"",
            ""region"": ""IAD"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://hkg.blockstorage.api.rackspacecloud.com/v1/TENANT_ID"",
            ""region"": ""HKG"",
            ""tenantId"": ""TENANT_ID""
          }
        ],
        ""name"": ""cloudBlockStorage"",
        ""type"": ""volume""
      },
      {
        ""endpoints"": [
          {
            ""publicURL"": ""https://iad.images.api.rackspacecloud.com/v2"",
            ""region"": ""IAD"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://ord.images.api.rackspacecloud.com/v2"",
            ""region"": ""ORD"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://hkg.images.api.rackspacecloud.com/v2"",
            ""region"": ""HKG"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://dfw.images.api.rackspacecloud.com/v2"",
            ""region"": ""DFW"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://syd.images.api.rackspacecloud.com/v2"",
            ""region"": ""SYD"",
            ""tenantId"": ""TENANT_ID""
          }
        ],
        ""name"": ""cloudImages"",
        ""type"": ""image""
      },
      {
        ""endpoints"": [
          {
            ""internalURL"": ""https://snet-hkg.queues.api.rackspacecloud.com/v1/TENANT_ID"",
            ""publicURL"": ""https://hkg.queues.api.rackspacecloud.com/v1/TENANT_ID"",
            ""region"": ""HKG"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""internalURL"": ""https://snet-ord.queues.api.rackspacecloud.com/v1/TENANT_ID"",
            ""publicURL"": ""https://ord.queues.api.rackspacecloud.com/v1/TENANT_ID"",
            ""region"": ""ORD"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""internalURL"": ""https://snet-syd.queues.api.rackspacecloud.com/v1/TENANT_ID"",
            ""publicURL"": ""https://syd.queues.api.rackspacecloud.com/v1/TENANT_ID"",
            ""region"": ""SYD"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""internalURL"": ""https://snet-dfw.queues.api.rackspacecloud.com/v1/TENANT_ID"",
            ""publicURL"": ""https://dfw.queues.api.rackspacecloud.com/v1/TENANT_ID"",
            ""region"": ""DFW"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""internalURL"": ""https://snet-iad.queues.api.rackspacecloud.com/v1/TENANT_ID"",
            ""publicURL"": ""https://iad.queues.api.rackspacecloud.com/v1/TENANT_ID"",
            ""region"": ""IAD"",
            ""tenantId"": ""TENANT_ID""
          }
        ],
        ""name"": ""cloudQueues"",
        ""type"": ""rax:queues""
      },
      {
        ""endpoints"": [
          {
            ""publicURL"": ""https://iad.bigdata.api.rackspacecloud.com/v1.0/TENANT_ID"",
            ""region"": ""IAD"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://dfw.bigdata.api.rackspacecloud.com/v1.0/TENANT_ID"",
            ""region"": ""DFW"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://ord.bigdata.api.rackspacecloud.com/v1.0/TENANT_ID"",
            ""region"": ""ORD"",
            ""tenantId"": ""TENANT_ID""
          }
        ],
        ""name"": ""cloudBigData"",
        ""type"": ""rax:bigdata""
      },
      {
        ""endpoints"": [
          {
            ""publicURL"": ""https://hkg.orchestration.api.rackspacecloud.com/v1/TENANT_ID"",
            ""region"": ""HKG"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://dfw.orchestration.api.rackspacecloud.com/v1/TENANT_ID"",
            ""region"": ""DFW"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://ord.orchestration.api.rackspacecloud.com/v1/TENANT_ID"",
            ""region"": ""ORD"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://iad.orchestration.api.rackspacecloud.com/v1/TENANT_ID"",
            ""region"": ""IAD"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://syd.orchestration.api.rackspacecloud.com/v1/TENANT_ID"",
            ""region"": ""SYD"",
            ""tenantId"": ""TENANT_ID""
          }
        ],
        ""name"": ""cloudOrchestration"",
        ""type"": ""orchestration""
      },
      {
        ""endpoints"": [
          {
            ""publicURL"": ""https://syd.servers.api.rackspacecloud.com/v2/TENANT_ID"",
            ""region"": ""SYD"",
            ""tenantId"": ""TENANT_ID"",
            ""versionId"": ""2"",
            ""versionInfo"": ""https://syd.servers.api.rackspacecloud.com/v2"",
            ""versionList"": ""https://syd.servers.api.rackspacecloud.com/""
          },
          {
            ""publicURL"": ""https://dfw.servers.api.rackspacecloud.com/v2/TENANT_ID"",
            ""region"": ""DFW"",
            ""tenantId"": ""TENANT_ID"",
            ""versionId"": ""2"",
            ""versionInfo"": ""https://dfw.servers.api.rackspacecloud.com/v2"",
            ""versionList"": ""https://dfw.servers.api.rackspacecloud.com/""
          },
          {
            ""publicURL"": ""https://iad.servers.api.rackspacecloud.com/v2/TENANT_ID"",
            ""region"": ""IAD"",
            ""tenantId"": ""TENANT_ID"",
            ""versionId"": ""2"",
            ""versionInfo"": ""https://iad.servers.api.rackspacecloud.com/v2"",
            ""versionList"": ""https://iad.servers.api.rackspacecloud.com/""
          },
          {
            ""publicURL"": ""https://hkg.servers.api.rackspacecloud.com/v2/TENANT_ID"",
            ""region"": ""HKG"",
            ""tenantId"": ""TENANT_ID"",
            ""versionId"": ""2"",
            ""versionInfo"": ""https://hkg.servers.api.rackspacecloud.com/v2"",
            ""versionList"": ""https://hkg.servers.api.rackspacecloud.com/""
          },
          {
            ""publicURL"": ""https://ord.servers.api.rackspacecloud.com/v2/TENANT_ID"",
            ""region"": ""ORD"",
            ""tenantId"": ""TENANT_ID"",
            ""versionId"": ""2"",
            ""versionInfo"": ""https://ord.servers.api.rackspacecloud.com/v2"",
            ""versionList"": ""https://ord.servers.api.rackspacecloud.com/""
          }
        ],
        ""name"": ""cloudServersOpenStack"",
        ""type"": ""compute""
      },
      {
        ""endpoints"": [
          {
            ""publicURL"": ""https://ord.autoscale.api.rackspacecloud.com/v1.0/TENANT_ID"",
            ""region"": ""ORD"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://dfw.autoscale.api.rackspacecloud.com/v1.0/TENANT_ID"",
            ""region"": ""DFW"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://hkg.autoscale.api.rackspacecloud.com/v1.0/TENANT_ID"",
            ""region"": ""HKG"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://iad.autoscale.api.rackspacecloud.com/v1.0/TENANT_ID"",
            ""region"": ""IAD"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://syd.autoscale.api.rackspacecloud.com/v1.0/TENANT_ID"",
            ""region"": ""SYD"",
            ""tenantId"": ""TENANT_ID""
          }
        ],
        ""name"": ""autoscale"",
        ""type"": ""rax:autoscale""
      },
      {
        ""endpoints"": [
          {
            ""publicURL"": ""https://syd.databases.api.rackspacecloud.com/v1.0/TENANT_ID"",
            ""region"": ""SYD"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://dfw.databases.api.rackspacecloud.com/v1.0/TENANT_ID"",
            ""region"": ""DFW"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://ord.databases.api.rackspacecloud.com/v1.0/TENANT_ID"",
            ""region"": ""ORD"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://iad.databases.api.rackspacecloud.com/v1.0/TENANT_ID"",
            ""region"": ""IAD"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://hkg.databases.api.rackspacecloud.com/v1.0/TENANT_ID"",
            ""region"": ""HKG"",
            ""tenantId"": ""TENANT_ID""
          }
        ],
        ""name"": ""cloudDatabases"",
        ""type"": ""rax:database""
      },
      {
        ""endpoints"": [
          {
            ""publicURL"": ""https://iad.backup.api.rackspacecloud.com/v1.0/TENANT_ID"",
            ""region"": ""IAD"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://hkg.backup.api.rackspacecloud.com/v1.0/TENANT_ID"",
            ""region"": ""HKG"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://syd.backup.api.rackspacecloud.com/v1.0/TENANT_ID"",
            ""region"": ""SYD"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://dfw.backup.api.rackspacecloud.com/v1.0/TENANT_ID"",
            ""region"": ""DFW"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://ord.backup.api.rackspacecloud.com/v1.0/TENANT_ID"",
            ""region"": ""ORD"",
            ""tenantId"": ""TENANT_ID""
          }
        ],
        ""name"": ""cloudBackup"",
        ""type"": ""rax:backup""
      },
      {
        ""endpoints"": [
          {
            ""publicURL"": ""https://iad.networks.api.rackspacecloud.com/v2.0"",
            ""region"": ""IAD"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://lon.networks.api.rackspacecloud.com/v2.0"",
            ""region"": ""LON"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://ord.networks.api.rackspacecloud.com/v2.0"",
            ""region"": ""ORD"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://syd.networks.api.rackspacecloud.com/v2.0"",
            ""region"": ""SYD"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://dfw.networks.api.rackspacecloud.com/v2.0"",
            ""region"": ""DFW"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://hkg.networks.api.rackspacecloud.com/v2.0"",
            ""region"": ""HKG"",
            ""tenantId"": ""TENANT_ID""
          }
        ],
        ""name"": ""cloudNetworks"",
        ""type"": ""network""
      },
      {
        ""endpoints"": [
          {
            ""publicURL"": ""https://global.metrics.api.rackspacecloud.com/v2.0/TENANT_ID"",
            ""region"": ""IAD"",
            ""tenantId"": ""TENANT_ID""
          }
        ],
        ""name"": ""cloudMetrics"",
        ""type"": ""rax:cloudmetrics""
      },
      {
        ""endpoints"": [
          {
            ""publicURL"": ""https://syd.loadbalancers.api.rackspacecloud.com/v1.0/TENANT_ID"",
            ""region"": ""SYD"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://iad.loadbalancers.api.rackspacecloud.com/v1.0/TENANT_ID"",
            ""region"": ""IAD"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://ord.loadbalancers.api.rackspacecloud.com/v1.0/TENANT_ID"",
            ""region"": ""ORD"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://hkg.loadbalancers.api.rackspacecloud.com/v1.0/TENANT_ID"",
            ""region"": ""HKG"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""publicURL"": ""https://dfw.loadbalancers.api.rackspacecloud.com/v1.0/TENANT_ID"",
            ""region"": ""DFW"",
            ""tenantId"": ""TENANT_ID""
          }
        ],
        ""name"": ""cloudLoadBalancers"",
        ""type"": ""rax:load-balancer""
      },
      {
        ""endpoints"": [
          {
            ""internalURL"": ""https://atom.prod.hkg1.us.ci.rackspace.net/TENANT_ID"",
            ""publicURL"": ""https://hkg.feeds.api.rackspacecloud.com/TENANT_ID"",
            ""region"": ""HKG"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""internalURL"": ""https://atom.prod.syd2.us.ci.rackspace.net/TENANT_ID"",
            ""publicURL"": ""https://syd.feeds.api.rackspacecloud.com/TENANT_ID"",
            ""region"": ""SYD"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""internalURL"": ""https://atom.prod.iad3.us.ci.rackspace.net/TENANT_ID"",
            ""publicURL"": ""https://iad.feeds.api.rackspacecloud.com/TENANT_ID"",
            ""region"": ""IAD"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""internalURL"": ""https://atom.prod.dfw1.us.ci.rackspace.net/TENANT_ID"",
            ""publicURL"": ""https://dfw.feeds.api.rackspacecloud.com/TENANT_ID"",
            ""region"": ""DFW"",
            ""tenantId"": ""TENANT_ID""
          },
          {
            ""internalURL"": ""https://atom.prod.ord1.us.ci.rackspace.net/TENANT_ID"",
            ""publicURL"": ""https://ord.feeds.api.rackspacecloud.com/TENANT_ID"",
            ""region"": ""ORD"",
            ""tenantId"": ""TENANT_ID""
          }
        ],
        ""name"": ""cloudFeeds"",
        ""type"": ""rax:feeds""
      },
      {
        ""endpoints"": [
          {
            ""publicURL"": ""https://monitoring.api.rackspacecloud.com/v1.0/TENANT_ID"",
            ""tenantId"": ""TENANT_ID""
          }
        ],
        ""name"": ""cloudMonitoring"",
        ""type"": ""rax:monitor""
      },
      {
        ""endpoints"": [
          {
            ""publicURL"": ""https://dns.api.rackspacecloud.com/v1.0/TENANT_ID"",
            ""tenantId"": ""TENANT_ID""
          }
        ],
        ""name"": ""cloudDNS"",
        ""type"": ""rax:dns""
      },
      {
        ""endpoints"": [
          {
            ""publicURL"": ""https://servers.api.rackspacecloud.com/v1.0/TENANT_ID"",
            ""tenantId"": ""TENANT_ID"",
            ""versionId"": ""1.0"",
            ""versionInfo"": ""https://servers.api.rackspacecloud.com/v1.0"",
            ""versionList"": ""https://servers.api.rackspacecloud.com/""
          }
        ],
        ""name"": ""cloudServers"",
        ""type"": ""compute""
      },
      {
        ""name"": ""rackCDN"",
        ""endpoints"": [
          {
            ""region"": ""DFW"",
            ""tenantId"": ""963451"",
            ""publicURL"": ""https://global.cdn.api.rackspacecloud.com/v1.0/TENANT_ID"",
            ""internalURL"": ""https://global.cdn.api.rackspacecloud.com/v1.0/TENANT_ID""
          }
        ],
        ""type"": ""rax:cdn""
      }
    ],
    ""token"": {
      ""RAX-AUTH:authenticatedBy"": [
        ""PASSWORD""
      ],
      ""expires"": ""2014-12-11T03:26:57.420Z"",
      ""id"": ""TOKEN_ID"",
      ""tenant"": {
        ""id"": ""TENANT_ID"",
        ""name"": ""TENANT_ID""
      }
    },
    ""user"": {
      ""RAX-AUTH:defaultRegion"": ""DFW"",
      ""id"": ""USER_ID"",
      ""name"": ""useranme"",
      ""roles"": [
        {
          ""description"": ""Checkmate Access role"",
          ""id"": ""10000150"",
          ""name"": ""checkmate""
        },
        {
          ""description"": ""A Role that allows a user access to keystone Service methods"",
          ""id"": ""5"",
          ""name"": ""object-store:default"",
          ""tenantId"": ""MossoCloudFS_ID""
        },
        {
          ""description"": ""A Role that allows a user access to keystone Service methods"",
          ""id"": ""6"",
          ""name"": ""compute:default"",
          ""tenantId"": ""TENANT_ID""
        },
        {
          ""description"": ""User Admin Role."",
          ""id"": ""3"",
          ""name"": ""identity:user-admin""
        }
      ]
    }
  }";
    }
}
