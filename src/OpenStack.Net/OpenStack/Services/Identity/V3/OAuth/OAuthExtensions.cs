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

    public static class OAuthExtensions
    {
        #region Consumers API

        public static Task<AddConsumerApiCall> PrepareAddConsumerAsync(this IIdentityService client, ConsumerRequest request, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/OS-OAUTH1/consumers");
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            IHttpApiCallFactory factory = client.GetHttpApiCallFactory();
            return client.GetBaseUriAsync(cancellationToken)
                .Then(client.PrepareRequestAsyncFunc(HttpMethod.Post, template, parameters, cancellationToken))
                .Select(task => new AddConsumerApiCall(factory.CreateJsonApiCall<ConsumerResponse>(task.Result)));
        }

        public static Task<ListConsumersApiCall> PrepareListConsumersAsync(this IIdentityService client, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public static Task<GetConsumerApiCall> PrepareGetConsumerAsync(this IIdentityService client, ConsumerId consumerId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/OS-OAUTH1/consumers/{consumer_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "consumer_id", consumerId.Value } };

            IHttpApiCallFactory factory = client.GetHttpApiCallFactory();
            return client.GetBaseUriAsync(cancellationToken)
                .Then(client.PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new GetConsumerApiCall(factory.CreateJsonApiCall<ConsumerResponse>(task.Result)));
        }

        public static Task<UpdateConsumerApiCall> PrepareUpdateConsumerAsync(this IIdentityService client, ConsumerId consumerId, ConsumerData consumerData, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/OS-OAUTH1/consumers/{consumer_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "consumer_id", consumerId.Value } };

            IHttpApiCallFactory factory = client.GetHttpApiCallFactory();
            return client.GetBaseUriAsync(cancellationToken)
                .Then(client.PrepareRequestAsyncFunc(new HttpMethod("PATCH"), template, parameters, consumerData, cancellationToken))
                .Select(task => new UpdateConsumerApiCall(factory.CreateJsonApiCall<ConsumerResponse>(task.Result)));
        }

        public static Task<RemoveConsumerApiCall> PrepareRemoveConsumerAsync(this IIdentityService client, ConsumerId consumerId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/OS-OAUTH1/consumers/{consumer_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "consumer_id", consumerId.Value } };

            IHttpApiCallFactory factory = client.GetHttpApiCallFactory();
            return client.GetBaseUriAsync(cancellationToken)
                .Then(client.PrepareRequestAsyncFunc(HttpMethod.Delete, template, parameters, cancellationToken))
                .Select(task => new RemoveConsumerApiCall(factory.CreateBasicApiCall(task.Result, HttpCompletionOption.ResponseContentRead)));
        }

        #endregion

        #region Delegated Auth API

        /* This section is "poorly described" in the documentation...
         * implementing it according to the spec alone will be nearly impossible.
         */

        //public static Task<AddRequestTokenApiCall> PrepareAddRequestTokenAsync(this IIdentityService client, RequestTokenData requestTokenData, CancellationToken cancellationToken);

        //public static Task<AuthorizeRequestTokenApiCall> PrepareAuthorizeRequestTokenAsync(this IIdentityService client, RequestTokenId requestTokenId, AuthorizeRequestTokenRequest request, CancellationToken cancellationToken);

        //public static Task<AddAccessTokenApiCall> PrepareAddAccessTokenAsync();

        #endregion

        #region User Access Tokens API

        public static Task<ListAccessTokensApiCall> PrepareListAccessTokensAsync(this IIdentityService client, UserId userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public static Task<GetAccessTokenApiCall> PrepareGetAccessTokenAsync(this IIdentityService client, UserId userId, AccessTokenId accessTokenId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/OS-OAUTH1/users/{user_id}/access_tokens/{access_token_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "user_id", userId.Value }, { "access_token_id", accessTokenId.Value } };

            IHttpApiCallFactory factory = client.GetHttpApiCallFactory();
            return client.GetBaseUriAsync(cancellationToken)
                .Then(client.PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new GetAccessTokenApiCall(factory.CreateJsonApiCall<AccessTokenResponse>(task.Result)));
        }

        public static Task<RemoveAccessTokenApiCall> PrepareRemoveAccessTokenAsync(this IIdentityService client, UserId userId, AccessTokenId accessTokenId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/OS-OAUTH1/users/{user_id}/access_tokens/{access_token_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "user_id", userId.Value }, { "access_token_id", accessTokenId.Value } };

            IHttpApiCallFactory factory = client.GetHttpApiCallFactory();
            return client.GetBaseUriAsync(cancellationToken)
                .Then(client.PrepareRequestAsyncFunc(HttpMethod.Delete, template, parameters, cancellationToken))
                .Select(task => new RemoveAccessTokenApiCall(factory.CreateBasicApiCall(task.Result, HttpCompletionOption.ResponseContentRead)));
        }

        public static Task<ListAccessTokenRolesApiCall> PrepareListAccessTokenRolesAsync(this IIdentityService client, UserId userId, AccessTokenId accessTokenId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public static Task<GetAccessTokenRoleApiCall> PrepareGetAccessTokenRoleAsync(this IIdentityService client, UserId userId, AccessTokenId accessTokenId, RoleId roleId, CancellationToken cancellationToken)
        {
            UriTemplate template = new UriTemplate("v3/OS-OAUTH1/users/{user_id}/access_tokens/{access_token_id}/roles/{role_id}");
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "user_id", userId.Value },
                { "access_token_id", accessTokenId.Value },
                { "role_id", roleId.Value },
            };

            IHttpApiCallFactory factory = client.GetHttpApiCallFactory();
            return client.GetBaseUriAsync(cancellationToken)
                .Then(client.PrepareRequestAsyncFunc(HttpMethod.Get, template, parameters, cancellationToken))
                .Select(task => new GetAccessTokenRoleApiCall(factory.CreateJsonApiCall<RoleResponse>(task.Result)));
        }

        #endregion
    }
}
