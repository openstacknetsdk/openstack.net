namespace OpenStack.Services.ContentDelivery.V1
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenStack.Collections;
    using OpenStack.ObjectModel.JsonHome;
    using Rackspace.Threading;

    /// <summary>
    /// This class provides extension methods that simplify the process of preparing and sending Content Delivery
    /// Service HTTP API calls for the most common use cases.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public static class ContentDeliveryServiceExtensions
    {
        public static Task<HomeDocument> GetHomeAsync(this IContentDeliveryService service, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareGetHomeAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        //public static Task<GetHealthApiCall> GetHealthAsync(this IContentDeliveryService service, CancellationToken cancellationToken)
        //{
        //    if (service == null)
        //        throw new ArgumentNullException("service");

        //    return TaskBlocks.Using(
        //        () => service.PrepareGetHealthAsync(cancellationToken),
        //        task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        //}

        //public static Task<GetSubsystemHealthApiCall> GetSubsystemHealthAsync(this IContentDeliveryService service, SubsystemId subsystemId, CancellationToken cancellationToken)
        //{
        //    if (service == null)
        //        throw new ArgumentNullException("service");

        //    return TaskBlocks.Using(
        //        () => service.PrepareGetSubsystemHealthAsync(subsystemId, cancellationToken),
        //        task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        //}

        public static Task PingAsync(this IContentDeliveryService service, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PreparePingAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        public static Task<ReadOnlyCollectionPage<Service>> ListServicesAsync(this IContentDeliveryService service, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareListServicesAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<Service> GetServiceAsync(this IContentDeliveryService service, ServiceId serviceId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareGetServiceAsync(serviceId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<ServiceId> AddServiceAsync(this IContentDeliveryService service, ServiceData serviceData, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareAddServiceAsync(serviceData, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task UpdateServiceAsync(this IContentDeliveryService service, ServiceId serviceId, ServiceData updatedServiceData, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareUpdateServiceAsync(serviceId, updatedServiceData, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        public static Task RemoveServiceAsync(this IContentDeliveryService service, ServiceId serviceId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareRemoveServiceAsync(serviceId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        public static Task RemoveAssetAsync(this IContentDeliveryService service, ServiceId serviceId, CancellationToken cancellationToken, string urlOfAsset = null, bool deleteAll = false)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareRemoveAssetAsync(serviceId, cancellationToken, urlOfAsset, deleteAll),
                task => task.Result.SendAsync(cancellationToken));
        }

        public static Task<ReadOnlyCollectionPage<Flavor>> ListFlavorsAsync(this IContentDeliveryService service, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareListFlavorsAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<Flavor> GetFlavorAsync(this IContentDeliveryService service, FlavorId flavorId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareGetFlavorAsync(flavorId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<FlavorId> AddFlavorAsync(this IContentDeliveryService service, FlavorData flavorData, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareAddFlavorAsync(flavorData, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task RemoveFlavorAsync(this IContentDeliveryService service, FlavorId flavorId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareRemoveFlavorAsync(flavorId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }
    }
}
