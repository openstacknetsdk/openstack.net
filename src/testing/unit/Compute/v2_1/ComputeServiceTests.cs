using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using OpenStack.Synchronous;
using OpenStack.Testing;
using Xunit;

namespace OpenStack.Compute.v2_1
{
    public class ComputeServiceTests
    {
        private readonly ComputeService _compute;

        public ComputeServiceTests()
        {
            _compute = new ComputeService(Stubs.AuthenticationProvider, "region");
        }

        [Fact]
        public void GetLimits()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(JObject.Parse(@"
{
    'limits': {
        'rate': [
            {
                'limit': [
                    {
                    'next-available': '2012-09-10T20:11:45.146Z',
                    'remaining': 0,
                    'unit': 'DAY',
                    'value': 0,
                    'verb': 'POST'
                },
                {
                    'next-available': '2012-09-10T20:11:45.146Z',
                    'remaining': 0,
                    'unit': 'MINUTE',
                    'value': 0,
                    'verb': 'GET'
                }
            ],
            'regex': '/v[^/]/(\\d+)/(rax-networks)/?.*',
            'uri': '/rax-networks'
            }
        ],
        'absolute': {
            'maxServerMeta': 128,
            'maxPersonality': 5,
            'totalServerGroupsUsed': 0,
            'maxImageMeta': 128,
            'maxPersonalitySize': 10240,
            'maxTotalKeypairs': 100,
            'maxSecurityGroupRules': 20,
            'maxServerGroups': 10,
            'totalCoresUsed': 1,
            'totalRAMUsed': 2048,
            'totalInstancesUsed': 1,
            'maxSecurityGroups': 10,
            'totalFloatingIpsUsed': 0,
            'maxTotalCores': 20,
            'maxServerGroupMembers': 10,
            'maxTotalFloatingIps': 10,
            'totalSecurityGroupsUsed': 1,
            'maxTotalInstances': 10,
            'maxTotalRAMSize': 51200
        }
    }
}").ToString());

                var limits = _compute.GetLimits();

                Assert.NotNull(limits);

                Assert.NotNull(limits.RateLimits);
                Assert.Equal(1, limits.RateLimits.Count);
                var networkLimits = limits.RateLimits.FirstOrDefault(l => l.Name.Contains("rax-networks"));
                Assert.NotNull(networkLimits);
                var networkGetLimit = networkLimits.Limits.FirstOrDefault(l => l.HttpMethod == "GET");
                Assert.NotNull(networkGetLimit);
                
                Assert.NotNull(limits.ResourceLimits);
                Assert.NotNull(limits.ResourceLimits.CoresMax);
            }
        }

        [Fact]
        public void GetCurrentQuotas()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(JObject.Parse(@"
{
  'quota_set': {
    'injected_file_content_bytes': 10240,
    'metadata_items': 128,
    'server_group_members': 10,
    'server_groups': 10,
    'ram': 51200,
    'floating_ips': 10,
    'key_pairs': 100,
    'id': 'details',
    'instances': 10,
    'security_group_rules': 20,
    'injected_files': 5,
    'cores': 20,
    'fixed_ips': -1,
    'injected_file_path_bytes': 255,
    'security_groups': 10
  }
}").ToString());

                var quotas = _compute.GetCurrentQuotas();

                httpTest.ShouldHaveCalled("*/os-quota-sets/details");
                Assert.NotNull(quotas);

                Assert.Equal(100, quotas.KeyPairs);
                Assert.Equal(-1, quotas.FixedIPs);
            }
        }

        [Fact]
        public void GetDefaultQuotas()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(JObject.Parse(@"
{
  'quota_set': {
    'injected_file_content_bytes': 10240,
    'metadata_items': 128,
    'server_group_members': 10,
    'server_groups': 10,
    'ram': 51200,
    'floating_ips': 10,
    'key_pairs': 100,
    'id': 'defaults',
    'instances': 10,
    'security_group_rules': 20,
    'injected_files': 5,
    'cores': 20,
    'fixed_ips': -1,
    'injected_file_path_bytes': 255,
    'security_groups': 10
  }
}").ToString());

                var quotas = _compute.GetDefaultQuotas();

                httpTest.ShouldHaveCalled("*/os-quota-sets/defaults");
                Assert.NotNull(quotas);

                Assert.Equal(100, quotas.KeyPairs);
                Assert.Equal(-1, quotas.FixedIPs);
            }
        }
    }
}
