namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using System.Collections.Immutable;

    /// <summary>
    /// This class provides extension methods for obtaining specific account information from the account metadata
    /// provided by the service.
    /// </summary>
    /// <seealso cref="GetAccountMetadataApiCall"/>
    /// <seealso cref="IObjectStorageService.PrepareGetAccountMetadataAsync"/>
    /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/GET_showAccountDetails__v1__account__storage_account_services.html">Show account details and list containers (OpenStack Object Storage API V1 Reference)</seealso>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public static class AccountMetadataExtensions
    {
        /// <summary>
        /// Gets the name of the <c>X-Account-Bytes-Used</c> HTTP header, which the Object Storage Service may
        /// include with the <see cref="AccountMetadata"/> to specify the total size of all objects stored in the
        /// Object Storage Service for the account.
        /// </summary>
        /// <seealso cref="GetBytesUsed(AccountMetadata)"/>
        public const string AccountBytesUsed = "X-Account-Bytes-Used";

        /// <summary>
        /// Gets the name of the <c>X-Account-Container-Count</c> HTTP header, which the Object Storage Service may
        /// include with the <see cref="AccountMetadata"/> to specify the number of containers in the account.
        /// </summary>
        /// <seealso cref="GetContainerCount(AccountMetadata)"/>
        public const string AccountContainerCount = "X-Account-Container-Count";

        /// <summary>
        /// Gets the name of the <c>X-Account-Object-Count</c> HTTP header, which the Object Storage Service may
        /// include with the <see cref="AccountMetadata"/> to specify the number of objects in the account.
        /// </summary>
        /// <seealso cref="GetObjectCount(AccountMetadata)"/>
        public const string AccountObjectCount = "X-Account-Object-Count";

        /// <summary>
        /// Gets the key for the <c>Temp-URL-Key</c> metadata value, which specifies the first of two account secret
        /// keys which may be used for constructing URIs for specific unauthenticated read and/or write operations to an
        /// Object Storage account.
        /// </summary>
        /// <seealso cref="GetSecretKey(AccountMetadata)"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/tempurl.html#account-secret-keys-temp-url">Account secret keys - Temporary URL middleware (OpenStack Object Storage API V1 Reference)</seealso>
        public const string AccountSecretKey = "Temp-URL-Key";

        /// <summary>
        /// Gets the key for the <c>Temp-URL-Key-2</c> metadata value, which specifies the second of two account secret
        /// keys which may be used for constructing URIs for specific unauthenticated read and/or write operations to an
        /// Object Storage account.
        /// </summary>
        /// <seealso cref="GetSecretKey2(AccountMetadata)"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/tempurl.html#account-secret-keys-temp-url">Account secret keys - Temporary URL middleware (OpenStack Object Storage API V1 Reference)</seealso>
        public const string AccountSecretKey2 = "Temp-URL-Key-2";

        /// <summary>
        /// Gets the total size of objects stored in the account.
        /// <token>OpenStackNotDefined</token>
        /// </summary>
        /// <remarks>
        /// <para>
        /// This field is <see href="http://en.wikipedia.org/wiki/Eventual_consistency">eventually consistent</see>.
        /// </para>
        /// <note type="note">
        /// <para>The <paramref name="metadata"/> argument should be an instance provided by the service, since
        /// instances created in user code would not contain the necessary header values.</para>
        /// </note>
        /// </remarks>
        /// <param name="metadata">An <see cref="AccountMetadata"/> instance containing the metadata associated with the
        /// account.</param>
        /// <returns>
        /// <para>The total size of all objects in the account.</para>
        /// <para>-or-</para>
        /// <para><see langword="null"/> if <paramref name="metadata"/> does not contain a value for the
        /// <see cref="AccountBytesUsed"/> header, or if the header value cannot be parsed as an integer.</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="metadata"/> is <see langword="null"/>.
        /// </exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/GET_showAccountDetails__v1__account__storage_account_services.html">Show account details and list containers (OpenStack Object Storage API V1 Reference)</seealso>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/GET_listcontainers_v1__account__accountServicesOperations_d1e000.html">Show Account Details and List Containers (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        public static long? GetBytesUsed(this AccountMetadata metadata)
        {
            if (metadata == null)
                throw new ArgumentNullException("metadata");

            string rawValue;
            if (!metadata.Headers.TryGetValue(AccountBytesUsed, out rawValue))
                return null;

            long value;
            if (!long.TryParse(rawValue, out value))
                return null;

            return value;
        }

        /// <summary>
        /// Gets the total number of containers in the account.
        /// <token>OpenStackNotDefined</token>
        /// </summary>
        /// <remarks>
        /// <para>
        /// This field is <see href="http://en.wikipedia.org/wiki/Eventual_consistency">eventually consistent</see>.
        /// </para>
        /// <note type="note">
        /// <para>The <paramref name="metadata"/> argument should be an instance provided by the service, since
        /// instances created in user code would not contain the necessary header values.</para>
        /// </note>
        /// </remarks>
        /// <param name="metadata">An <see cref="AccountMetadata"/> instance containing the metadata associated with the
        /// account.</param>
        /// <returns>
        /// <para>The total number of containers in the account.</para>
        /// <para>-or-</para>
        /// <para><see langword="null"/> if <paramref name="metadata"/> does not contain a value for the
        /// <see cref="AccountContainerCount"/> header, or if the header value cannot be parsed as an integer.</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="metadata"/> is <see langword="null"/>.
        /// </exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/GET_showAccountDetails__v1__account__storage_account_services.html">Show account details and list containers (OpenStack Object Storage API V1 Reference)</seealso>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/GET_listcontainers_v1__account__accountServicesOperations_d1e000.html">Show Account Details and List Containers (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        public static long? GetContainerCount(this AccountMetadata metadata)
        {
            if (metadata == null)
                throw new ArgumentNullException("metadata");

            string rawValue;
            if (!metadata.Headers.TryGetValue(AccountContainerCount, out rawValue))
                return null;

            long value;
            if (!long.TryParse(rawValue, out value))
                return null;

            return value;
        }

        /// <summary>
        /// Gets the total number of objects in the account.
        /// <token>OpenStackNotDefined</token>
        /// </summary>
        /// <remarks>
        /// <para>
        /// This field is <see href="http://en.wikipedia.org/wiki/Eventual_consistency">eventually consistent</see>.
        /// </para>
        /// <note type="note">
        /// <para>The <paramref name="metadata"/> argument should be an instance provided by the service, since
        /// instances created in user code would not contain the necessary header values.</para>
        /// </note>
        /// </remarks>
        /// <param name="metadata">An <see cref="AccountMetadata"/> instance containing the metadata associated with the
        /// account.</param>
        /// <returns>
        /// <para>The total number of objects in the account.</para>
        /// <para>-or-</para>
        /// <para><see langword="null"/> if <paramref name="metadata"/> does not contain a value for the
        /// <see cref="AccountObjectCount"/> header, or if the header value cannot be parsed as an integer.</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="metadata"/> is <see langword="null"/>.
        /// </exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/GET_showAccountDetails__v1__account__storage_account_services.html">Show account details and list containers (OpenStack Object Storage API V1 Reference)</seealso>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/GET_listcontainers_v1__account__accountServicesOperations_d1e000.html">Show Account Details and List Containers (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        public static long? GetObjectCount(this AccountMetadata metadata)
        {
            if (metadata == null)
                throw new ArgumentNullException("metadata");

            string rawValue;
            if (!metadata.Headers.TryGetValue(AccountObjectCount, out rawValue))
                return null;

            long value;
            if (!long.TryParse(rawValue, out value))
                return null;

            return value;
        }

        /// <summary>
        /// Gets the first of two secret keys which may be configured for an account.
        /// </summary>
        /// <param name="metadata">An <see cref="AccountMetadata"/> instance containing the metadata associated with the
        /// account.</param>
        /// <returns>
        /// <para>The first account secret key.</para>
        /// <para>-or-</para>
        /// <para><see langword="null"/> if <paramref name="metadata"/> does not contain a value for the
        /// <see cref="AccountSecretKey"/> metadata key.</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="metadata"/> is <see langword="null"/>.
        /// </exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/tempurl.html#account-secret-keys-temp-url">Account secret keys - Temporary URL middleware (OpenStack Object Storage API V1 Reference)</seealso>
        public static string GetSecretKey(this AccountMetadata metadata)
        {
            if (metadata == null)
                throw new ArgumentNullException("metadata");

            string rawValue;
            if (!metadata.Metadata.TryGetValue(AccountSecretKey, out rawValue))
                return null;

            return rawValue;
        }

        /// <summary>
        /// Gets the second of two secret keys which may be configured for an account.
        /// </summary>
        /// <param name="metadata">An <see cref="AccountMetadata"/> instance containing the metadata associated with the
        /// account.</param>
        /// <returns>
        /// <para>The second account secret key.</para>
        /// <para>-or-</para>
        /// <para><see langword="null"/> if <paramref name="metadata"/> does not contain a value for the
        /// <see cref="AccountSecretKey2"/> metadata key.</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="metadata"/> is <see langword="null"/>.
        /// </exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/tempurl.html#account-secret-keys-temp-url">Account secret keys - Temporary URL middleware (OpenStack Object Storage API V1 Reference)</seealso>
        public static string GetSecretKey2(this AccountMetadata metadata)
        {
            if (metadata == null)
                throw new ArgumentNullException("metadata");

            string rawValue;
            if (!metadata.Metadata.TryGetValue(AccountSecretKey2, out rawValue))
                return null;

            return rawValue;
        }

        /// <summary>
        /// Updates an <see cref="AccountMetadata"/> instance to include the specified first secret key.
        /// </summary>
        /// <remarks>
        /// <para>The input <paramref name="metadata"/> object is not changed. A new <see cref="AccountMetadata"/>
        /// instance is created and returned by this method.</para>
        /// </remarks>
        /// <param name="metadata">The account metadata to update.</param>
        /// <param name="key">
        /// <para>The first secret key for the account.</para>
        /// <para>-or-</para>
        /// <para><see langword="null"/> or the empty string to remove and disable the first secret key for the
        /// account.</para>
        /// </param>
        /// <returns>
        /// A new <see cref="AccountMetadata"/> instance which is updated to include the specified secret key.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="metadata"/> is <see langword="null"/>.
        /// </exception>
        /// <seealso cref="AccountSecretKey"/>
        public static AccountMetadata WithSecretKey(this AccountMetadata metadata, string key)
        {
            ImmutableDictionary<string, string> updatedMetadata = metadata.Metadata.SetItem(AccountSecretKey, key ?? string.Empty);
            return metadata.WithMetadata(updatedMetadata);
        }

        /// <summary>
        /// Updates an <see cref="AccountMetadata"/> instance to include the specified second secret key.
        /// </summary>
        /// <remarks>
        /// <para>The input <paramref name="metadata"/> object is not changed. A new <see cref="AccountMetadata"/>
        /// instance is created and returned by this method.</para>
        /// </remarks>
        /// <param name="metadata">The account metadata to update.</param>
        /// <param name="key">
        /// <para>The second secret key for the account.</para>
        /// <para>-or-</para>
        /// <para><see langword="null"/> or the empty string to remove and disable the second secret key for the
        /// account.</para>
        /// </param>
        /// <returns>
        /// A new <see cref="AccountMetadata"/> instance which is updated to include the specified secret key.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="metadata"/> is <see langword="null"/>.
        /// </exception>
        /// <seealso cref="AccountSecretKey"/>
        public static AccountMetadata WithSecretKey2(this AccountMetadata metadata, string key)
        {
            ImmutableDictionary<string, string> updatedMetadata = metadata.Metadata.SetItem(AccountSecretKey2, key ?? string.Empty);
            return metadata.WithMetadata(updatedMetadata);
        }
    }
}
