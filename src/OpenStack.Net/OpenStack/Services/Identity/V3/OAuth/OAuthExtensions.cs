namespace OpenStack.Services.Identity.V3.OAuth
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenStack.Collections;
    using Rackspace.Threading;

    public static class OAuthExtensions
    {
        #region Consumers API

        public static Task<Consumer> AddConsumerAsync(this IIdentityService service, ConsumerData consumerData, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IOAuthExtension extension = service.GetServiceExtension(PredefinedIdentityExtensions.OAuth);
            return TaskBlocks.Using(
                () => extension.PrepareAddConsumerAsync(new ConsumerRequest(consumerData), cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Consumer));
        }

        public static Task<ReadOnlyCollectionPage<Consumer>> ListConsumersAsync(this IIdentityService service, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IOAuthExtension extension = service.GetServiceExtension(PredefinedIdentityExtensions.OAuth);
            return TaskBlocks.Using(
                () => extension.PrepareListConsumersAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<Consumer> GetConsumerAsync(this IIdentityService service, ConsumerId consumerId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IOAuthExtension extension = service.GetServiceExtension(PredefinedIdentityExtensions.OAuth);
            return TaskBlocks.Using(
                () => extension.PrepareGetConsumerAsync(consumerId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Consumer));
        }

        public static Task<Consumer> UpdateConsumerAsync(this IIdentityService service, ConsumerId consumerId, ConsumerData consumerData, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IOAuthExtension extension = service.GetServiceExtension(PredefinedIdentityExtensions.OAuth);
            return TaskBlocks.Using(
                () => extension.PrepareUpdateConsumerAsync(consumerId, consumerData, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Consumer));
        }

        public static Task RemoveConsumerAsync(this IIdentityService service, ConsumerId consumerId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IOAuthExtension extension = service.GetServiceExtension(PredefinedIdentityExtensions.OAuth);
            return TaskBlocks.Using(
                () => extension.PrepareRemoveConsumerAsync(consumerId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        #endregion

        #region Delegated Auth API

        /* This section is "poorly described" in the documentation...
         * implementing it according to the spec alone will be nearly impossible.
         */

        //public static Task<AddRequestTokenApiCall> AddRequestTokenAsync(this IIdentityService service, RequestTokenData requestTokenData, CancellationToken cancellationToken);

        //public static Task<AuthorizeRequestTokenApiCall> AuthorizeRequestTokenAsync(this IIdentityService service, RequestTokenId requestTokenId, AuthorizeRequestTokenRequest request, CancellationToken cancellationToken);

        //public static Task<AddAccessTokenApiCall> AddAccessTokenAsync(this IIdentityService service);

        #endregion

        #region User Access Tokens API

        public static Task<ReadOnlyCollectionPage<AccessToken>> ListAccessTokensAsync(this IIdentityService service, UserId userId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IOAuthExtension extension = service.GetServiceExtension(PredefinedIdentityExtensions.OAuth);
            return TaskBlocks.Using(
                () => extension.PrepareListAccessTokensAsync(userId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<AccessToken> GetAccessTokenAsync(this IIdentityService service, UserId userId, AccessTokenId accessTokenId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IOAuthExtension extension = service.GetServiceExtension(PredefinedIdentityExtensions.OAuth);
            return TaskBlocks.Using(
                () => extension.PrepareGetAccessTokenAsync(userId, accessTokenId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.AccessToken));
        }

        public static Task RemoveAccessTokenAsync(this IIdentityService service, UserId userId, AccessTokenId accessTokenId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IOAuthExtension extension = service.GetServiceExtension(PredefinedIdentityExtensions.OAuth);
            return TaskBlocks.Using(
                () => extension.PrepareRemoveAccessTokenAsync(userId, accessTokenId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        public static Task<ReadOnlyCollectionPage<Role>> ListAccessTokenRolesAsync(this IIdentityService service, UserId userId, AccessTokenId accessTokenId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IOAuthExtension extension = service.GetServiceExtension(PredefinedIdentityExtensions.OAuth);
            return TaskBlocks.Using(
                () => extension.PrepareListAccessTokenRolesAsync(userId, accessTokenId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        public static Task<Role> GetAccessTokenRoleAsync(this IIdentityService service, UserId userId, AccessTokenId accessTokenId, RoleId roleId, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            IOAuthExtension extension = service.GetServiceExtension(PredefinedIdentityExtensions.OAuth);
            return TaskBlocks.Using(
                () => extension.PrepareGetAccessTokenRoleAsync(userId, accessTokenId, roleId, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2.Role));
        }

        #endregion
    }
}
