namespace OpenStack.Services.ObjectStorage.V1
{
    using System;

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
    }
}
