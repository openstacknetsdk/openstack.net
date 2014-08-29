namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using System.Threading.Tasks;
    using OpenStack.Net;
    using Rackspace.Threading;

    /// <summary>
    /// This class contains extension methods to support the <c>X-Newest</c> header in various requests to the
    /// <see cref="IObjectStorageService"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public static class NewestExtensions
    {
        /// <summary>
        /// The name of the <c>X-Newest</c> header.
        /// </summary>
        public static readonly string Newest = "X-Newest";

        /// <summary>
        /// Updates the HTTP API call created by <see cref="IObjectStorageService.PrepareGetAccountMetadataAsync"/> to
        /// include the <c>X-Newest</c> header. The value of the header is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// <token>ObjectStorage_XNewest_Remarks</token>
        /// </remarks>
        /// <param name="apiCall">The prepared HTTP API call.</param>
        /// <returns>The input argument <paramref name="apiCall"/>, which was modified to include the <c>X-Newest</c>
        /// header.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="apiCall"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        public static GetAccountMetadataApiCall WithNewest(this GetAccountMetadataApiCall apiCall)
        {
            if (apiCall == null)
                throw new ArgumentNullException("apiCall");

            return apiCall.WithNewestImpl();
        }

        /// <summary>
        /// Updates the HTTP API call created by <see cref="IObjectStorageService.PrepareGetAccountMetadataAsync"/> to
        /// include the <c>X-Newest</c> header. The value of the header is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// <token>ObjectStorage_XNewest_Remarks</token>
        /// <para>This method simplifies the use of <see cref="WithNewest(GetAccountMetadataApiCall)"/> in scenarios
        /// where <see langword="async/await"/> are not used for the preparation of the HTTP API call.</para>
        /// </remarks>
        /// <param name="task">A <see cref="Task"/> representing the asynchronous operation to prepare the HTTP API
        /// call.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property contains the result of the input <paramref name="task"/>,
        /// which was modified to include the <c>X-Newest</c> header.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="task"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        public static Task<GetAccountMetadataApiCall> WithNewest(this Task<GetAccountMetadataApiCall> task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            return task.WithNewestImpl();
        }

        /// <summary>
        /// Updates the HTTP API call created by <see cref="IObjectStorageService.PrepareGetContainerMetadataAsync"/>
        /// to include the <c>X-Newest</c> header. The value of the header is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// <token>ObjectStorage_XNewest_Remarks</token>
        /// </remarks>
        /// <param name="apiCall">The prepared HTTP API call.</param>
        /// <returns>The input argument <paramref name="apiCall"/>, which was modified to include the <c>X-Newest</c>
        /// header.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="apiCall"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        public static GetContainerMetadataApiCall WithNewest(this GetContainerMetadataApiCall apiCall)
        {
            if (apiCall == null)
                throw new ArgumentNullException("task");

            return apiCall.WithNewestImpl();
        }

        /// <summary>
        /// Updates the HTTP API call created by <see cref="IObjectStorageService.PrepareGetContainerMetadataAsync"/>
        /// to include the <c>X-Newest</c> header. The value of the header is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// <token>ObjectStorage_XNewest_Remarks</token>
        /// <para>This method simplifies the use of <see cref="WithNewest(GetContainerMetadataApiCall)"/> in scenarios
        /// where <see langword="async/await"/> are not used for the preparation of the HTTP API call.</para>
        /// </remarks>
        /// <param name="task">A <see cref="Task"/> representing the asynchronous operation to prepare the HTTP API
        /// call.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation. When the task
        /// completes successfully, the <see cref="Task{TResult}.Result"/> property contains
        /// the modified API call.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="task"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        public static Task<GetContainerMetadataApiCall> WithNewest(this Task<GetContainerMetadataApiCall> task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            return task.WithNewestImpl();
        }

        /// <summary>
        /// Updates the HTTP API call created by <see cref="IObjectStorageService.PrepareGetObjectMetadataAsync"/>
        /// to include the <c>X-Newest</c> header. The value of the header is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// <token>ObjectStorage_XNewest_Remarks</token>
        /// </remarks>
        /// <param name="apiCall">The prepared HTTP API call.</param>
        /// <returns>The input argument <paramref name="apiCall"/>, which was modified to include the <c>X-Newest</c>
        /// header.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="apiCall"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        public static GetObjectMetadataApiCall WithNewest(this GetObjectMetadataApiCall apiCall)
        {
            if (apiCall == null)
                throw new ArgumentNullException("task");

            return apiCall.WithNewestImpl();
        }

        /// <summary>
        /// Updates the HTTP API call created by <see cref="IObjectStorageService.PrepareGetObjectMetadataAsync"/>
        /// to include the <c>X-Newest</c> header. The value of the header is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// <token>ObjectStorage_XNewest_Remarks</token>
        /// <para>This method simplifies the use of <see cref="WithNewest(GetObjectMetadataApiCall)"/> in scenarios
        /// where <see langword="async/await"/> are not used for the preparation of the HTTP API call.</para>
        /// </remarks>
        /// <param name="task">A <see cref="Task"/> representing the asynchronous operation to prepare the HTTP API
        /// call.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation. When the task
        /// completes successfully, the <see cref="Task{TResult}.Result"/> property contains
        /// the modified API call.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="task"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        public static Task<GetObjectMetadataApiCall> WithNewest(this Task<GetObjectMetadataApiCall> task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            return task.WithNewestImpl();
        }

        /// <summary>
        /// Updates the HTTP API call created by <see cref="IObjectStorageService.PrepareGetObjectAsync"/>
        /// to include the <c>X-Newest</c> header. The value of the header is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// <token>ObjectStorage_XNewest_Remarks</token>
        /// </remarks>
        /// <param name="apiCall">The prepared HTTP API call.</param>
        /// <returns>The input argument <paramref name="apiCall"/>, which was modified to include the <c>X-Newest</c>
        /// header.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="apiCall"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        public static GetObjectApiCall WithNewest(this GetObjectApiCall apiCall)
        {
            if (apiCall == null)
                throw new ArgumentNullException("task");

            return apiCall.WithNewestImpl();
        }

        /// <summary>
        /// Updates the HTTP API call created by <see cref="IObjectStorageService.PrepareGetObjectAsync"/>
        /// to include the <c>X-Newest</c> header. The value of the header is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// <token>ObjectStorage_XNewest_Remarks</token>
        /// <para>This method simplifies the use of <see cref="WithNewest(GetObjectApiCall)"/> in scenarios
        /// where <see langword="async/await"/> are not used for the preparation of the HTTP API call.</para>
        /// </remarks>
        /// <param name="task">A <see cref="Task"/> representing the asynchronous operation to prepare the HTTP API
        /// call.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation. When the task
        /// completes successfully, the <see cref="Task{TResult}.Result"/> property contains
        /// the modified API call.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="task"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        public static Task<GetObjectApiCall> WithNewest(this Task<GetObjectApiCall> task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            return task.WithNewestImpl();
        }

        /// <summary>
        /// Updates the HTTP API call created by <see cref="IObjectStorageService.PrepareListContainersAsync"/>
        /// to include the <c>X-Newest</c> header. The value of the header is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// <token>ObjectStorage_XNewest_Remarks</token>
        /// </remarks>
        /// <param name="apiCall">The prepared HTTP API call.</param>
        /// <returns>The input argument <paramref name="apiCall"/>, which was modified to include the <c>X-Newest</c>
        /// header.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="apiCall"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        public static ListContainersApiCall WithNewest(this ListContainersApiCall apiCall)
        {
            if (apiCall == null)
                throw new ArgumentNullException("task");

            return apiCall.WithNewestImpl();
        }

        /// <summary>
        /// Updates the HTTP API call created by <see cref="IObjectStorageService.PrepareListContainersAsync"/>
        /// to include the <c>X-Newest</c> header. The value of the header is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// <token>ObjectStorage_XNewest_Remarks</token>
        /// <para>This method simplifies the use of <see cref="WithNewest(ListContainersApiCall)"/> in scenarios
        /// where <see langword="async/await"/> are not used for the preparation of the HTTP API call.</para>
        /// </remarks>
        /// <param name="task">A <see cref="Task"/> representing the asynchronous operation to prepare the HTTP API
        /// call.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation. When the task
        /// completes successfully, the <see cref="Task{TResult}.Result"/> property contains
        /// the modified API call.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="task"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        public static Task<ListContainersApiCall> WithNewest(this Task<ListContainersApiCall> task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            return task.WithNewestImpl();
        }

        /// <summary>
        /// Updates the HTTP API call created by <see cref="IObjectStorageService.PrepareListObjectsAsync"/>
        /// to include the <c>X-Newest</c> header. The value of the header is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// <token>ObjectStorage_XNewest_Remarks</token>
        /// </remarks>
        /// <param name="apiCall">The prepared HTTP API call.</param>
        /// <returns>The input argument <paramref name="apiCall"/>, which was modified to include the <c>X-Newest</c>
        /// header.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="apiCall"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        public static ListObjectsApiCall WithNewest(this ListObjectsApiCall apiCall)
        {
            if (apiCall == null)
                throw new ArgumentNullException("task");

            return apiCall.WithNewestImpl();
        }

        /// <summary>
        /// Updates the HTTP API call created by <see cref="IObjectStorageService.PrepareListObjectsAsync"/>
        /// to include the <c>X-Newest</c> header. The value of the header is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// <token>ObjectStorage_XNewest_Remarks</token>
        /// <para>This method simplifies the use of <see cref="WithNewest(ListObjectsApiCall)"/> in scenarios
        /// where <see langword="async/await"/> are not used for the preparation of the HTTP API call.</para>
        /// </remarks>
        /// <param name="task">A <see cref="Task"/> representing the asynchronous operation to prepare the HTTP API
        /// call.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation. When the task
        /// completes successfully, the <see cref="Task{TResult}.Result"/> property contains
        /// the modified API call.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="task"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        public static Task<ListObjectsApiCall> WithNewest(this Task<ListObjectsApiCall> task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            return task.WithNewestImpl();
        }

        /// <summary>
        /// Updates an arbitrary <see cref="IHttpApiRequest"/> to include the <c>X-Newest</c> header. The value of the
        /// header is <c>true</c>.
        /// </summary>
        /// <param name="apiCall">The prepared HTTP API call.</param>
        /// <returns>The input argument <paramref name="apiCall"/>, which was modified to include the <c>X-Newest</c>
        /// header.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="apiCall"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        private static TCall WithNewestImpl<TCall>(this TCall apiCall)
            where TCall : IHttpApiRequest
        {
            if (apiCall == null)
                throw new ArgumentNullException("apiCall");

            apiCall.RequestMessage.Headers.Remove(Newest);
            apiCall.RequestMessage.Headers.Add(Newest, "true");
            return apiCall;
        }

        /// <summary>
        /// Updates an arbitrary <see cref="IHttpApiRequest"/> to include the <c>X-Newest</c> header.
        /// The value of the header is <c>true</c>.
        /// </summary>
        /// <param name="task">A <see cref="Task"/> representing the asynchronous operation to prepare an HTTP API call.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation. When the task
        /// completes successfully, the <see cref="Task{TResult}.Result"/> property contains
        /// the modified API call.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="task"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        private static Task<TCall> WithNewestImpl<TCall>(this Task<TCall> task)
            where TCall : IHttpApiRequest
        {
            if (task == null)
                throw new ArgumentNullException("task");

            return task.Select(innerTask => innerTask.Result.WithNewestImpl());
        }
    }
}
