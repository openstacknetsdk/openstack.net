using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Moq;
using net.openstack.Core.Domain;
using net.openstack.Providers.Hp;
using Newtonsoft.Json;
using OpenStack.Authentication;
using Xunit;

namespace OpenStack.HP.Authentication
{
    public class ServiceCatalogTests
    {
        [Theory]
        [MemberData("ListServiceTypes")]
        public async void WhenUsingHPIdentityProvider_EveryServiceTypeCanResolveAnEndpoint(ServiceType serviceType)
        {
            UserAccess userAccess = JsonConvert.DeserializeObject<UserAccess>(UserAccessJson);
            var hpIdentityProvider = new Mock<HpIdentityProvider>(new CloudIdentity()){CallBase = true};
            hpIdentityProvider.Setup(x => x.GetUserAccess(It.IsAny<CloudIdentity>(), It.IsAny<bool>())).Returns(userAccess);
            IAuthenticationProvider authenticationProvider = hpIdentityProvider.Object;

            string result = await authenticationProvider.GetEndpoint(serviceType, "region-a.geo-1", false, CancellationToken.None);
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
            ""publicURL"": ""https://region-a.geo-1.identity.hpcloudsvc.com:35357/v2.0/"",
            ""region"": ""region-a.geo-1"",
            ""versionId"": ""2.0"",
            ""versionInfo"": ""https://region-a.geo-1.identity.hpcloudsvc.com:35357/v2.0/"",
            ""versionList"": ""https://region-a.geo-1.identity.hpcloudsvc.com:35357""
          },
          {
            ""publicURL"": ""https://region-a.geo-1.identity.hpcloudsvc.com:35357/v3/"",
            ""region"": ""region-a.geo-1"",
            ""versionId"": ""3.0"",
            ""versionInfo"": ""https://region-a.geo-1.identity.hpcloudsvc.com:35357/v3/"",
            ""versionList"": ""https://region-a.geo-1.identity.hpcloudsvc.com:35357""
          },
          {
            ""publicURL"": ""https://region-b.geo-1.identity.hpcloudsvc.com:35357/v2.0/"",
            ""region"": ""region-b.geo-1"",
            ""versionId"": ""2.0"",
            ""versionInfo"": ""https://region-b.geo-1.identity.hpcloudsvc.com:35357/v2.0/"",
            ""versionList"": ""https://region-b.geo-1.identity.hpcloudsvc.com:35357""
          },
          {
            ""publicURL"": ""https://region-b.geo-1.identity.hpcloudsvc.com:35357/v3/"",
            ""region"": ""region-b.geo-1"",
            ""versionId"": ""3.0"",
            ""versionInfo"": ""https://region-b.geo-1.identity.hpcloudsvc.com:35357/v3/"",
            ""versionList"": ""https://region-b.geo-1.identity.hpcloudsvc.com:35357""
          }
        ],
        ""name"": ""Identity"",
        ""type"": ""identity""
      },
      {
        ""endpoints"": [
          {
            ""publicURL"": ""https://az-1.region-a.geo-1.compute.hpcloudsvc.com/v1.1/TENANT_ID"",
            ""publicURL2"": ""https://az-1.region-a.geo-1.ec2-compute.hpcloudsvc.com/services/Cloud"",
            ""region"": ""az-1.region-a.geo-1"",
            ""tenantId"": ""TENANT_ID"",
            ""versionId"": ""1.1"",
            ""versionInfo"": ""https://az-1.region-a.geo-1.compute.hpcloudsvc.com/v1.1/"",
            ""versionList"": ""https://az-1.region-a.geo-1.compute.hpcloudsvc.com""
          },
          {
            ""publicURL"": ""https://az-2.region-a.geo-1.compute.hpcloudsvc.com/v1.1/TENANT_ID"",
            ""publicURL2"": ""https://az-2.region-a.geo-1.ec2-compute.hpcloudsvc.com/services/Cloud"",
            ""region"": ""az-2.region-a.geo-1"",
            ""tenantId"": ""TENANT_ID"",
            ""versionId"": ""1.1"",
            ""versionInfo"": ""https://az-2.region-a.geo-1.compute.hpcloudsvc.com/v1.1/"",
            ""versionList"": ""https://az-2.region-a.geo-1.compute.hpcloudsvc.com""
          }
        ],
        ""name"": ""Compute"",
        ""type"": ""compute""
      },
      {
        ""endpoints"": [
          {
            ""publicURL"": """",
            ""publicURL2"": """",
            ""region"": ""az-1.region-a.geo-1"",
            ""tenantId"": ""TENANT_ID"",
            ""versionId"": """",
            ""versionInfo"": """",
            ""versionList"": """"
          },
          {
            ""publicURL"": """",
            ""publicURL2"": """",
            ""region"": ""az-2.region-a.geo-1"",
            ""tenantId"": ""TENANT_ID"",
            ""versionId"": """",
            ""versionInfo"": """",
            ""versionList"": """"
          }
        ],
        ""name"": ""Networking"",
        ""type"": ""network""
      },
      {
        ""endpoints"": [
          {
            ""publicURL"": """",
            ""region"": ""region-a.geo-1"",
            ""versionId"": ""2"",
            ""versionInfo"": ""https://region-a.geo-1.usage-reporting-internal.hpcloudsvc.com:8777"",
            ""versionList"": ""https://region-a.geo-1.usage-reporting-internal.hpcloudsvc.com:8777""
          },
          {
            ""publicURL"": """",
            ""region"": ""region-b.geo-1"",
            ""versionId"": ""2"",
            ""versionInfo"": ""https://region-b.geo-1.usage-reporting-internal.hpcloudsvc.com:8777"",
            ""versionList"": ""https://region-b.geo-1.usage-reporting-internal.hpcloudsvc.com:8777""
          }
        ],
        ""name"": ""Usage Reporting"",
        ""type"": ""metering""
      },
      {
        ""endpoints"": [
          {
            ""publicURL"": ""https://region-a.geo-1.objects.hpcloudsvc.com/v1/TENANT_ID"",
            ""region"": ""region-a.geo-1"",
            ""tenantId"": ""TENANT_ID"",
            ""versionId"": ""1.0"",
            ""versionInfo"": ""https://region-a.geo-1.objects.hpcloudsvc.com/v1.0/"",
            ""versionList"": ""https://region-a.geo-1.objects.hpcloudsvc.com""
          }
        ],
        ""name"": ""Object Storage"",
        ""type"": ""object-store""
      },
      {
        ""endpoints"": [
          {
            ""publicURL"": ""https://az-1.region-a.geo-1.compute.hpcloudsvc.com/v1.1/TENANT_ID"",
            ""publicURL2"": """",
            ""region"": ""az-1.region-a.geo-1"",
            ""tenantId"": ""TENANT_ID"",
            ""versionId"": ""1.1"",
            ""versionInfo"": ""https://az-1.region-a.geo-1.compute.hpcloudsvc.com/v1.1/"",
            ""versionList"": ""https://az-1.region-a.geo-1.compute.hpcloudsvc.com""
          },
          {
            ""publicURL"": ""https://az-2.region-a.geo-1.compute.hpcloudsvc.com/v1.1/TENANT_ID"",
            ""publicURL2"": """",
            ""region"": ""az-2.region-a.geo-1"",
            ""tenantId"": ""TENANT_ID"",
            ""versionId"": ""1.1"",
            ""versionInfo"": ""https://az-2.region-a.geo-1.compute.hpcloudsvc.com/v1.1/"",
            ""versionList"": ""https://az-2.region-a.geo-1.compute.hpcloudsvc.com""
          }
        ],
        ""name"": ""Block Storage"",
        ""type"": ""volume""
      },
      {
        ""endpoints"": [
          {
            ""publicURL"": ""https://region-a.geo-1.cdnmgmt.hpcloudsvc.com/v1.0/TENANT_ID"",
            ""region"": ""region-a.geo-1"",
            ""tenantId"": ""TENANT_ID"",
            ""versionId"": ""1.0"",
            ""versionInfo"": ""https://region-a.geo-1.cdnmgmt.hpcloudsvc.com/v1.0/"",
            ""versionList"": ""https://region-a.geo-1.cdnmgmt.hpcloudsvc.com/""
          }
        ],
        ""name"": ""CDN"",
        ""type"": ""hpext:cdn""
      },
      {
        ""endpoints"": [
          {
            ""publicURL"": ""https://glance1.uswest.hpcloud.net:9292/v1.0"",
            ""publicURL2"": """",
            ""region"": ""az-1.region-a.geo-1"",
            ""tenantId"": ""TENANT_ID"",
            ""versionId"": ""1.0"",
            ""versionInfo"": ""https://glance1.uswest.hpcloud.net:9292/v1.0/"",
            ""versionList"": ""https://glance1.uswest.hpcloud.net:9292""
          },
          {
            ""publicURL"": ""https://glance2.uswest.hpcloud.net:9292/v1.0"",
            ""publicURL2"": """",
            ""region"": ""az-2.region-a.geo-1"",
            ""tenantId"": ""TENANT_ID"",
            ""versionId"": ""1.0"",
            ""versionInfo"": ""https://glance2.uswest.hpcloud.net:9292/v1.0/"",
            ""versionList"": ""https://glance2.uswest.hpcloud.net:9292""
          }
        ],
        ""name"": ""Image Management"",
        ""type"": ""image""
      }
    ],
    ""token"": {
      ""expires"": ""2014-12-15T03:15:25.438Z"",
      ""id"": ""TOKEN_ID"",
      ""tenant"": {
        ""id"": ""TENANT_ID"",
        ""name"": ""TOKEN_NAME""
      }
    },
    ""user"": {
      ""id"": ""USER_ID"",
      ""name"": ""USER_NAME"",
      ""otherAttributes"": {
        ""domainStatus"": ""enabled"",
        ""domainStatusCode"": ""00""
      },
      ""roles"": [
        {
          ""id"": ""00000000004003"",
          ""name"": ""domainadmin"",
          ""serviceId"": ""100""
        },
        {
          ""id"": ""00000000004014"",
          ""name"": ""cdn-admin"",
          ""serviceId"": ""150"",
          ""tenantId"": ""TENANT_ID""
        },
        {
          ""id"": ""00000000004025"",
          ""name"": ""sysadmin"",
          ""serviceId"": ""120"",
          ""tenantId"": ""TENANT_ID""
        },
        {
          ""id"": ""00000000004022"",
          ""name"": ""Admin"",
          ""serviceId"": ""110"",
          ""tenantId"": ""TENANT_ID""
        },
        {
          ""id"": ""00000000004004"",
          ""name"": ""domainuser"",
          ""serviceId"": ""100""
        },
        {
          ""id"": ""00000000004016"",
          ""name"": ""netadmin"",
          ""serviceId"": ""120"",
          ""tenantId"": ""TENANT_ID""
        },
        {
          ""id"": ""00000000004024"",
          ""name"": ""user"",
          ""serviceId"": ""140"",
          ""tenantId"": ""TENANT_ID""
        },
        {
          ""id"": ""00000000004013"",
          ""name"": ""block-admin"",
          ""serviceId"": ""130"",
          ""tenantId"": ""TENANT_ID""
        }
      ]
    }
}";
    }
}
