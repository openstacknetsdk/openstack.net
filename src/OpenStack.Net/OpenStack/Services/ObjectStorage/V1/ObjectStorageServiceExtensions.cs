// Intentionally placed in the outer scope so IProgress<T> resolves to System.IProgress<T> instead of
// Rackspace.Threading.IProgress<T> in the .NET 4.0 build.
using Rackspace.Threading;

namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using OpenStack.Collections;
    using OpenStack.Net;
    using Stream = System.IO.Stream;

    /// <summary>
    /// This class provides extension methods that simplify the process of preparing and sending Object Storage Service
    /// HTTP API calls for the most common use cases.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public static class ObjectStorageServiceExtensions
    {
        #region Discoverability

        /// <summary>
        /// Determine the features enabled in the Object Storage service.
        /// </summary>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// <para>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property returns a dictionary; the keys are the names of properties,
        /// and the values are JSON objects containing arbitrary data associated with the properties.</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="service"/> is <see langword="null"/>.</exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso cref="GetObjectStorageInfoApiCall"/>
        /// <seealso cref="IObjectStorageService.PrepareGetObjectStorageInfoAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/discoverability.html">Discoverability (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task<ReadOnlyDictionary<string, JToken>> GetObjectStorageInfoAsync(this IObjectStorageService service, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareGetObjectStorageInfoAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        #endregion

        #region Accounts

        /// <summary>
        /// Prepare and send an HTTP API call to list the containers present in the Object Storage Service.
        /// </summary>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// <para>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property returns a tuple; the first item is the metadata associated
        /// with the account, and the second item is the first page of the requested container listing.</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="service"/> is <see langword="null"/>.</exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso cref="ListContainersApiCall"/>
        /// <seealso cref="IObjectStorageService.PrepareListContainersAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/GET_showAccountDetails__v1__account__storage_account_services.html">Show account details and list containers (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task<Tuple<AccountMetadata, ReadOnlyCollectionPage<Container>>> ListContainersAsync(this IObjectStorageService service, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareListContainersAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        /// <summary>
        /// Prepare and send an API call to get the metadata associated with the authenticated account in the Object
        /// Storage Service.
        /// </summary>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// <para>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property contains an <see cref="AccountMetadata"/> object containing
        /// the metadata associated with the account.</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="service"/> is <see langword="null"/>.</exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso cref="GetAccountMetadataApiCall"/>
        /// <seealso cref="IObjectStorageService.PrepareGetAccountMetadataAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/GET_showAccountDetails__v1__account__storage_account_services.html">Show account details and list containers (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task<AccountMetadata> GetAccountMetadataAsync(this IObjectStorageService service, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareGetAccountMetadataAsync(cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        /// <summary>
        /// Prepare and send an API call to update the metadata associated with the authenticated account in the Object
        /// Storage Service.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This API call sets the value of metadata items which are already associated with the account, and adds the
        /// items which are not already present. If an element in <paramref name="metadata"/> has an empty value, the
        /// metadata item with the corresponding key is removed. Account metadata items which do not match any of the
        /// keys in <paramref name="metadata"/> are left unchanged.
        /// </para>
        /// </remarks>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="metadata">The updated account metadata.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="metadata"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso cref="UpdateAccountMetadataApiCall"/>
        /// <seealso cref="IObjectStorageService.PrepareUpdateAccountMetadataAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/POST_updateAccountMeta__v1__account__storage_account_services.html">Create, update, or delete account metadata (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task UpdateAccountMetadataAsync(this IObjectStorageService service, AccountMetadata metadata, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareUpdateAccountMetadataAsync(metadata, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        /// <summary>
        /// Prepare and send an API call to remove particular metadata associated with the authenticated account in the
        /// Object Storage Service.
        /// </summary>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="keys">The keys of the metadata items to remove from the account.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="keys"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">If <paramref name="keys"/> contains any <see langword="null"/> or
        /// empty values.</exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso cref="UpdateAccountMetadataApiCall"/>
        /// <seealso cref="IObjectStorageService.PrepareRemoveAccountMetadataAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/POST_updateAccountMeta__v1__account__storage_account_services.html">Create, update, or delete account metadata (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task RemoveAccountMetadataAsync(this IObjectStorageService service, IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareRemoveAccountMetadataAsync(keys, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        #endregion

        #region Containers

        /// <summary>
        /// Prepare and send an API call to create a container in the Object Storage Service.
        /// </summary>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="container">The container name.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso cref="CreateContainerApiCall"/>
        /// <seealso cref="IObjectStorageService.PrepareCreateContainerAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/PUT_createContainer__v1__account___container__storage_container_services.html">Create container (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task CreateContainerAsync(this IObjectStorageService service, ContainerName container, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareCreateContainerAsync(container, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        /// <summary>
        /// Prepare and send an API call to remove a container from the Object Storage Service.
        /// </summary>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="container">The container name.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso cref="RemoveContainerApiCall"/>
        /// <seealso cref="IObjectStorageService.PrepareRemoveContainerAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/DELETE_deleteContainer__v1__account___container__storage_container_services.html">Delete container (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task RemoveContainerAsync(this IObjectStorageService service, ContainerName container, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareRemoveContainerAsync(container, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        /// <summary>
        /// Prepare and send an API call to list the objects present in a container in the Object Storage Service.
        /// </summary>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="container">The container name.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// <para>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property returns a tuple; the first item is the metadata associated
        /// with the container, and the second item is the first page of the requested objects listing.</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso cref="ListObjectsApiCall"/>
        /// <seealso cref="IObjectStorageService.PrepareListObjectsAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/GET_showContainerDetails__v1__account___container__storage_container_services.html">Show container details and list objects (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task<Tuple<ContainerMetadata, ReadOnlyCollectionPage<ContainerObject>>> ListObjectsAsync(this IObjectStorageService service, ContainerName container, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareListObjectsAsync(container, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        /// <summary>
        /// Prepare and send an API call to get the metadata associated with a container in the Object Storage Service.
        /// </summary>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="container">The container name.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// <para>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property contains a <see cref="ContainerMetadata"/> object containing
        /// the metadata associated with the container.</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso cref="GetContainerMetadataApiCall"/>
        /// <seealso cref="IObjectStorageService.PrepareGetContainerMetadataAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/HEAD_showContainerMeta__v1__account___container__storage_container_services.html">Show container metadata (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task<ContainerMetadata> GetContainerMetadataAsync(this IObjectStorageService service, ContainerName container, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareGetContainerMetadataAsync(container, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        /// <summary>
        /// Prepare and send an API call to update the metadata associated with a container in the Object Storage
        /// Service.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This API call sets the value of metadata items which are already associated with the container, and adds the
        /// items which are not already present. If an element in <paramref name="metadata"/> has an empty value, the
        /// metadata item with the corresponding key is removed. Container metadata items which do not match any of the
        /// keys in <paramref name="metadata"/> are left unchanged.
        /// </para>
        /// </remarks>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="container">The name of the container to update.</param>
        /// <param name="metadata">The updated container metadata.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="metadata"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso cref="UpdateContainerMetadataApiCall"/>
        /// <seealso cref="IObjectStorageService.PrepareUpdateContainerMetadataAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/POST_updateContainerMeta__v1__account___container__storage_container_services.html">Create, update, or delete container metadata (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task UpdateContainerMetadataAsync(this IObjectStorageService service, ContainerName container, ContainerMetadata metadata, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareUpdateContainerMetadataAsync(container, metadata, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        /// <summary>
        /// Prepare and send an API call to remove particular metadata associated with a container in the Object Storage
        /// Service.
        /// </summary>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="container">The name of the container to update.</param>
        /// <param name="keys">The keys of the metadata items to remove from the container.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="keys"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="keys"/> contains any <see langword="null"/> or empty values.
        /// </exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso cref="UpdateContainerMetadataApiCall"/>
        /// <seealso cref="IObjectStorageService.PrepareRemoveContainerMetadataAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/POST_updateContainerMeta__v1__account___container__storage_container_services.html">Create, update, or delete container metadata (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task RemoveContainerMetadataAsync(this IObjectStorageService service, ContainerName container, IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareRemoveContainerMetadataAsync(container, keys, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        #endregion

        #region Objects

        /// <summary>
        /// Prepare and send an API call to create an object in the Object Storage Service.
        /// </summary>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="container">The container name.</param>
        /// <param name="object">The object name.</param>
        /// <param name="stream">A stream containing the object data.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <param name="progress">
        /// <para>An <see cref="IProgress{T}"/> instance receiving progress updates during the request.</para>
        /// <para>-or-</para>
        /// <para><see langword="null"/> if progress updates are not required.</para>
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="object"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="stream"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso cref="CreateObjectApiCall"/>
        /// <seealso cref="IObjectStorageService.PrepareCreateObjectAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/PUT_createOrReplaceObject__v1__account___container___object__storage_object_services.html">Create or replace object (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task CreateObjectAsync(this IObjectStorageService service, ContainerName container, ObjectName @object, Stream stream, CancellationToken cancellationToken, IProgress<long> progress)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareCreateObjectAsync(container, @object, stream, cancellationToken, progress),
                task => task.Result.SendAsync(cancellationToken));
        }

        /// <summary>
        /// Prepare and send an API call to copy an object in the Object Storage Service.
        /// </summary>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="sourceContainer">The source container name.</param>
        /// <param name="sourceObject">The source object name.</param>
        /// <param name="destinationContainer">The destination container name.</param>
        /// <param name="destinationObject">The destination object name.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="sourceContainer"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="sourceObject"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="destinationContainer"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="destinationObject"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso cref="CopyObjectApiCall"/>
        /// <seealso cref="IObjectStorageService.PrepareCopyObjectAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/COPY_copyObject__v1__account___container___object__storage_object_services.html">Copy object (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task CopyObjectAsync(this IObjectStorageService service, ContainerName sourceContainer, ObjectName sourceObject, ContainerName destinationContainer, ObjectName destinationObject, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareCopyObjectAsync(sourceContainer, sourceObject, destinationContainer, destinationObject, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        /// <summary>
        /// Move and/or rename an object in the Object Storage Service.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method implements the move operation by first copying the object to the new location, followed by
        /// removing the original object. The remove operation is only attempted if the copy operation succeeded.
        /// </para>
        /// </remarks>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="sourceContainer">The source container name.</param>
        /// <param name="sourceObject">The source object name.</param>
        /// <param name="destinationContainer">The destination container name.</param>
        /// <param name="destinationObject">The destination object name.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="sourceContainer"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="sourceObject"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="destinationContainer"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="destinationObject"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso cref="CopyObjectAsync"/>
        /// <seealso cref="RemoveObjectAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/COPY_copyObject__v1__account___container___object__storage_object_services.html">Copy object (OpenStack Object Storage API V1 Reference)</seealso>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/DELETE_deleteObject__v1__account___container___object__storage_object_services.html">Delete object (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task MoveObjectAsync(this IObjectStorageService service, ContainerName sourceContainer, ObjectName sourceObject, ContainerName destinationContainer, ObjectName destinationObject, CancellationToken cancellationToken)
        {
            return service.CopyObjectAsync(sourceContainer, sourceObject, destinationContainer, destinationObject, cancellationToken)
                .Then(task => service.RemoveObjectAsync(sourceContainer, sourceObject, cancellationToken));
        }

        /// <summary>
        /// Prepare and send an API call to remove an object from a container in the Object Storage Service.
        /// </summary>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="container">The container name.</param>
        /// <param name="object">The object name.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="object"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso cref="RemoveObjectApiCall"/>
        /// <seealso cref="IObjectStorageService.PrepareRemoveObjectAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/DELETE_deleteObject__v1__account___container___object__storage_object_services.html">Delete object (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task RemoveObjectAsync(this IObjectStorageService service, ContainerName container, ObjectName @object, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareRemoveObjectAsync(container, @object, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        /// <summary>
        /// Prepare and send an API call to get the content and metadata of an object in the Object Storage Service.
        /// </summary>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="container">The container name.</param>
        /// <param name="object">The object name.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// <para>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property returns a tuple; the first item is the metadata associated
        /// with the object, and the second item is a <see cref="Stream"/> providing read access to the requested object
        /// data.</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="object"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso cref="GetObjectApiCall"/>
        /// <seealso cref="IObjectStorageService.PrepareGetObjectAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/GET_getObject__v1__account___container___object__storage_object_services.html">Get object content and metadata (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task<Tuple<ObjectMetadata, Stream>> GetObjectAsync(this IObjectStorageService service, ContainerName container, ObjectName @object, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareGetObjectAsync(container, @object, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        /// <summary>
        /// Prepare and send an API call to get the content and metadata of an object in the Object Storage Service.
        /// </summary>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="container">The container name.</param>
        /// <param name="object">The object name.</param>
        /// <param name="rangeHeader">A <see cref="RangeHeaderValue"/> specifying the range of data to read from the
        /// object.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// <para>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property returns a tuple; the first item is the metadata associated
        /// with the object, and the second item is a <see cref="Stream"/> providing read access to the requested object
        /// data.</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="object"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="rangeHeader"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso cref="GetObjectApiCall"/>
        /// <seealso cref="IObjectStorageService.PrepareGetObjectAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/GET_getObject__v1__account___container___object__storage_object_services.html">Get object content and metadata (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task<Tuple<ObjectMetadata, Stream>> GetObjectAsync(this IObjectStorageService service, ContainerName container, ObjectName @object, RangeHeaderValue rangeHeader, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            if (rangeHeader == null)
                throw new ArgumentNullException("rangeHeader");

            return TaskBlocks.Using(
                () => service.PrepareGetObjectAsync(container, @object, cancellationToken),
                task =>
                {
                    task.Result.RequestMessage.Headers.Range = rangeHeader;
                    return task.Result.SendAsync(cancellationToken)
                        .Select(innerTask => innerTask.Result.Item2);
                });
        }

        /// <summary>
        /// Prepare and send an API call to get the metadata associated with an object in the Object Storage Service.
        /// </summary>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="container">The container name.</param>
        /// <param name="object">The object name.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// <para>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property contains an <see cref="ObjectMetadata"/> object containing
        /// the metadata associated with the object.</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="object"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso cref="GetObjectMetadataApiCall"/>
        /// <seealso cref="IObjectStorageService.PrepareGetObjectMetadataAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/HEAD_showObjectMeta__v1__account___container___object__storage_object_services.html">Show object metadata (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task<ObjectMetadata> GetObjectMetadataAsync(this IObjectStorageService service, ContainerName container, ObjectName @object, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareGetObjectMetadataAsync(container, @object, cancellationToken),
                task => task.Result.SendAsync(cancellationToken).Select(innerTask => innerTask.Result.Item2));
        }

        /// <summary>
        /// Prepare and send an API call to set the metadata associated with an object in the Object Storage Service.
        /// <note type="warning">
        /// This method does not behave like other metadata methods in the Object Storage Service; the metadata
        /// associated with an object is updated all at once, including the values in both the
        /// <see cref="StorageMetadata.Headers"/> and <see cref="StorageMetadata.Metadata"/> collections.
        /// </note>
        /// </summary>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="container">The container name.</param>
        /// <param name="object">The object name.</param>
        /// <param name="metadata">
        /// The complete metadata to associate with the object, replacing all previous metadata.
        /// </param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="object"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="metadata"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso cref="SetObjectMetadataApiCall"/>
        /// <seealso cref="IObjectStorageService.PrepareSetObjectMetadataAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/POST_updateObjectMeta__v1__account___container___object__storage_object_services.html">Create or update object metadata (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task SetObjectMetadataAsync(this IObjectStorageService service, ContainerName container, ObjectName @object, ObjectMetadata metadata, CancellationToken cancellationToken)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return TaskBlocks.Using(
                () => service.PrepareSetObjectMetadataAsync(container, @object, metadata, cancellationToken),
                task => task.Result.SendAsync(cancellationToken));
        }

        /// <summary>
        /// Update the metadata associated with an object in the Object Storage Service.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This API call sets the value of metadata items which are already associated with the object, and adds the
        /// items which are not already present. If an element in <paramref name="metadata"/> has an empty value, the
        /// metadata item with the corresponding key is removed. Object metadata items which do not match any of the
        /// keys in <paramref name="metadata"/> are left unchanged.
        /// </para>
        /// <para>
        /// This method provides behavior similar to <see cref="UpdateContainerMetadataAsync"/> by first calling
        /// <see cref="GetObjectMetadataAsync"/> to retrieve the existing metadata associated with the object, then
        /// merging the existing metadata with the updated <paramref name="metadata"/> passed to this method, and
        /// finally calling <see cref="SetObjectMetadataAsync"/> to apply the changes.
        /// </para>
        /// <note type="important">
        /// <para>
        /// As a side effect of the eventual consistency model used by the Object Storage Service, this method may not
        /// behave as one might expect in common scenarios. Specifically, this method will merge the input
        /// <paramref name="metadata"/> with <em>some</em> copy of the metadata from the object, but not necessarily the
        /// most recent copy of the metadata. In addition, the updated metadata resulting from this call may not be
        /// immediately available through calls such as <see cref="GetObjectMetadataAsync"/> or
        /// <see cref="O:OpenStack.Services.ObjectStorage.V1.ObjectStorageServiceExtensions.GetObjectAsync"/>.
        /// </para>
        /// </note>
        /// </remarks>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="container">The container name.</param>
        /// <param name="object">The object name.</param>
        /// <param name="metadata">The updated object metadata.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="object"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="metadata"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso cref="ObjectStorageServiceExtensions.GetObjectMetadataAsync"/>
        /// <seealso cref="ObjectStorageServiceExtensions.SetObjectMetadataAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/POST_updateObjectMeta__v1__account___container___object__storage_object_services.html">Create or update object metadata (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task UpdateObjectMetadataAsync(this IObjectStorageService service, ContainerName container, ObjectName @object, ObjectMetadata metadata, CancellationToken cancellationToken)
        {
            return GetObjectMetadataAsync(service, container, @object, cancellationToken)
                .Then(task => SetObjectMetadataAsync(service, container, @object, MergeMetadata(task.Result, metadata), cancellationToken));
        }

        /// <summary>
        /// Remove particular metadata associated with an object in the Object Storage Service.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method provides behavior similar to <see cref="RemoveContainerMetadataAsync"/> by first calling
        /// <see cref="GetObjectMetadataAsync"/> to retrieve the existing metadata associated with the object, then
        /// removing the specified metadata <paramref name="keys"/> from the result, and finally calling
        /// <see cref="SetObjectMetadataAsync"/> to apply the changes.
        /// </para>
        /// <note type="important">
        /// <para>
        /// As a side effect of the eventual consistency model used by the Object Storage Service, this method may not
        /// behave as one might expect in common scenarios. Specifically, this method will remove the specified metadata
        /// keys from <em>some</em> copy of the metadata from the object, but not necessarily the most recent copy of
        /// the metadata. The result of this operation will completely replace the metadata for the object, whether or
        /// not the most recent copy was accessed. In addition, the updated metadata resulting from this call may not be
        /// immediately available through calls such as <see cref="GetObjectMetadataAsync"/> or
        /// <see cref="O:OpenStack.Services.ObjectStorage.V1.ObjectStorageServiceExtensions.GetObjectAsync"/>.
        /// </para>
        /// </note>
        /// </remarks>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="container">The container name.</param>
        /// <param name="object">The object name.</param>
        /// <param name="keys">The keys of the metadata items to remove from the object.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="container"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="object"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="keys"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="keys"/> contains any <see langword="null"/> or empty values.
        /// </exception>
        /// <exception cref="HttpWebException">If an HTTP API call failed during the operation.</exception>
        /// <seealso cref="ObjectStorageServiceExtensions.GetObjectMetadataAsync"/>
        /// <seealso cref="ObjectStorageServiceExtensions.SetObjectMetadataAsync"/>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/POST_updateObjectMeta__v1__account___container___object__storage_object_services.html">Create or update object metadata (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task RemoveObjectMetadataAsync(this IObjectStorageService service, ContainerName container, ObjectName @object, IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            ObjectMetadata updatedMetadata = new ObjectMetadata(new Dictionary<string, string>(), keys.ToDictionary(i => i, i => string.Empty));
            return UpdateObjectMetadataAsync(service, container, @object, updatedMetadata, cancellationToken);
        }

        #endregion

        /// <inheritdoc cref="MergeMetadata(IDictionary{string, string}, IDictionary{string, string})"/>
        private static ObjectMetadata MergeMetadata(ObjectMetadata originalMetadata, ObjectMetadata updatedMetadata)
        {
            if (originalMetadata == null)
                throw new ArgumentNullException("originalMetadata");
            if (updatedMetadata == null)
                throw new ArgumentNullException("updatedMetadata");

            Dictionary<string, string> headers = MergeMetadata(originalMetadata.Headers, updatedMetadata.Headers);
            Dictionary<string, string> metadata = MergeMetadata(originalMetadata.Metadata, updatedMetadata.Metadata);
            return new ObjectMetadata(headers, metadata);
        }

        /// <summary>
        /// Apply the changes specified in <paramref name="updatedMetadata"/> to the original metadata specified in
        /// <paramref name="originalMetadata"/>, and return the resulting complete representation of the metadata.
        /// </summary>
        /// <remarks>
        /// <para>This method provides the update operation required by <see cref="UpdateObjectMetadataAsync"/>.</para>
        /// <para>
        /// This method sets the value of metadata items which are already included in
        /// <paramref name="originalMetadata"/>, and adds the items which are not already present. If an element in
        /// <paramref name="updatedMetadata"/> has an empty value, the metadata item with the corresponding key is
        /// removed. Metadata items in <paramref name="originalMetadata"/> which do not match any of the keys in
        /// <paramref name="updatedMetadata"/> are left unchanged.
        /// </para>
        /// <note type="note">
        /// <para>The <paramref name="originalMetadata"/> argument is not directly modified. The results are returned in
        /// a new object which is a copy of <paramref name="originalMetadata"/> with the indicated changes
        /// applied.</para>
        /// </note>
        /// </remarks>
        /// <param name="originalMetadata">The original metadata.</param>
        /// <param name="updatedMetadata">The updated metadata to apply as a patch to the original metadata.</param>
        /// <returns>
        /// The transformed metadata.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="originalMetadata"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="updatedMetadata"/> is <see langword="null"/>.</para>
        /// </exception>
        private static Dictionary<string, string> MergeMetadata(IDictionary<string, string> originalMetadata, IDictionary<string, string> updatedMetadata)
        {
            if (originalMetadata == null)
                throw new ArgumentNullException("originalMetadata");
            if (updatedMetadata == null)
                throw new ArgumentNullException("updatedMetadata");

            Dictionary<string, string> result = new Dictionary<string, string>(originalMetadata, StringComparer.OrdinalIgnoreCase);
            foreach (var pair in updatedMetadata)
                result[pair.Key] = pair.Value;

            return result;
        }
    }
}
