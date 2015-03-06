namespace OpenStack.Services.Networking.V2.Quotas
{
    using System.Threading;
    using System.Threading.Tasks;
    using OpenStack.Services.Identity;

    public interface IQuotasExtension
    {
        Task<QuotasSupportedApiCall> PrepareQuotasSupportedAsync(CancellationToken cancellationToken);

        Task<ListQuotasApiCall> PrepareListQuotasAsync(CancellationToken cancellationToken);

        Task<GetQuotasApiCall> PrepareGetQuotasAsync(ProjectId projectId, CancellationToken cancellationToken);

        Task<UpdateQuotasApiCall> PrepareUpdateQuotasAsync(ProjectId projectId, QuotaRequest request, CancellationToken cancellationToken);

        Task<ResetQuotasApiCall> PrepareResetQuotasAsync(ProjectId projectId, CancellationToken cancellationToken);
    }
}
