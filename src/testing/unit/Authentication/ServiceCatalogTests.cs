using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Moq;
using net.openstack.Core.Domain;
using net.openstack.Core.Providers;
using Newtonsoft.Json;
using Xunit;

namespace OpenStack.Authentication
{
    public class ServiceCatalogTests
    {
        [Theory]
        [MemberData("ListServiceTypes")]
        public async void WhenUsingGenericOpenstackIdentityProvider_EveryServiceTypeCanResolveAnEndpoint(ServiceType serviceType)
        {
            UserAccess userAccess = JsonConvert.DeserializeObject<UserAccess>(UserAccessJson);
            var hpIdentityProvider = new Mock<OpenStackIdentityProvider>(new Uri("http://example.com"), new CloudIdentity()){CallBase = true};
            hpIdentityProvider.Setup(x => x.GetUserAccess(It.IsAny<CloudIdentity>(), It.IsAny<bool>())).Returns(userAccess);
            IAuthenticationProvider authenticationProvider = hpIdentityProvider.Object;

            string result = await authenticationProvider.GetEndpoint(serviceType, "RegionOne", false, CancellationToken.None);
            Assert.NotNull(result);
        }

        public static IEnumerable<object[]> ListServiceTypes()
        {
            return Enum.GetValues(typeof (ServiceType)).Cast<ServiceType>().Select(x => new object[] {x});
        }

