namespace OpenStack.Services.Networking.V2.Quotas
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
    using OpenStack.Services.Identity;
    using Rackspace.Net;
    using Rackspace.Threading;
    using ExtensionAlias = Identity.V2.ExtensionAlias;

    public class QuotasExtension : ServiceExtension<INetworkingService>, IQuotasExtension
    {
        public static readonly ExtensionAlias ExtensionAlias = new ExtensionAlias("quotas");

        public QuotasExtension(INetworkingService service, IHttpApiCallFactory httpApiCallFactory)
            : base(service, httpApiCallFactory)
        {
        }

        public Task<QuotasSupportedApiCall> PrepareQuotasSupportedAsync(CancellationToken cancellationToken)
        {
            return Service.PrepareListExtensionsAsync(cancellationToken)
                .Select(task => new QuotasSupportedApiCall(task.Result));
        }

        public Task<ListQuotasApiCall> PrepareListQuotasAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("quotas");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            Func<HttpResponseMessage, CancellationToken, Task<ReadOnlyCollectionPage<Quota>>> deserializeResult =
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

                                JToken quotasToken = jsonObject["quotas"];
                                if (quotasToken == null)
                                    return null;

                                // the pagination behavior, if any, is not described in the documentation
                                Quota[] quotas = quotasToken.ToObject<Quota[]>();
                                ReadOnlyCollectionPage<Quota> result = new BasicReadOnlyCollectionPage<Quota>(quotas, null);
                                return result;
                            });
                };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new ListQuotasApiCall(factory.CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }

        public Task<GetQuotasApiCall> PrepareGetQuotasAsync(ProjectId projectId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("quotas/{project_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "project_id", projectId.Value } };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new GetQuotasApiCall(factory.CreateJsonApiCall<QuotaResponse>(task.Result)));
        }

        public Task<UpdateQuotasApiCall> PrepareUpdateQuotasAsync(ProjectId projectId, QuotaRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("quotas/{project_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "project_id", projectId.Value } };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Put, template, parameters, request, cancellationToken))
                .Select(task => new UpdateQuotasApiCall(factory.CreateJsonApiCall<QuotaResponse>(task.Result)));
        }

        public Task<ResetQuotasApiCall> PrepareResetQuotasAsync(ProjectId projectId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("quotas/{project_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "project_id", projectId.Value } };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Delete, template, parameters, cancellationToken))
                .Select(task => new ResetQuotasApiCall(factory.CreateBasicApiCall(task.Result, HttpCompletionOption.ResponseContentRead)));
        }
    }
}
