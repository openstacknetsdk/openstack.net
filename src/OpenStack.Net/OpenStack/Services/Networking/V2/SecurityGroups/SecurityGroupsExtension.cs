namespace OpenStack.Services.Networking.V2.SecurityGroups
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

    public class SecurityGroupsExtension : ServiceExtension<INetworkingService>, ISecurityGroupsExtension
    {
        public static readonly ExtensionAlias ExtensionAlias = new ExtensionAlias("security-groups");

        public SecurityGroupsExtension(INetworkingService service, IHttpApiCallFactory httpApiCallFactory)
            : base(service, httpApiCallFactory)
        {
        }

        public Task<SecurityGroupsSupportedApiCall> PrepareSecurityGroupsSupportedAsync(CancellationToken cancellationToken)
        {
            return Service.PrepareListExtensionsAsync(cancellationToken)
                .Select(task => new SecurityGroupsSupportedApiCall(task.Result));
        }

        public Task<AddSecurityGroupApiCall> PrepareAddSecurityGroupAsync(SecurityGroupRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("security-groups");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Post, template, parameters, request, cancellationToken))
                .Select(task => new AddSecurityGroupApiCall(factory.CreateJsonApiCall<SecurityGroupResponse>(task.Result)));
        }

        public Task<ListSecurityGroupsApiCall> PrepareListSecurityGroupsAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("security-groups");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            Func<HttpResponseMessage, CancellationToken, Task<ReadOnlyCollectionPage<SecurityGroup>>> deserializeResult =
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

                                JToken securityGroupsToken = jsonObject["security_groups"];
                                if (securityGroupsToken == null)
                                    return null;

                                // the pagination behavior, if any, is not described in the documentation
                                SecurityGroup[] securityGroups = securityGroupsToken.ToObject<SecurityGroup[]>();
                                ReadOnlyCollectionPage<SecurityGroup> result = new BasicReadOnlyCollectionPage<SecurityGroup>(securityGroups, null);
                                return result;
                            });
                };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new ListSecurityGroupsApiCall(factory.CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }

        public Task<GetSecurityGroupApiCall> PrepareGetSecurityGroupAsync(SecurityGroupId securityGroupId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("security-groups/{security_group_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "security_group_id", securityGroupId.Value } };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new GetSecurityGroupApiCall(factory.CreateJsonApiCall<SecurityGroupResponse>(task.Result)));
        }

        //public Task<UpdateSecurityGroupApiCall> PrepareUpdateSecurityGroupAsync(SecurityGroupId securityGroupId, SecurityGroupRequest request, CancellationToken cancellationToken)
        //{
        //    UriTemplate template = new UriTemplate("security-groups/{security_group_id}");
        //    Dictionary<string, string> parameters = new Dictionary<string, string> { { "security_group_id", securityGroupId.Value } };

        //    IHttpApiCallFactory factory = GetHttpApiCallFactory(client);
        //    return Service.GetBaseUriAsync(cancellationToken)
        //        .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Put, template, parameters, request, cancellationToken))
        //        .Select(task => new UpdateSecurityGroupApiCall(factory.CreateJsonApiCall<SecurityGroupResponse>(task.Result)));
        //}

        public Task<RemoveSecurityGroupApiCall> PrepareRemoveSecurityGroupAsync(SecurityGroupId securityGroupId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("security-groups/{security_group_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "security_group_id", securityGroupId.Value } };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Delete, template, parameters, cancellationToken))
                .Select(task => new RemoveSecurityGroupApiCall(factory.CreateBasicApiCall(task.Result, HttpCompletionOption.ResponseContentRead)));
        }

        public Task<AddSecurityGroupRuleApiCall> PrepareAddSecurityGroupRuleAsync(SecurityGroupRuleRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("security-group-rules");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Post, template, parameters, request, cancellationToken))
                .Select(task => new AddSecurityGroupRuleApiCall(factory.CreateJsonApiCall<SecurityGroupRuleResponse>(task.Result)));
        }

        public Task<ListSecurityGroupRulesApiCall> PrepareListSecurityGroupRulesAsync(CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("security-group-rules");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            Func<HttpResponseMessage, CancellationToken, Task<ReadOnlyCollectionPage<SecurityGroupRule>>> deserializeResult =
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

                                JToken securityGroupRulesToken = jsonObject["security_group_rules"];
                                if (securityGroupRulesToken == null)
                                    return null;

                                // the pagination behavior, if any, is not described in the documentation
                                SecurityGroupRule[] securityGroupRules = securityGroupRulesToken.ToObject<SecurityGroupRule[]>();
                                ReadOnlyCollectionPage<SecurityGroupRule> result = new BasicReadOnlyCollectionPage<SecurityGroupRule>(securityGroupRules, null);
                                return result;
                            });
                };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new ListSecurityGroupRulesApiCall(factory.CreateCustomApiCall(task.Result, HttpCompletionOption.ResponseContentRead, deserializeResult)));
        }

        public Task<GetSecurityGroupRuleApiCall> PrepareGetSecurityGroupRuleAsync(SecurityGroupRuleId securityGroupRuleId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("security-group-rules/{security_group_rule_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "security_group_rule_id", securityGroupRuleId.Value } };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new GetSecurityGroupRuleApiCall(factory.CreateJsonApiCall<SecurityGroupRuleResponse>(task.Result)));
        }

        //public Task<UpdateSecurityGroupRuleApiCall> PrepareUpdateSecurityGroupRuleAsync(SecurityGroupRuleId securityGroupRuleId, SecurityGroupRuleRequest request, CancellationToken cancellationToken)
        //{
        //    UriTemplate template = new UriTemplate("security-group-rules/{security_group_rule_id}");
        //    Dictionary<string, string> parameters = new Dictionary<string, string> { { "security_group_rule_id", securityGroupRuleId.Value } };

        //    IHttpApiCallFactory factory = GetHttpApiCallFactory(client);
        //    return Service.GetBaseUriAsync(cancellationToken)
        //        .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Put, template, parameters, request, cancellationToken))
        //        .Select(task => new UpdateSecurityGroupRuleApiCall(factory.CreateJsonApiCall<SecurityGroupRuleResponse>(task.Result)));
        //}

        public Task<RemoveSecurityGroupRuleApiCall> PrepareRemoveSecurityGroupRuleAsync(SecurityGroupRuleId securityGroupRuleId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("security-group-rules/{security_group_rule_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "security_group_rule_id", securityGroupRuleId.Value } };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Delete, template, parameters, cancellationToken))
                .Select(task => new RemoveSecurityGroupRuleApiCall(factory.CreateBasicApiCall(task.Result, HttpCompletionOption.ResponseContentRead)));
        }
    }
}
