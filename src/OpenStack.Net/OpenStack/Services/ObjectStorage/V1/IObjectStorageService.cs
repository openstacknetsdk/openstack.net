namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenStack.Net;
    using Stream = System.IO.Stream;

#if !NET40PLUS
    using Rackspace.Threading;
#endif

    /// <summary>
    /// This is the base interface for the OpenStack Object Storage Service V1.
    /// </summary>
    /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/">OpenStack Object Storage API V1 Reference</seealso>
    /// <preliminary/>
    public interface IObjectStorageService : IHttpService
    {
        #region Discoverability

        /// <summary>
        /// Prepare an HTTP API call to determine the features enabled in the Object Storage Service.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns><token>PrepareCallReturns</token></returns>
        /// <exception cref="HttpWebException">
        /// If an error occurs during an HTTP request as part of preparing the API call.
        /// </exception>
        /// <seealso cref="GetObjectStorageInfoApiCall"/>
        /// <seealso cref="ObjectStorageServiceExtensions.GetObjectStorageInfoAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/discoverability.html">Discoverability (OpenStack Object Storage API V1 Reference)</seealso>
        Task<GetObjectStorageInfoApiCall> PrepareGetObjectStorageInfoAsync(CancellationToken cancellationToken);

        #endregion

        #region Accounts

        /// <summary>
        /// Prepare an API call to list the containers present in the Object Storage Service.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns><token>PrepareCallReturns</token></returns>
        /// <exception cref="HttpWebException">
        /// If an error occurs during an HTTP request as part of preparing the API call.
        /// </exception>
        /// <seealso cref="ListContainersApiCall"/>
        /// <seealso cref="ObjectStorageServiceExtensions.ListContainersAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/GET_showAccountDetails__v1__account__storage_account_services.html">Show account details and list containers (OpenStack Object Storage API V1 Reference)</seealso>
        Task<ListContainersApiCall> PrepareListContainersAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Prepare an API call to get the metadata associated with the authenticated account in the Object Storage
        /// Service.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns><token>PrepareCallReturns</token></returns>
        /// <exception cref="HttpWebException">
        /// If an error occurs during an HTTP request as part of preparing the API call.
        /// </exception>
        /// <seealso cref="GetAccountMetadataApiCall"/>
        /// <seealso cref="ObjectStorageServiceExtensions.GetAccountMetadataAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/GET_showAccountDetails__v1__account__storage_account_services.html">Show account details and list containers (OpenStack Object Storage API V1 Reference)</seealso>
        Task<GetAccountMetadataApiCall> PrepareGetAccountMetadataAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Prepare an API call to update the metadata associated with the authenticated account in the Object Storage
        /// Service.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This API call sets the value of metadata items which are already associated with the account, and adds the
        /// items which are not already present. If an element in <paramref name="metadata"/> has an empty value, the
        /// metadata item with the corresponding key is removed. Account metadata items which do not match any of the
        /// keys in <paramref name="metadata"/> are left unchanged.
        /// </para>
        /// </remarks>
        /// <param name="metadata">The updated account metadata.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns><token>PrepareCallReturns</token></returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="metadata"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="FormatException">
        /// <para>If <paramref name="metadata"/> contains any headers or metadata keys which are not a valid
        /// <c>token</c> according to the HTTP/1.1 specification.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// <para>If <paramref name="metadata"/> contains any keys or values which are not supported by the Object
        /// Storage Service.</para>
        /// </exception>
        /// <exception cref="HttpWebException">
        /// If an error occurs during an HTTP request as part of preparing the API call.
        /// </exception>
        /// <seealso cref="UpdateAccountMetadataApiCall"/>
        /// <seealso cref="ObjectStorageServiceExtensions.UpdateAccountMetadataAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/POST_updateAccountMeta__v1__account__storage_account_services.html">Create, update, or delete account metadata (OpenStack Object Storage API V1 Reference)</seealso>
        Task<UpdateAccountMetadataApiCall> PrepareUpdateAccountMetadataAsync(AccountMetadata metadata, CancellationToken cancellationToken);

        /// <summary>
        /// Prepare an API call to remove particular metadata associated with the authenticated account in the Object
        /// Storage Service.
        /// </summary>
        /// <param name="keys">The keys of the metadata items to remove from the account.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns><token>PrepareCallReturns</token></returns>
        /// <exception cref="ArgumentNullException">If <paramref name="keys"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="keys"/> contains any <see langword="null"/> or
        /// empty values.</exception>
        /// <exception cref="HttpWebException">
        /// If an error occurs during an HTTP request as part of preparing the API call.
        /// </exception>
        /// <seealso cref="UpdateAccountMetadataApiCall"/>
        /// <seealso cref="ObjectStorageServiceExtensions.RemoveAccountMetadataAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/POST_updateAccountMeta__v1__account__storage_account_services.html">Create, update, or delete account metadata (OpenStack Object Storage API V1 Reference)</seealso>
        Task<UpdateAccountMetadataApiCall> PrepareRemoveAccountMetadataAsync(IEnumerable<string> keys, CancellationToken cancellationToken);

        #endregion

        #region Containers

        /// <summary>
        /// Prepare an API call to create a container in the Object Storage Service.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns><token>PrepareCallReturns</token></returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="container"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="HttpWebException">
        /// If an error occurs during an HTTP request as part of preparing the API call.
        /// </exception>
        /// <seealso cref="CreateContainerApiCall"/>
        /// <seealso cref="ObjectStorageServiceExtensions.CreateContainerAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/PUT_createContainer__v1__account___container__storage_container_services.html">Create container (OpenStack Object Storage API V1 Reference)</seealso>
        Task<CreateContainerApiCall> PrepareCreateContainerAsync(ContainerName container, CancellationToken cancellationToken);

        /// <summary>
        /// Prepare an API call to remove a container from the Object Storage Service.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns><token>PrepareCallReturns</token></returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="container"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="HttpWebException">
        /// If an error occurs during an HTTP request as part of preparing the API call.
        /// </exception>
        /// <seealso cref="RemoveContainerApiCall"/>
        /// <seealso cref="ObjectStorageServiceExtensions.RemoveContainerAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/DELETE_deleteContainer__v1__account___container__storage_container_services.html">Delete container (OpenStack Object Storage API V1 Reference)</seealso>
        Task<RemoveContainerApiCall> PrepareRemoveContainerAsync(ContainerName container, CancellationToken cancellationToken);

        /// <summary>
        /// Prepare an API call to list the objects present in a container in the Object Storage Service.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns><token>PrepareCallReturns</token></returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="container"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="HttpWebException">
        /// If an error occurs during an HTTP request as part of preparing the API call.
        /// </exception>
        /// <seealso cref="ListObjectsApiCall"/>
        /// <seealso cref="ObjectStorageServiceExtensions.ListObjectsAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/GET_showContainerDetails__v1__account___container__storage_container_services.html">Show container details and list objects (OpenStack Object Storage API V1 Reference)</seealso>
        Task<ListObjectsApiCall> PrepareListObjectsAsync(ContainerName container, CancellationToken cancellationToken);

        /// <summary>
        /// Prepare an API call to get the metadata associated with a container in the Object Storage Service.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns><token>PrepareCallReturns</token></returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="container"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="HttpWebException">
        /// If an error occurs during an HTTP request as part of preparing the API call.
        /// </exception>
        /// <seealso cref="GetContainerMetadataApiCall"/>
        /// <seealso cref="ObjectStorageServiceExtensions.GetContainerMetadataAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/HEAD_showContainerMeta__v1__account___container__storage_container_services.html">Show container metadata (OpenStack Object Storage API V1 Reference)</seealso>
        Task<GetContainerMetadataApiCall> PrepareGetContainerMetadataAsync(ContainerName container, CancellationToken cancellationToken);

        /// <summary>
        /// Prepare an API call to update the metadata associated with a container in the Object Storage Service.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This API call sets the value of metadata items which are already associated with the container, and adds the
        /// items which are not already present. If an element in <paramref name="metadata"/> has an empty value, the
        /// metadata item with the corresponding key is removed. Container metadata items which do not match any of the
        /// keys in <paramref name="metadata"/> are left unchanged.
        /// </para>
        /// </remarks>
        /// <param name="container">The name of the container to update.</param>
        /// <param name="metadata">The updated container metadata.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns><token>PrepareCallReturns</token></returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="metadata"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="FormatException">
        /// <para>If <paramref name="metadata"/> contains any headers or metadata keys which are not a valid
        /// <c>token</c> according to the HTTP/1.1 specification.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// <para>If <paramref name="metadata"/> contains any keys or values which are not supported by the Object
        /// Storage Service.</para>
        /// </exception>
        /// <exception cref="HttpWebException">
        /// If an error occurs during an HTTP request as part of preparing the API call.
        /// </exception>
        /// <seealso cref="UpdateContainerMetadataApiCall"/>
        /// <seealso cref="ObjectStorageServiceExtensions.UpdateContainerMetadataAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/POST_updateContainerMeta__v1__account___container__storage_container_services.html">Create, update, or delete container metadata (OpenStack Object Storage API V1 Reference)</seealso>
        Task<UpdateContainerMetadataApiCall> PrepareUpdateContainerMetadataAsync(ContainerName container, ContainerMetadata metadata, CancellationToken cancellationToken);

        /// <summary>
        /// Prepare an API call to remove particular metadata associated with a container in the Object Storage Service.
        /// </summary>
        /// <param name="container">The name of the container to update.</param>
        /// <param name="keys">The keys of the metadata items to remove from the container.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns><token>PrepareCallReturns</token></returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="keys"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="keys"/> contains any <see langword="null"/> or empty values.
        /// </exception>
        /// <exception cref="HttpWebException">
        /// If an error occurs during an HTTP request as part of preparing the API call.
        /// </exception>
        /// <seealso cref="UpdateContainerMetadataApiCall"/>
        /// <seealso cref="ObjectStorageServiceExtensions.RemoveContainerMetadataAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/POST_updateContainerMeta__v1__account___container__storage_container_services.html">Create, update, or delete container metadata (OpenStack Object Storage API V1 Reference)</seealso>
        Task<UpdateContainerMetadataApiCall> PrepareRemoveContainerMetadataAsync(ContainerName container, IEnumerable<string> keys, CancellationToken cancellationToken);

        #endregion

        #region Objects

        /// <summary>
        /// Prepare an API call to create an object in the Object Storage Service.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="object">The object name.</param>
        /// <param name="stream">A stream containing the object data.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <param name="progress">
        /// <para>An <see cref="IProgress{T}"/> instance receiving progress updates during the request.</para>
        /// <para>-or-</para>
        /// <para><see langword="null"/> if progress updates are not required.</para>
        /// </param>
        /// <returns><token>PrepareCallReturns</token></returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="object"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="stream"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">
        /// If an error occurs during an HTTP request as part of preparing the API call.
        /// </exception>
        /// <seealso cref="CreateObjectApiCall"/>
        /// <seealso cref="ObjectStorageServiceExtensions.CreateObjectAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/PUT_createOrReplaceObject__v1__account___container___object__storage_object_services.html">Create or replace object (OpenStack Object Storage API V1 Reference)</seealso>
        Task<CreateObjectApiCall> PrepareCreateObjectAsync(ContainerName container, ObjectName @object, Stream stream, CancellationToken cancellationToken, IProgress<long> progress);

        /// <summary>
        /// Prepare an API call to copy an object in the Object Storage Service.
        /// </summary>
        /// <param name="sourceContainer">The source container name.</param>
        /// <param name="sourceObject">The source object name.</param>
        /// <param name="destinationContainer">The destination container name.</param>
        /// <param name="destinationObject">The destination object name.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns><token>PrepareCallReturns</token></returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="sourceContainer"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="sourceObject"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="destinationContainer"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="destinationObject"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">
        /// If an error occurs during an HTTP request as part of preparing the API call.
        /// </exception>
        /// <seealso cref="CopyObjectApiCall"/>
        /// <seealso cref="ObjectStorageServiceExtensions.CopyObjectAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/COPY_copyObject__v1__account___container___object__storage_object_services.html">Copy object (OpenStack Object Storage API V1 Reference)</seealso>
        Task<CopyObjectApiCall> PrepareCopyObjectAsync(ContainerName sourceContainer, ObjectName sourceObject, ContainerName destinationContainer, ObjectName destinationObject, CancellationToken cancellationToken);

        /// <summary>
        /// Prepare an API call to remove an object from a container in the Object Storage Service.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="object">The object name.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns><token>PrepareCallReturns</token></returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="object"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">
        /// If an error occurs during an HTTP request as part of preparing the API call.
        /// </exception>
        /// <seealso cref="RemoveObjectApiCall"/>
        /// <seealso cref="ObjectStorageServiceExtensions.RemoveObjectAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/DELETE_deleteObject__v1__account___container___object__storage_object_services.html">Delete object (OpenStack Object Storage API V1 Reference)</seealso>
        Task<RemoveObjectApiCall> PrepareRemoveObjectAsync(ContainerName container, ObjectName @object, CancellationToken cancellationToken);

        /// <summary>
        /// Prepare an API call to get the content and metadata of an object in the Object Storage Service.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="object">The object name.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns><token>PrepareCallReturns</token></returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="object"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">
        /// If an error occurs during an HTTP request as part of preparing the API call.
        /// </exception>
        /// <seealso cref="GetObjectApiCall"/>
        /// <seealso cref="O:OpenStack.Services.ObjectStorage.V1.ObjectStorageServiceExtensions.GetObjectAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/GET_getObject__v1__account___container___object__storage_object_services.html">Get object content and metadata (OpenStack Object Storage API V1 Reference)</seealso>
        Task<GetObjectApiCall> PrepareGetObjectAsync(ContainerName container, ObjectName @object, CancellationToken cancellationToken);

        /// <summary>
        /// Prepare an API call to get the metadata associated with an object in the Object Storage Service.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="object">The object name.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns><token>PrepareCallReturns</token></returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="object"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">
        /// If an error occurs during an HTTP request as part of preparing the API call.
        /// </exception>
        /// <seealso cref="GetObjectMetadataApiCall"/>
        /// <seealso cref="ObjectStorageServiceExtensions.GetObjectMetadataAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/HEAD_showObjectMeta__v1__account___container___object__storage_object_services.html">Show object metadata (OpenStack Object Storage API V1 Reference)</seealso>
        Task<GetObjectMetadataApiCall> PrepareGetObjectMetadataAsync(ContainerName container, ObjectName @object, CancellationToken cancellationToken);

        /// <summary>
        /// Prepare an API call to set the metadata associated with an object in the Object Storage Service.
        /// <note type="warning">
        /// This method does not behave like other metadata methods in the Object Storage Service; the metadata
        /// associated with an object is updated all at once, including the values in both the
        /// <see cref="StorageMetadata.Headers"/> and <see cref="StorageMetadata.Metadata"/> collections.
        /// </note>
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="object">The object name.</param>
        /// <param name="metadata">
        /// The complete metadata to associate with the object, replacing all previous metadata.
        /// </param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns><token>PrepareCallReturns</token></returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="object"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="metadata"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="FormatException">
        /// <para>If <paramref name="metadata"/> contains any headers or metadata keys which are not a valid
        /// <c>token</c> according to the HTTP/1.1 specification.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// <para>If <paramref name="metadata"/> contains any keys or values which are not supported by the Object
        /// Storage Service.</para>
        /// </exception>
        /// <exception cref="HttpWebException">
        /// If an error occurs during an HTTP request as part of preparing the API call.
        /// </exception>
        /// <seealso cref="SetObjectMetadataApiCall"/>
        /// <seealso cref="ObjectStorageServiceExtensions.SetObjectMetadataAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/POST_updateObjectMeta__v1__account___container___object__storage_object_services.html">Create or update object metadata (OpenStack Object Storage API V1 Reference)</seealso>
        Task<SetObjectMetadataApiCall> PrepareSetObjectMetadataAsync(ContainerName container, ObjectName @object, ObjectMetadata metadata, CancellationToken cancellationToken);

        #endregion
    }
}
