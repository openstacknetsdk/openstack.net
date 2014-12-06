namespace OpenStack.Services.Networking.V2.SecurityGroups
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenStack.Collections;
    using Rackspace.Threading;

    public static class SecurityGroupsExtensions
    {
        public static Task<bool> SupportsSecurityGroupsAsync(this INetworkingService service, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ISecurityGroupsExtension extension;
            try
            {
                extension = service.GetServiceExtension(PredefinedNetworkingExtensions.SecurityGroups);
            }
            catch (NotSupportedException)
            {
                return CompletedTask.FromResult(false);
            }

            return TaskBlocks.Using(
                () => extension.PrepareSecurityGroupsSupportedAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<SecurityGroup> AddSecurityGroupAsync(this INetworkingService service, SecurityGroupData securityGroupData, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ISecurityGroupsExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.SecurityGroups);
            return TaskBlocks.Using(
                () => extension.PrepareAddSecurityGroupAsync(new SecurityGroupRequest(securityGroupData), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.SecurityGroup));
        }

        public static Task<ReadOnlyCollectionPage<SecurityGroup>> ListSecurityGroupsAsync(this INetworkingService service, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ISecurityGroupsExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.SecurityGroups);
            return TaskBlocks.Using(
                () => extension.PrepareListSecurityGroupsAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<SecurityGroup> GetSecurityGroupAsync(this INetworkingService service, SecurityGroupId securityGroupId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ISecurityGroupsExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.SecurityGroups);
            return TaskBlocks.Using(
                () => extension.PrepareGetSecurityGroupAsync(securityGroupId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.SecurityGroup));
        }

        //public static Task<SecurityGroup> UpdateSecurityGroupAsync(this INetworkingService service, SecurityGroupId securityGroupId, SecurityGroupData securityGroupData, CancellationToken cancellationToken)
        //{
        //    if (service == null)
        //        throw new ArgumentNullException("service");

        //    ISecurityGroupsExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.SecurityGroups);
        //    return TaskBlocks.Using(
        //        () => extension.PrepareUpdateSecurityGroupAsync(securityGroupId, new SecurityGroupRequest(securityGroupData), cancellationToken),
        //        task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.SecurityGroup));
        //}

        public static Task RemoveSecurityGroupAsync(this INetworkingService service, SecurityGroupId securityGroupId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ISecurityGroupsExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.SecurityGroups);
            return TaskBlocks.Using(
                () => extension.PrepareRemoveSecurityGroupAsync(securityGroupId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        public static Task<SecurityGroupRule> AddSecurityGroupRuleAsync(this INetworkingService service, SecurityGroupRuleData securityGroupRuleData, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ISecurityGroupsExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.SecurityGroups);
            return TaskBlocks.Using(
                () => extension.PrepareAddSecurityGroupRuleAsync(new SecurityGroupRuleRequest(securityGroupRuleData), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.SecurityGroupRule));
        }


        public static Task<ReadOnlyCollectionPage<SecurityGroupRule>> ListSecurityGroupRulesAsync(this INetworkingService service, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ISecurityGroupsExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.SecurityGroups);
            return TaskBlocks.Using(
                () => extension.PrepareListSecurityGroupRulesAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<SecurityGroupRule> GetSecurityGroupRuleAsync(this INetworkingService service, SecurityGroupRuleId securityGroupRuleId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ISecurityGroupsExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.SecurityGroups);
            return TaskBlocks.Using(
                () => extension.PrepareGetSecurityGroupRuleAsync(securityGroupRuleId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.SecurityGroupRule));
        }

        //public static Task<SecurityGroupRule> UpdateSecurityGroupRuleAsync(this INetworkingService service, SecurityGroupRuleId securityGroupRuleId, SecurityGroupRuleData securityGroupRuleData, CancellationToken cancellationToken)
        //{
        //    if (service == null)
        //        throw new ArgumentNullException("service");

        //    ISecurityGroupsExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.SecurityGroups);
        //    return TaskBlocks.Using(
        //        () => extension.PrepareUpdateSecurityGroupRuleAsync(securityGroupRuleId, new SecurityGroupRuleRequest(securityGroupRuleData), cancellationToken),
        //        task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.SecurityGroupRule));
        //}

        public static Task RemoveSecurityGroupRuleAsync(this INetworkingService service, SecurityGroupRuleId securityGroupRuleId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            ISecurityGroupsExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.SecurityGroups);
            return TaskBlocks.Using(
                () => extension.PrepareRemoveSecurityGroupRuleAsync(securityGroupRuleId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }
    }
}
