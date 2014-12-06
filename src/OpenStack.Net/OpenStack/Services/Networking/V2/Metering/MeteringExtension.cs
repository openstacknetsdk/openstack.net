namespace OpenStack.Services.Networking.V2.Metering
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OpenStack.Collections;
    using OpenStack.Net;
    using Rackspace.Net;
    using Rackspace.Threading;
    using ExtensionAlias = Identity.V2.ExtensionAlias;

    public class MeteringExtension : ServiceExtension<INetworkingService>, IMeteringExtension
    {
#warning this alias is not correct.
        public static readonly ExtensionAlias ExtensionAlias = new ExtensionAlias("");

        public MeteringExtension(INetworkingService service, IHttpApiCallFactory httpApiCallFactory)
            : base(service, httpApiCallFactory)
        {
        }

        public Task<MeteringSupportedApiCall> PrepareMeteringSupportedAsync(CancellationToken cancellationToken)
        {
            return Service.PrepareListExtensionsAsync(cancellationToken)
                .Select(task => new MeteringSupportedApiCall(task.Result));
        }

        public Task<AddMeteringLabelApiCall> PrepareAddMeteringLabelAsync(MeteringLabelRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("metering-labels");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Post, template, parameters, request, cancellationToken))
                .Select(task => new AddMeteringLabelApiCall(factory.CreateJsonApiCall<MeteringLabelResponse>(task.Result)));
        }

        public Task<ListMeteringLabelsApiCall> PrepareListMeteringLabelsAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("metering-labels");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            Func<HttpResponseMessage, CancellationToken, Task<ReadOnlyCollectionPage<MeteringLabel>>> deserializeResult =
                (responseMessage, innerCancellationToken) =>
                {
                    if (!HttpApiCall.IsAcceptable(responseMessage))
                        throw new HttpWebException(responseMessage);

                    return responseMessage.Content.ReadAsStringAsync()
                        .Select(
                            content =>
                            {
                                JObject jsonObject = JsonConvert.DeserializeObject<JObject>(content.Result);
                                if (jsonObject == null)
                                    return null;

                                JToken meteringLabelsToken = jsonObject["metering_labels"];
                                if (meteringLabelsToken == null)
                                    return null;

                                // the pagination behavior, if any, is not described in the documentation
                                MeteringLabel[] meteringLabels = meteringLabelsToken.ToObject<MeteringLabel[]>();
                                ReadOnlyCollectionPage<MeteringLabel> result = new BasicReadOnlyCollectionPage<MeteringLabel>(meteringLabels, null);
                                return result;
                            });
                };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new ListMeteringLabelsApiCall(factory.CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }

        public Task<GetMeteringLabelApiCall> PrepareGetMeteringLabelAsync(MeteringLabelId meteringLabelId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("metering-labels/{metering_label_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "metering_label_id", meteringLabelId.Value } };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new GetMeteringLabelApiCall(factory.CreateJsonApiCall<MeteringLabelResponse>(task.Result)));
        }

        public Task<RemoveMeteringLabelApiCall> PrepareRemoveMeteringLabelAsync(MeteringLabelId meteringLabelId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("metering-labels/{metering_label_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "metering_label_id", meteringLabelId.Value } };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Delete, template, parameters, cancellationToken))
                .Select(task => new RemoveMeteringLabelApiCall(factory.CreateBasicApiCall(task.Result, HttpCompletionOption.ResponseContentRead)));
        }

        public Task<AddMeteringLabelRuleApiCall> PrepareAddMeteringLabelRuleAsync(MeteringLabelRuleRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("metering-label-rules");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Post, template, parameters, request, cancellationToken))
                .Select(task => new AddMeteringLabelRuleApiCall(factory.CreateJsonApiCall<MeteringLabelRuleResponse>(task.Result)));
        }

        public Task<ListMeteringLabelRulesApiCall> PrepareListMeteringLabelRulesAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("metering-label-rules");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            Func<HttpResponseMessage, CancellationToken, Task<ReadOnlyCollectionPage<MeteringLabelRule>>> deserializeResult =
                (responseMessage, innerCancellationToken) =>
                {
                    if (!HttpApiCall.IsAcceptable(responseMessage))
                        throw new HttpWebException(responseMessage);

                    return responseMessage.Content.ReadAsStringAsync()
                        .Select(
                            content =>
                            {
                                JObject jsonObject = JsonConvert.DeserializeObject<JObject>(content.Result);
                                if (jsonObject == null)
                                    return null;

                                JToken meteringLabelRulesToken = jsonObject["metering_label_rules"];
                                if (meteringLabelRulesToken == null)
                                    return null;

                                // the pagination behavior, if any, is not described in the documentation
                                MeteringLabelRule[] meteringLabelRules = meteringLabelRulesToken.ToObject<MeteringLabelRule[]>();
                                ReadOnlyCollectionPage<MeteringLabelRule> result = new BasicReadOnlyCollectionPage<MeteringLabelRule>(meteringLabelRules, null);
                                return result;
                            });
                };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new ListMeteringLabelRulesApiCall(factory.CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }

        public Task<GetMeteringLabelRuleApiCall> PrepareGetMeteringLabelRuleAsync(MeteringLabelRuleId meteringLabelRuleId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("metering-label-rules/{metering_label_rule_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "metering_label_rule_id", meteringLabelRuleId.Value } };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new GetMeteringLabelRuleApiCall(factory.CreateJsonApiCall<MeteringLabelRuleResponse>(task.Result)));
        }

        public Task<RemoveMeteringLabelRuleApiCall> PrepareRemoveMeteringLabelRuleAsync(MeteringLabelRuleId meteringLabelRuleId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("metering-label-rules/{metering_label_rule_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "metering_label_rule_id", meteringLabelRuleId.Value } };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Delete, template, parameters, cancellationToken))
                .Select(task => new RemoveMeteringLabelRuleApiCall(factory.CreateBasicApiCall(task.Result, HttpCompletionOption.ResponseContentRead)));
        }
    }
}
