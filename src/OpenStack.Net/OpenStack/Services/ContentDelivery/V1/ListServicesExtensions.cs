namespace OpenStack.Services.ContentDelivery.V1
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using OpenStack.Net;
    using Rackspace.Threading;

    /// <summary>
    /// This class provides extension methods for specifying additional parameters for the
    /// <see cref="ListServicesApiCall"/> HTTP API call.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public static class ListServicesExtensions
    {
        /// <summary>
        /// Sets (or removes) the <c>limit</c> query parameter for a <see cref="ListServicesApiCall"/> HTTP API call.
        /// </summary>
        /// <param name="apiCall">The prepared HTTP API call.</param>
        /// <param name="pageSize">
        /// <para>The maximum number of items to return in a single page of results.</para>
        /// <para>-or-</para>
        /// <para><see langword="null"/> to remove the <c>limit</c> query parameter and use the default page size
        /// configured by the vendor for the service.</para>
        /// </param>
        /// <returns>Returns the input argument <paramref name="apiCall"/>, which was modified according to the
        /// specified <paramref name="pageSize"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="apiCall"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="pageSize"/> is less than 0.</exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        public static ListServicesApiCall WithPageSize(this ListServicesApiCall apiCall, int? pageSize)
        {
            if (apiCall == null)
                throw new ArgumentNullException("apiCall");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize");

            Uri uri = apiCall.RequestMessage.RequestUri;
            if (!pageSize.HasValue)
                apiCall.RequestMessage.RequestUri = UriUtility.RemoveQueryParameter(uri, "limit");
            else
                apiCall.RequestMessage.RequestUri = UriUtility.SetQueryParameter(uri, "limit", pageSize.ToString());

            return apiCall;
        }

        /// <summary>
        /// Sets (or removes) the <c>marker</c> query parameter for a <see cref="ListServicesApiCall"/> HTTP API call.
        /// </summary>
        /// <param name="apiCall">The prepared HTTP API call.</param>
        /// <param name="serviceId">
        /// <para>The ID of the last <see cref="Service"/> in the previous page of results.</para>
        /// <para>-or-</para>
        /// <para><see langword="null"/> to remove the <c>marker</c> query parameter and have the resulting page start
        /// with the first item in the list.</para>
        /// </param>
        /// <returns>Returns the input argument <paramref name="apiCall"/>, which was modified according to the
        /// specified <paramref name="serviceId"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="apiCall"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        public static ListServicesApiCall WithMarker(this ListServicesApiCall apiCall, ServiceId serviceId)
        {
            if (apiCall == null)
                throw new ArgumentNullException("apiCall");
            if (serviceId == null)
                throw new ArgumentNullException("serviceId");

            Uri uri = apiCall.RequestMessage.RequestUri;
            if (serviceId == null)
                apiCall.RequestMessage.RequestUri = UriUtility.RemoveQueryParameter(uri, "marker");
            else
                apiCall.RequestMessage.RequestUri = UriUtility.SetQueryParameter(uri, "marker", serviceId.Value);

            return apiCall;
        }

        /// <summary>
        /// Sets (or removes) the <c>limit</c> query parameter for a <see cref="ListServicesApiCall"/> HTTP API call.
        /// </summary>
        /// <remarks>
        /// <para>This method simplifies the use of <see cref="WithPageSize(ListServicesApiCall, int?)"/> in scenarios
        /// where <see langword="async/await"/> are not used for the preparation of the HTTP API call.</para>
        /// </remarks>
        /// <param name="apiCall">A <see cref="Task"/> representing the asynchronous operation to prepare the HTTP API
        /// call.</param>
        /// <param name="pageSize">
        /// <para>The maximum number of items to return in a single page of results.</para>
        /// <para>-or-</para>
        /// <para><see langword="null"/> to remove the <c>limit</c> query parameter and use the default page size
        /// configured by the vendor for the service.</para>
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property contains the result of the input task
        /// <paramref name="apiCall"/>, which was modified according to the specified
        /// <paramref name="pageSize"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="apiCall"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="pageSize"/> is less than 0.</exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/Paginated_Collections-d1e325.html">Paginated collections (OpenStack Identity API v2.0 Reference)</seealso>
        public static Task<ListServicesApiCall> WithPageSize(this Task<ListServicesApiCall> apiCall, int? pageSize)
        {
            return apiCall.Select(task => apiCall.Result.WithPageSize(pageSize));
        }

        /// <summary>
        /// Sets (or removes) the <c>marker</c> query parameter for a <see cref="ListServicesApiCall"/> HTTP API call.
        /// </summary>
        /// <remarks>
        /// <para>This method simplifies the use of <see cref="WithMarker(ListServicesApiCall, ServiceId)"/> in
        /// scenarios where <see langword="async/await"/> are not used for the preparation of the HTTP API call.</para>
        /// </remarks>
        /// <param name="apiCall">A <see cref="Task"/> representing the asynchronous operation to prepare the HTTP API
        /// call.</param>
        /// <param name="serviceId">
        /// <para>The ID of the last <see cref="Service"/> in the previous page of results.</para>
        /// <para>-or-</para>
        /// <para><see langword="null"/> to remove the <c>marker</c> query parameter and have the resulting page start
        /// with the first item in the list.</para>
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. When the task completes successfully,
        /// the <see cref="Task{TResult}.Result"/> property contains the result of the input task
        /// <paramref name="apiCall"/>, which was modified according to the specified
        /// <paramref name="serviceId"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="apiCall"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ObjectDisposedException">If the HTTP API call has been disposed.</exception>
        /// <exception cref="InvalidOperationException">If the HTTP API call has already been sent.</exception>
        /// <seealso href="http://docs.openstack.org/api/openstack-identity-service/2.0/content/Paginated_Collections-d1e325.html">Paginated collections (OpenStack Identity API v2.0 Reference)</seealso>
        public static Task<ListServicesApiCall> WithMarker(this Task<ListServicesApiCall> apiCall, ServiceId serviceId)
        {
            return apiCall.Select(task => apiCall.Result.WithMarker(serviceId));
        }
    }
}
