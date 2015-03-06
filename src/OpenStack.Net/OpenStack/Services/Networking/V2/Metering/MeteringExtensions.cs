namespace OpenStack.Services.Networking.V2.Metering
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenStack.Collections;
    using Rackspace.Threading;

    public static class MeteringExtensions
    {
        public static Task<bool> SupportsMeteringAsync(this INetworkingService service, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IMeteringExtension extension;
            try
            {
                extension = service.GetServiceExtension(PredefinedNetworkingExtensions.Metering);
            }
            catch (NotSupportedException)
            {
                return CompletedTask.FromResult(false);
            }

            return TaskBlocks.Using(
                () => extension.PrepareMeteringSupportedAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<MeteringLabel> AddMeteringLabelAsync(this INetworkingService service, MeteringLabelData meteringLabelData, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IMeteringExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.Metering);
            return TaskBlocks.Using(
                () => extension.PrepareAddMeteringLabelAsync(new MeteringLabelRequest(meteringLabelData), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.MeteringLabel));
        }

        public static Task<ReadOnlyCollectionPage<MeteringLabel>> ListMeteringLabelsAsync(this INetworkingService service, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IMeteringExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.Metering);
            return TaskBlocks.Using(
                () => extension.PrepareListMeteringLabelsAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<MeteringLabel> GetMeteringLabelAsync(this INetworkingService service, MeteringLabelId meteringLabelId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IMeteringExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.Metering);
            return TaskBlocks.Using(
                () => extension.PrepareGetMeteringLabelAsync(meteringLabelId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.MeteringLabel));
        }

        public static Task RemoveMeteringLabelAsync(this INetworkingService service, MeteringLabelId meteringLabelId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IMeteringExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.Metering);
            return TaskBlocks.Using(
                () => extension.PrepareRemoveMeteringLabelAsync(meteringLabelId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        public static Task<MeteringLabelRule> AddMeteringLabelRuleAsync(this INetworkingService service, MeteringLabelRuleData meteringLabelRuleData, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IMeteringExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.Metering);
            return TaskBlocks.Using(
                () => extension.PrepareAddMeteringLabelRuleAsync(new MeteringLabelRuleRequest(meteringLabelRuleData), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.MeteringLabelRule));
        }

        public static Task<ReadOnlyCollectionPage<MeteringLabelRule>> ListMeteringLabelRulesAsync(this INetworkingService service, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IMeteringExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.Metering);
            return TaskBlocks.Using(
                () => extension.PrepareListMeteringLabelRulesAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<MeteringLabelRule> GetMeteringLabelRuleAsync(this INetworkingService service, MeteringLabelRuleId meteringLabelRuleId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IMeteringExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.Metering);
            return TaskBlocks.Using(
                () => extension.PrepareGetMeteringLabelRuleAsync(meteringLabelRuleId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.MeteringLabelRule));
        }

        public static Task RemoveMeteringLabelRuleAsync(this INetworkingService service, MeteringLabelRuleId meteringLabelRuleId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IMeteringExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.Metering);
            return TaskBlocks.Using(
                () => extension.PrepareRemoveMeteringLabelRuleAsync(meteringLabelRuleId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }
    }
}
