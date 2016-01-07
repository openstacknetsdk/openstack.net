using System;
using System.Collections.Generic;
using System.Extensions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenStack.Serialization;

namespace OpenStack.Compute.v2_1
{
    /// <summary />
    [JsonConverterWithConstructor(typeof(RootWrapperConverter), "security_group")]
    public class SecurityGroup : SecurityGroupReference
    {
        /// <summary />
        public SecurityGroup()
        {
            Rules = new List<SecurityGroupRule>();
        }

        /// <summary />
        [JsonProperty("id")]
        public Identifier Id { get; set; }

        /// <summary />
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary />
        [JsonProperty("rules")]
        public IList<SecurityGroupRule> Rules { get; set; }

        /// <inheritdoc cref="ComputeApiBuilder.UpdateSecurityGroupAsync{T}" />
        /// <exception cref="InvalidOperationException">When the <see cref="Server"/> instance was not constructed by the <see cref="ComputeService"/>, as it is missing the appropriate internal state to execute service calls.</exception>
        public async Task UpdateAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var compute = this.GetOwnerOrThrow<ComputeApiBuilder>();
            var request = new SecurityGroupDefinition(Name, Description);

            var result = await compute.UpdateSecurityGroupAsync<SecurityGroup>(Id, request, cancellationToken);
            result.CopyProperties(this);
        }

        /// <inheritdoc cref="ComputeApiBuilder.CreateSecurityGroupRuleAsync{T}" />
        /// <exception cref="InvalidOperationException">When the <see cref="Server"/> instance was not constructed by the <see cref="ComputeService"/>, as it is missing the appropriate internal state to execute service calls.</exception>
        public async Task<SecurityGroupRule> AddRuleAsync(SecurityGroupRuleDefinition rule, CancellationToken cancellationToken = default(CancellationToken))
        {
            var compute = this.GetOwnerOrThrow<ComputeApiBuilder>();
            rule.GroupId = Id;

            var result = await compute.CreateSecurityGroupRuleAsync<SecurityGroupRule>(rule, cancellationToken);
            Rules.Add(result);
            return result;
        }
    }
}