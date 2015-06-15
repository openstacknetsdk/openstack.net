namespace OpenStack.Services.Identity.V3.OAuth
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenStack.Net;
    using Rackspace.Net;
    using Rackspace.Threading;

    public class OAuthExtension : ServiceExtension<IIdentityService>, IOAuthExtension
    {
        public OAuthExtension(IIdentityService service, IHttpApiCallFactory httpApiCallFactory)
            : base(service, httpApiCallFactory)
        {
        }

        #region Consumers API

        public Task<AddConsumerApiCall> PrepareAddConsumerAsync(ConsumerRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/OS-OAUTH1/consumers");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Post, template, parameters, cancellationToken))
                .Select(task => new AddConsumerApiCall(factory.CreateJsonApiCall<ConsumerResponse>(task.Result)));
        }

        public Task<ListConsumersApiCall> PrepareListConsumersAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<GetConsumerApiCall> PrepareGetConsumerAsync(ConsumerId consumerId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/OS-OAUTH1/consumers/{consumer_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "consumer_id", consumerId.Value } };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new GetConsumerApiCall(factory.CreateJsonApiCall<ConsumerResponse>(task.Result)));
        }

        public Task<UpdateConsumerApiCall> PrepareUpdateConsumerAsync(ConsumerId consumerId, ConsumerData consumerData, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/OS-OAUTH1/consumers/{consumer_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "consumer_id", consumerId.Value } };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(new HttpMethod("PATCH"), template, parameters, consumerData, cancellationToken))
                .Select(task => new UpdateConsumerApiCall(factory.CreateJsonApiCall<ConsumerResponse>(task.Result)));
        }

        public Task<RemoveConsumerApiCall> PrepareRemoveConsumerAsync(ConsumerId consumerId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/OS-OAUTH1/consumers/{consumer_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "consumer_id", consumerId.Value } };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Delete, template, parameters, cancellationToken))
                .Select(task => new RemoveConsumerApiCall(factory.CreateBasicApiCall(task.Result, HttpCompletionOption.ResponseContentRead)));
        }

        #endregion

        #region Delegated Auth API

        /* This section is "poorly described" in the documentation...
         * implementing it according to the spec alone will be nearly impossible.
         */

        //public Task<AddRequestTokenApiCall> PrepareAddRequestTokenAsync(RequestTokenData requestTokenData, CancellationToken cancellationToken);

        //public Task<AuthorizeRequestTokenApiCall> PrepareAuthorizeRequestTokenAsync(RequestTokenId requestTokenId, AuthorizeRequestTokenRequest request, CancellationToken cancellationToken);

        //public Task<AddAccessTokenApiCall> PrepareAddAccessTokenAsync();

        #endregion

        #region User Access Tokens API

        public Task<ListAccessTokensApiCall> PrepareListAccessTokensAsync(UserId userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<GetAccessTokenApiCall> PrepareGetAccessTokenAsync(UserId userId, AccessTokenId accessTokenId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/OS-OAUTH1/users/{user_id}/access_tokens/{access_token_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "user_id", userId.Value }, { "access_token_id", accessTokenId.Value } };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new GetAccessTokenApiCall(factory.CreateJsonApiCall<AccessTokenResponse>(task.Result)));
        }

        public Task<RemoveAccessTokenApiCall> PrepareRemoveAccessTokenAsync(UserId userId, AccessTokenId accessTokenId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/OS-OAUTH1/users/{user_id}/access_tokens/{access_token_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "user_id", userId.Value }, { "access_token_id", accessTokenId.Value } };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Delete, template, parameters, cancellationToken))
                .Select(task => new RemoveAccessTokenApiCall(factory.CreateBasicApiCall(task.Result, HttpCompletionOption.ResponseContentRead)));
        }

        public Task<ListAccessTokenRolesApiCall> PrepareListAccessTokenRolesAsync(UserId userId, AccessTokenId accessTokenId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<GetAccessTokenRoleApiCall> PrepareGetAccessTokenRoleAsync(UserId userId, AccessTokenId accessTokenId, RoleId roleId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/OS-OAUTH1/users/{user_id}/access_tokens/{access_token_id}/roles/{role_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "user_id", userId.Value },
                { "access_token_id", accessTokenId.Value },
                { "role_id", roleId.Value },
            };

            IHttpApiCallFactory factory = HttpApiCallFactory;
            return Service.GetBaseUriAsync(cancellationToken)
                .Then(Service.PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new GetAccessTokenRoleApiCall(factory.CreateJsonApiCall<RoleResponse>(task.Result)));
        }

        #endregion
    }
}
