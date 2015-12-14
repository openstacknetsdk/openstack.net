using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using OpenStack.Serialization;

namespace OpenStack
{
    /// <summary />
    public static class ApiHelper
    {
        /// <summary>
        /// Waits for the server to reach the specified status.
        /// </summary>
        /// <param name="resourceId">The resource identifier.</param>
        /// <param name="status">The status to wait for.</param>
        /// <param name="getResource">Function which retrieves the resource.</param>
        /// <param name="refreshDelay">The amount of time to wait between requests.</param>
        /// <param name="timeout">The amount of time to wait before throwing a <see cref="TimeoutException"/>.</param>
        /// <param name="progress">The progress callback.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="TimeoutException">If the <paramref name="timeout"/> value is reached.</exception>
        /// <exception cref="FlurlHttpException">If the API call returns a bad <see cref="HttpStatusCode"/>.</exception>
        public static async Task<TResource> WaitForStatusAsync<TResource, TStatus>(string resourceId, TStatus status, Func<Task<TResource>> getResource, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
            where TStatus : StringEnumeration
        {
            if (string.IsNullOrEmpty(resourceId))
                throw new ArgumentNullException("resourceId");
            
            refreshDelay = refreshDelay ?? TimeSpan.FromSeconds(5);
            timeout = timeout ?? TimeSpan.FromMinutes(5);

            using (var timeoutSource = new CancellationTokenSource(timeout.Value))
            using (var rootCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutSource.Token))
            {
                while (true)
                {
                    dynamic resource = await getResource().ConfigureAwait(false);
                    if (resource.Status?.IsError == true)
                        throw new ResourceErrorException($"The resource ({resourceId}) is in an error state ({resource.Status})");

                    bool complete = resource.Status == status;

                    progress?.Report(complete);

                    if (complete)
                        return resource;

                    try
                    {
                        await Task.Delay(refreshDelay.Value, rootCancellationToken.Token).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException ex)
                    {
                        if (timeoutSource.IsCancellationRequested)
                            throw new TimeoutException($"The requested timeout of {timeout.Value.TotalSeconds} seconds has been reached while waiting for the resource ({resourceId}) to reach the {status} state.", ex);

                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Waits for the resource to be deleted.
        /// <para>Treats a 404 NotFound exception as confirmation that it is deleted.</para>
        /// </summary>
        /// <param name="resourceId">The resource identifier.</param>
        /// <param name="deletedStatus">The deleted status for the specified resource.</param>
        /// <param name="getResource">Function which retrieves the resource.</param>
        /// <param name="refreshDelay">The amount of time to wait between requests.</param>
        /// <param name="timeout">The amount of time to wait before throwing a <see cref="TimeoutException"/>.</param>
        /// <param name="progress">The progress callback.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="TimeoutException">If the <paramref name="timeout"/> value is reached.</exception>
        /// <exception cref="FlurlHttpException">If the API call returns a bad <see cref="HttpStatusCode"/>.</exception>
        public static async Task WaitUntilDeletedAsync<TStatus>(string resourceId, TStatus deletedStatus, Func<Task<dynamic>> getResource, TimeSpan? refreshDelay = null, TimeSpan? timeout = null, IProgress<bool> progress = null, CancellationToken cancellationToken = default(CancellationToken))
            where TStatus : StringEnumeration
        {
            try
            {
                await WaitForStatusAsync<object, TStatus>(resourceId, deletedStatus, getResource, refreshDelay, timeout, progress, cancellationToken);
            }
            catch (FlurlHttpException httpError) when (httpError.Call.HttpStatus == HttpStatusCode.NotFound)
            {
                progress?.Report(true);
            }
        }
    }
}
