namespace OpenStack.Services.Networking.V2.Metering
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IMeteringExtension
    {
        Task<MeteringSupportedApiCall> PrepareMeteringSupportedAsync(CancellationToken cancellationToken);

        Task<AddMeteringLabelApiCall> PrepareAddMeteringLabelAsync(MeteringLabelRequest request, CancellationToken cancellationToken);

        Task<ListMeteringLabelsApiCall> PrepareListMeteringLabelsAsync(CancellationToken cancellationToken);

        Task<GetMeteringLabelApiCall> PrepareGetMeteringLabelAsync(MeteringLabelId meteringLabelId, CancellationToken cancellationToken);

        Task<RemoveMeteringLabelApiCall> PrepareRemoveMeteringLabelAsync(MeteringLabelId meteringLabelId, CancellationToken cancellationToken);

        Task<AddMeteringLabelRuleApiCall> PrepareAddMeteringLabelRuleAsync(MeteringLabelRuleRequest request, CancellationToken cancellationToken);

        Task<ListMeteringLabelRulesApiCall> PrepareListMeteringLabelRulesAsync(CancellationToken cancellationToken);

        Task<GetMeteringLabelRuleApiCall> PrepareGetMeteringLabelRuleAsync(MeteringLabelRuleId meteringLabelRuleId, CancellationToken cancellationToken);

        Task<RemoveMeteringLabelRuleApiCall> PrepareRemoveMeteringLabelRuleAsync(MeteringLabelRuleId meteringLabelRuleId, CancellationToken cancellationToken);
    }
}
