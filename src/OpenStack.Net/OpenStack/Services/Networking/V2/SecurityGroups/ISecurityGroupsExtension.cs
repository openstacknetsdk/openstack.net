namespace OpenStack.Services.Networking.V2.SecurityGroups
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface ISecurityGroupsExtension
    {
        Task<SecurityGroupsSupportedApiCall> PrepareSecurityGroupsSupportedAsync(CancellationToken cancellationToken);

        Task<AddSecurityGroupApiCall> PrepareAddSecurityGroupAsync(SecurityGroupRequest request, CancellationToken cancellationToken);

        Task<ListSecurityGroupsApiCall> PrepareListSecurityGroupsAsync(CancellationToken cancellationToken);

        Task<GetSecurityGroupApiCall> PrepareGetSecurityGroupAsync(SecurityGroupId securityGroupId, CancellationToken cancellationToken);

        //Task<UpdateSecurityGroupApiCall> PrepareUpdateSecurityGroupAsync(SecurityGroupId securityGroupId, SecurityGroupRequest request, CancellationToken cancellationToken);

        Task<RemoveSecurityGroupApiCall> PrepareRemoveSecurityGroupAsync(SecurityGroupId securityGroupId, CancellationToken cancellationToken);

        Task<AddSecurityGroupRuleApiCall> PrepareAddSecurityGroupRuleAsync(SecurityGroupRuleRequest request, CancellationToken cancellationToken);

        Task<ListSecurityGroupRulesApiCall> PrepareListSecurityGroupRulesAsync(CancellationToken cancellationToken);

        Task<GetSecurityGroupRuleApiCall> PrepareGetSecurityGroupRuleAsync(SecurityGroupRuleId securityGroupRuleId, CancellationToken cancellationToken);

        //Task<UpdateSecurityGroupRuleApiCall> PrepareUpdateSecurityGroupRuleAsync(SecurityGroupRuleId securityGroupRuleId, SecurityGroupRuleRequest request, CancellationToken cancellationToken);

        Task<RemoveSecurityGroupRuleApiCall> PrepareRemoveSecurityGroupRuleAsync(SecurityGroupRuleId securityGroupRuleId, CancellationToken cancellationToken);
    }
}