        // Sample from https://wiki.openstack.org/wiki/API_Working_Group/Current_Design/Service_Catalog#DevStack
        private const string UserAccessJson = @"{
    ""metadata"": {
      ""is_admin"": 0,
      ""roles"": [
        ""b4b435b99d7846048a27a32b8ebcea89"",
        ""9fe2ff9ee4384b1894a90878d3e92bab"",
        ""e026698b534844e8829e11d55e3a745c"",
        ""d42cb0c16b23461d971df41c843192b5""
      ]
    },
    ""serviceCatalog"": [
      {
        ""endpoints"": [
          {
            ""adminURL"": ""http://111.222.333.444:8004/v1/TENANT_ID"",
            ""id"": ""249083e367a242e5bde5833805d0ec96"",
            ""internalURL"": ""http://111.222.333.444:8004/v1/TENANT_ID"",
            ""publicURL"": ""http://111.222.333.444:8004/v1/TENANT_ID"",
            ""region"": ""RegionOne""
          }
        ],
        ""endpoints_links"": [],
        ""name"": ""heat"",
        ""type"": ""orchestration""
      },
      {
        ""endpoints"": [
          {
            ""adminURL"": ""http://111.222.333.444:8774/v2/TENANT_ID"",
            ""id"": ""483b2f0455df450ba49a992ab078e0eb"",
            ""internalURL"": ""http://111.222.333.444:8774/v2/TENANT_ID"",
            ""publicURL"": ""http://111.222.333.444:8774/v2/TENANT_ID"",
            ""region"": ""RegionOne""
          }
        ],
        ""endpoints_links"": [],
        ""name"": ""nova"",
        ""type"": ""compute""
      },
      {
        ""endpoints"": [
          {
            ""adminURL"": ""http://111.222.333.444:9696/"",
            ""id"": ""33b6fbc1b4f247038c159cda2c202429"",
            ""internalURL"": ""http://111.222.333.444:9696/"",
            ""publicURL"": ""http://111.222.333.444:9696/"",
            ""region"": ""RegionOne""
          }
        ],
        ""endpoints_links"": [],
        ""name"": ""neutron"",
        ""type"": ""network""
      },
      {
        ""endpoints"": [
          {
            ""adminURL"": ""http://111.222.333.444:8776/v2/TENANT_ID"",
            ""id"": ""08f5f43c145c4552b81e58e81e7fa563"",
            ""internalURL"": ""http://111.222.333.444:8776/v2/TENANT_ID"",
            ""publicURL"": ""http://111.222.333.444:8776/v2/TENANT_ID"",
            ""region"": ""RegionOne""
          }
        ],
        ""endpoints_links"": [],
        ""name"": ""cinderv2"",
        ""type"": ""volumev2""
      },
      {
        ""endpoints"": [
          {
            ""adminURL"": ""http://111.222.333.444:8779/v1.0/TENANT_ID"",
            ""id"": ""3c09e566901b4adf9c76bd115c2b3da5"",
            ""internalURL"": ""http://111.222.333.444:8779/v1.0/TENANT_ID"",
            ""publicURL"": ""http://111.222.333.444:8779/v1.0/TENANT_ID"",
            ""region"": ""RegionOne""
          }
        ],
        ""endpoints_links"": [],
        ""name"": ""trove"",
        ""type"": ""database""
      },
      {
        ""endpoints"": [
          {
            ""adminURL"": ""http://111.222.333.444:8888/v1.0/TENANT_ID"",
            ""id"": ""962b5253970748b6926293989070829e"",
            ""internalURL"": ""http://111.222.333.444:8888/v1.0/TENANT_ID"",
            ""publicURL"": ""http://111.222.333.444:8888/v1.0/TENANT_ID"",
            ""region"": ""RegionOne""
          }
        ],
        ""endpoints_links"": [],
        ""name"": ""poppy"",
        ""type"": ""cdn""
      },
      {
        ""endpoints"": [
          {
            ""adminURL"": ""http://111.222.333.444:3333"",
            ""id"": ""0dc5ba19c56a4136b3a64f35787557fc"",
            ""internalURL"": ""http://111.222.333.444:3333"",
            ""publicURL"": ""http://111.222.333.444:3333"",
            ""region"": ""RegionOne""
          }
        ],
        ""endpoints_links"": [],
        ""name"": ""s3"",
        ""type"": ""s3""
      },
      {
        ""endpoints"": [
          {
            ""adminURL"": ""http://111.222.333.444:9292"",
            ""id"": ""a90b1081ec4e4c89bf785de88ba4c821"",
            ""internalURL"": ""http://111.222.333.444:9292"",
            ""publicURL"": ""http://111.222.333.444:9292"",
            ""region"": ""RegionOne""
          }
        ],
        ""endpoints_links"": [],
        ""name"": ""glance"",
        ""type"": ""image""
      },
      {
        ""endpoints"": [
          {
            ""adminURL"": ""http://111.222.333.444:8000/v1"",
            ""id"": ""1ea607dab4fa4c2c9f5ab7e9cba48cc7"",
            ""internalURL"": ""http://111.222.333.444:8000/v1"",
            ""publicURL"": ""http://111.222.333.444:8000/v1"",
            ""region"": ""RegionOne""
          }
        ],
        ""endpoints_links"": [],
        ""name"": ""heat-cfn"",
        ""type"": ""cloudformation""
      },
      {
        ""endpoints"": [
          {
            ""adminURL"": ""http://111.222.333.444:8776/v1/TENANT_ID"",
            ""id"": ""74e3e4245a1848a5bc8933775165711d"",
            ""internalURL"": ""http://111.222.333.444:8776/v1/TENANT_ID"",
            ""publicURL"": ""http://111.222.333.444:8776/v1/TENANT_ID"",
            ""region"": ""RegionOne""
          }
        ],
        ""endpoints_links"": [],
        ""name"": ""cinder"",
        ""type"": ""volume""
      },
      {
        ""endpoints"": [
          {
            ""adminURL"": ""http://111.222.333.444:8773/services/Admin"",
            ""id"": ""718512c34d264188ba06deb48e86cd2d"",
            ""internalURL"": ""http://111.222.333.444:8773/services/Cloud"",
            ""publicURL"": ""http://111.222.333.444:8773/services/Cloud"",
            ""region"": ""RegionOne""
          }
        ],
        ""endpoints_links"": [],
        ""name"": ""ec2"",
        ""type"": ""ec2""
      },
      {
        ""endpoints"": [
          {
            ""adminURL"": ""http://111.222.333.444:8774/v2.1/TENANT_ID"",
            ""id"": ""5671f9e9789f49188184b5b1d6cd2d0d"",
            ""internalURL"": ""http://111.222.333.444:8774/v2.1/TENANT_ID"",
            ""publicURL"": ""http://111.222.333.444:8774/v2.1/TENANT_ID"",
            ""region"": ""RegionOne""
          }
        ],
        ""endpoints_links"": [],
        ""name"": ""novav21"",
        ""type"": ""computev21""
      },
      {
        ""endpoints"": [
          {
            ""adminURL"": ""http://111.222.333.444:8080"",
            ""id"": ""64117a0362294a2488f4e0b2e82d2391"",
            ""internalURL"": ""http://111.222.333.444:8080/v1/AUTH_TENANT_ID"",
            ""publicURL"": ""http://111.222.333.444:8080/v1/AUTH_TENANT_ID"",
            ""region"": ""RegionOne""
          }
        ],
        ""endpoints_links"": [],
        ""name"": ""swift"",
        ""type"": ""object-store""
      },
      {
        ""endpoints"": [
          {
            ""adminURL"": ""http://111.222.333.444:35357/v2.0"",
            ""id"": ""1fbdedef18064304954ef0cb439949d6"",
            ""internalURL"": ""http://111.222.333.444:5000/v2.0"",
            ""publicURL"": ""http://111.222.333.444:5000/v2.0"",
            ""region"": ""RegionOne""
          }
        ],
        ""endpoints_links"": [],
        ""name"": ""keystone"",
        ""type"": ""identity""
      }
    ],
    ""token"": {
      ""audit_ids"": [
        ""S0qw2tDSSiaaj7327vGXNw""
      ],
      ""expires"": ""2014-12-16T04:06:16Z"",
      ""id"": ""TOKEN_ID"",
      ""issued_at"": ""2014-12-16T03:06:16.054920"",
      ""tenant"": {
        ""description"": null,
        ""enabled"": true,
        ""id"": ""TENANT_ID"",
        ""name"": ""demo""
      }
    },
    ""user"": {
      ""id"": ""USER_ID"",
      ""name"": ""demo"",
      ""roles"": [
        {
          ""name"": ""Member""
        },
        {
          ""name"": ""_member_""
        },
        {
          ""name"": ""anotherrole""
        },
        {
          ""name"": ""heat_stack_owner""
        }
      ],
      ""roles_links"": [],
      ""username"": ""demo""
    }
  }";
    }
}
