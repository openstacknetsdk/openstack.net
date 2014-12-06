namespace OpenStack.Services.Networking.V2.Quotas
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenStack.Collections;
    using OpenStack.Services.Identity;
    using Rackspace.Threading;

    public static class QuotasExtensions
    {
        public static Task<bool> SupportsQuotasAsync(this INetworkingService service, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IQuotasExtension extension;
            try
            {
                extension = service.GetServiceExtension(PredefinedNetworkingExtensions.Quotas);
            }
            catch (NotSupportedException)
            {
                return CompletedTask.FromResult(false);
            }

            return TaskBlocks.Using(
                () => extension.PrepareQuotasSupportedAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<ReadOnlyCollectionPage<Quota>> ListQuotasAsync(this INetworkingService service, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IQuotasExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.Quotas);
            return TaskBlocks.Using(
                () => extension.PrepareListQuotasAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<Quota> GetQuotasAsync(this INetworkingService service, ProjectId projectId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IQuotasExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.Quotas);
            return TaskBlocks.Using(
                () => extension.PrepareGetQuotasAsync(projectId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Quota));
        }

        public static Task<Quota> UpdateQuotasAsync(this INetworkingService service, ProjectId projectId, QuotaData quotaData, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IQuotasExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.Quotas);
            return TaskBlocks.Using(
                () => extension.PrepareUpdateQuotasAsync(projectId, new QuotaRequest(quotaData), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Quota));
        }

        public static Task ResetQuotasAsync(this INetworkingService service, ProjectId projectId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IQuotasExtension extension = service.GetServiceExtension(PredefinedNetworkingExtensions.Quotas);
            return TaskBlocks.Using(
                () => extension.PrepareResetQuotasAsync(projectId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }
    }
}
