namespace OpenStack.Services.Identity.V3.OAuth
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IOAuthExtension
    {
        #region Consumers API

        Task<AddConsumerApiCall> PrepareAddConsumerAsync(ConsumerRequest request, CancellationToken cancellationToken);

        Task<ListConsumersApiCall> PrepareListConsumersAsync(CancellationToken cancellationToken);

        Task<GetConsumerApiCall> PrepareGetConsumerAsync(ConsumerId consumerId, CancellationToken cancellationToken);

        Task<UpdateConsumerApiCall> PrepareUpdateConsumerAsync(ConsumerId consumerId, ConsumerData consumerData, CancellationToken cancellationToken);

        Task<RemoveConsumerApiCall> PrepareRemoveConsumerAsync(ConsumerId consumerId, CancellationToken cancellationToken);

        #endregion

        #region Delegated Auth API

        /* This section is "poorly described" in the documentation...
         * implementing it according to the spec alone will be nearly impossible.
         */

        //Task<AddRequestTokenApiCall> PrepareAddRequestTokenAsync(RequestTokenData requestTokenData, CancellationToken cancellationToken);

        //Task<AuthorizeRequestTokenApiCall> PrepareAuthorizeRequestTokenAsync(RequestTokenId requestTokenId, AuthorizeRequestTokenRequest request, CancellationToken cancellationToken);

        //Task<AddAccessTokenApiCall> PrepareAddAccessTokenAsync();

        #endregion

        #region User Access Tokens API

        Task<ListAccessTokensApiCall> PrepareListAccessTokensAsync(UserId userId, CancellationToken cancellationToken);

        Task<GetAccessTokenApiCall> PrepareGetAccessTokenAsync(UserId userId, AccessTokenId accessTokenId, CancellationToken cancellationToken);

        Task<RemoveAccessTokenApiCall> PrepareRemoveAccessTokenAsync(UserId userId, AccessTokenId accessTokenId, CancellationToken cancellationToken);

        Task<ListAccessTokenRolesApiCall> PrepareListAccessTokenRolesAsync(UserId userId, AccessTokenId accessTokenId, CancellationToken cancellationToken);

        Task<GetAccessTokenRoleApiCall> PrepareGetAccessTokenRoleAsync(UserId userId, AccessTokenId accessTokenId, RoleId roleId, CancellationToken cancellationToken);

        #endregion
    }
}
