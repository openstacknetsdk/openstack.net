namespace OpenStack.Services.ContentDelivery.V1
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// This is the base interface for the OpenStack Content Delivery Service V1.
    /// </summary>
    /// <seealso href="">OpenStack Content Delivery API V1 Reference</seealso>
    /// <preliminary/>
    public interface IContentDeliveryService : IHttpService, IExtensibleService<IContentDeliveryService>
    {
        #region Service Information

        Task<GetHomeApiCall> PrepareGetHomeAsync(CancellationToken cancellationToken);

        Task<GetHealthApiCall> PrepareGetHealthAsync(CancellationToken cancellationToken);

        Task<GetSubsystemHealthApiCall> PrepareGetSubsystemHealthAsync(SubsystemId subsystemId, CancellationToken cancellationToken);

        Task<PingApiCall> PreparePingAsync(CancellationToken cancellationToken);

        #endregion

        #region Services

        Task<ListServicesApiCall> PrepareListServicesAsync(CancellationToken cancellationToken);

        Task<GetServiceApiCall> PrepareGetServiceAsync(ServiceId serviceId, CancellationToken cancellationToken);

        Task<AddServiceApiCall> PrepareAddServiceAsync(ServiceData serviceData, CancellationToken cancellationToken);

        Task<UpdateServiceApiCall> PrepareUpdateServiceAsync(ServiceId serviceId, ServiceData updatedServiceData, CancellationToken cancellationToken);

        Task<RemoveServiceApiCall> PrepareRemoveServiceAsync(ServiceId serviceId, CancellationToken cancellationToken);

        Task<RemoveAssetApiCall> PrepareRemoveAssetAsync(ServiceId serviceId, CancellationToken cancellationToken);

        #endregion

        #region Flavors

        Task<ListFlavorsApiCall> PrepareListFlavorsAsync(CancellationToken cancellationToken);

        Task<GetFlavorApiCall> PrepareGetFlavorAsync(FlavorId flavorId, CancellationToken cancellationToken);

        Task<AddFlavorApiCall> PrepareAddFlavorAsync(FlavorData flavorData, CancellationToken cancellationToken);

        Task<RemoveFlavorApiCall> PrepareRemoveFlavorAsync(FlavorId flavorId, CancellationToken cancellationToken);

        #endregion
    }
}
