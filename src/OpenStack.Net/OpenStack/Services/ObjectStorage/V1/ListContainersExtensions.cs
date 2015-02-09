namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using System.Threading.Tasks;
    using OpenStack.Net;

    /// <summary>
    /// This class provides extension methods for specifying optional parameters for the List Containers API call.
    /// </summary>
    /// <seealso cref="ListContainersApiCall"/>
    /// <seealso cref="IObjectStorageService.PrepareListContainersAsync"/>
    /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/GET_showAccountDetails__v1__account__storage_account_services.html">Show account details and list containers (OpenStack Object Storage API V1 Reference)</seealso>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public static class ListContainersExtensions
    {
        /// <summary>
        /// Update an HTTP API call prepared by <see cref="IObjectStorageService.PrepareListContainersAsync"/> to
        /// include the <c>limit</c> query parameter, limiting the maximum number of items in the returned list of
        /// containers to a specified value.
        /// </summary>
        /// <param name="apiCall">The prepared HTTP API call.</param>
        /// <param name="pageSize">The maximum number of containers to return in a single page of the resulting API
        /// call.</param>
        /// <returns>The input argument <paramref name="apiCall"/>, which was modified according to the specified
        /// <paramref name="pageSize"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="apiCall"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/GET_showAccountDetails__v1__account__storage_account_services.html">Show account details and list containers (OpenStack Object Storage API V1 Reference)</seealso>
        public static ListContainersApiCall WithPageSize(this ListContainersApiCall apiCall, int pageSize)
        {
            if (apiCall == null)
                throw new ArgumentNullException("apiCall");

            return apiCall.WithQueryParameter("limit", pageSize.ToString());
        }

        /// <summary>
        /// Update an HTTP API call prepared by <see cref="IObjectStorageService.PrepareListContainersAsync"/> to
        /// include the <c>limit</c> query parameter, limiting the maximum number of items in the returned list of
        /// containers to a specified value.
        /// </summary>
        /// <param name="task">A <see cref="Task"/> representing an asynchronous operation to prepare a
        /// <see cref="ListContainersApiCall"/> HTTP API call.</param>
        /// <param name="pageSize">The maximum number of containers to return in a single page of the resulting API
        /// call.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property contains the result of the input <paramref name="task"/>,
        /// which was modified according to the specified <paramref name="pageSize"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="task"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/GET_showAccountDetails__v1__account__storage_account_services.html">Show account details and list containers (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task<ListContainersApiCall> WithPageSize(this Task<ListContainersApiCall> task, int pageSize)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            return task.WithQueryParameter("limit", pageSize.ToString());
        }

        /// <summary>
        /// Update an HTTP API call prepared by <see cref="IObjectStorageService.PrepareListContainersAsync"/>
        /// to include the <c>prefix</c> query parameter, filtering the resulting container listing to only
        /// include containers beginning with the specified prefix.
        /// </summary>
        /// <param name="apiCall">The prepared HTTP API call.</param>
        /// <param name="prefix">The container name prefix used to filter the listing.</param>
        /// <returns>The input argument <paramref name="apiCall"/>, which was modified according to the specified
        /// <paramref name="prefix"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="apiCall"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="prefix"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/GET_showAccountDetails__v1__account__storage_account_services.html">Show account details and list containers (OpenStack Object Storage API V1 Reference)</seealso>
        public static ListContainersApiCall WithPrefix(this ListContainersApiCall apiCall, ContainerName prefix)
        {
            if (apiCall == null)
                throw new ArgumentNullException("apiCall");
            if (prefix == null)
                throw new ArgumentNullException("prefix");

            return apiCall.WithQueryParameter("prefix", prefix.Value);
        }

        /// <summary>
        /// Update an HTTP API call prepared by <see cref="IObjectStorageService.PrepareListContainersAsync"/>
        /// to include the <c>prefix</c> query parameter, filtering the resulting container listing to only
        /// include containers beginning with the specified prefix.
        /// </summary>
        /// <param name="task">A <see cref="Task"/> representing an asynchronous operation to prepare a
        /// <see cref="ListContainersApiCall"/> HTTP API call.</param>
        /// <param name="prefix">The container name prefix used to filter the listing.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property contains the result of the input <paramref name="task"/>,
        /// which was modified according to the specified <paramref name="prefix"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="prefix"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/GET_showAccountDetails__v1__account__storage_account_services.html">Show account details and list containers (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task<ListContainersApiCall> WithPrefix(this Task<ListContainersApiCall> task, ContainerName prefix)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            if (prefix == null)
                throw new ArgumentNullException("prefix");

            return task.WithQueryParameter("prefix", prefix.Value);
        }

        /// <summary>
        /// Update an HTTP API call prepared by <see cref="IObjectStorageService.PrepareListContainersAsync"/> to
        /// include the <c>marker</c> query parameter, filtering the resulting container listing to only include
        /// containers appearing after the marker when sorted using a binary comparison of container names, regardless
        /// of encoding.
        /// </summary>
        /// <remarks>
        /// <para>This SDK always uses the UTF-8 encoding for container and object names, but containers and/or objects
        /// created through other means may use another encoding.</para>
        /// </remarks>
        /// <param name="apiCall">The prepared HTTP API call.</param>
        /// <param name="marker">The marker container name used to filter the listing.</param>
        /// <returns>The input argument <paramref name="apiCall"/>, which was modified according to the specified
        /// <paramref name="marker"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="apiCall"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="marker"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/GET_showAccountDetails__v1__account__storage_account_services.html">Show account details and list containers (OpenStack Object Storage API V1 Reference)</seealso>
        public static ListContainersApiCall WithMarker(this ListContainersApiCall apiCall, ContainerName marker)
        {
            if (apiCall == null)
                throw new ArgumentNullException("apiCall");
            if (marker == null)
                throw new ArgumentNullException("marker");

            return apiCall.WithQueryParameter("marker", marker.Value);
        }

        /// <summary>
        /// Update an HTTP API call prepared by <see cref="IObjectStorageService.PrepareListContainersAsync"/> to
        /// include the <c>marker</c> query parameter, filtering the resulting container listing to only include
        /// containers appearing after the marker when sorted using a binary comparison of container names, regardless
        /// of encoding.
        /// </summary>
        /// <remarks>
        /// <para>This SDK always uses the UTF-8 encoding for container and object names, but containers and/or objects
        /// created through other means may use another encoding.</para>
        /// </remarks>
        /// <param name="task">A <see cref="Task"/> representing an asynchronous operation to prepare a
        /// <see cref="ListContainersApiCall"/> HTTP API call.</param>
        /// <param name="marker">The marker container name used to filter the listing.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property contains the result of the input <paramref name="task"/>,
        /// which was modified according to the specified <paramref name="marker"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="marker"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/GET_showAccountDetails__v1__account__storage_account_services.html">Show account details and list containers (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task<ListContainersApiCall> WithMarker(this Task<ListContainersApiCall> task, ContainerName marker)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            if (marker == null)
                throw new ArgumentNullException("marker");

            return task.WithQueryParameter("marker", marker.Value);
        }

        /// <summary>
        /// Update an HTTP API call prepared by <see cref="IObjectStorageService.PrepareListContainersAsync"/> to
        /// include the <c>end_marker</c> query parameter, filtering the resulting container listing to only include
        /// containers appearing before the marker when sorted using a binary comparison of container names, regardless
        /// of encoding.
        /// </summary>
        /// <remarks>
        /// <para>This SDK always uses the UTF-8 encoding for container and object names, but containers and/or objects
        /// created through other means may use another encoding.</para>
        /// </remarks>
        /// <param name="apiCall">The prepared HTTP API call.</param>
        /// <param name="endMarker">The marker container name used to filter the listing.</param>
        /// <returns>The input argument <paramref name="apiCall"/>, which was modified according to the specified
        /// <paramref name="endMarker"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="apiCall"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="endMarker"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/GET_showAccountDetails__v1__account__storage_account_services.html">Show account details and list containers (OpenStack Object Storage API V1 Reference)</seealso>
        public static ListContainersApiCall WithEndMarker(this ListContainersApiCall apiCall, ContainerName endMarker)
        {
            if (apiCall == null)
                throw new ArgumentNullException("apiCall");
            if (endMarker == null)
                throw new ArgumentNullException("endMarker");

            return apiCall.WithQueryParameter("end_marker", endMarker.Value);
        }

        /// <summary>
        /// Update an HTTP API call prepared by <see cref="IObjectStorageService.PrepareListContainersAsync"/> to
        /// include the <c>end_marker</c> query parameter, filtering the resulting container listing to only include
        /// containers appearing before the marker when sorted using a binary comparison of container names, regardless
        /// of encoding.
        /// </summary>
        /// <remarks>
        /// <para>This SDK always uses the UTF-8 encoding for container and object names, but containers and/or objects
        /// created through other means may use another encoding.</para>
        /// </remarks>
        /// <param name="task">A <see cref="Task"/> representing an asynchronous operation to prepare a
        /// <see cref="ListContainersApiCall"/> HTTP API call.</param>
        /// <param name="endMarker">The marker container name used to filter the listing.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property contains the result of the input <paramref name="task"/>,
        /// which was modified according to the specified <paramref name="endMarker"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="task"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="endMarker"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/GET_showAccountDetails__v1__account__storage_account_services.html">Show account details and list containers (OpenStack Object Storage API V1 Reference)</seealso>
        public static Task<ListContainersApiCall> WithEndMarker(this Task<ListContainersApiCall> task, ContainerName endMarker)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            if (endMarker == null)
                throw new ArgumentNullException("endMarker");

            return task.WithQueryParameter("end_marker", endMarker.Value);
        }
    }
}
