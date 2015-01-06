namespace OpenStack.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Threading.Tasks;
    using Rackspace.Threading;

    /// <summary>
    /// This class provides extension methods for the <see cref="ReadOnlyCollectionPage{T}"/> class.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public static class ReadOnlyCollectionPageExtensions
    {
        /// <summary>
        /// Get all pages in a paginated collection.
        /// </summary>
        /// <remarks>
        /// If <paramref name="progress"/> is non-<see langword="null"/>, the first call to
        /// <see cref="IProgress{T}.Report"/> will specify the <paramref name="page"/>
        /// argument. After each task to obtain to the next page of results completes,
        /// the <see cref="IProgress{T}.Report"/> method will be called again with the
        /// new page of results.
        /// <para>
        /// This method determines that the end of the collection is reached when either of
        /// the following conditions is true.
        /// </para>
        /// <list type="bullet">
        /// <item>The <see cref="ReadOnlyCollectionPage{T}.CanHaveNextPage"/> property returns <see langword="false"/>.</item>
        /// <item>An empty page is reached.</item>
        /// </list>
        /// </remarks>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="page">The first page in the collection.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <param name="progress">An optional callback object to receive progress notifications. If this is <see langword="null"/>, no progress notifications are sent.</param>
        /// <returns>
        /// A <see cref="Task"/> object representing the asynchronous operation. When the operation
        /// completes successfully, the <see cref="Task{TResult}.Result"/> property will contain a
        /// read-only collection containing the complete set of results from the paginated collection.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="page"/> is <see langword="null"/>.</exception>
        public static Task<ReadOnlyCollection<T>> GetAllPagesAsync<T>(this ReadOnlyCollectionPage<T> page, CancellationToken cancellationToken, IProgress<ReadOnlyCollectionPage<T>> progress)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            List<T> result = new List<T>();
            ReadOnlyCollectionPage<T> currentPage = page;
            Func<bool> condition = () => currentPage != null;
            Func<Task> body =
                () =>
                {
                    if (progress != null)
                        progress.Report(currentPage);

                    result.AddRange(currentPage);
                    if (currentPage.CanHaveNextPage && currentPage.Count > 0)
                    {
                        return currentPage.GetNextPageAsync(cancellationToken)
                            .Select(task => currentPage = task.Result);
                    }
                    else
                    {
                        currentPage = null;
                        return CompletedTask.Default;
                    }
                };

            return TaskBlocks.While(condition, body)
                .Select(_ => result.AsReadOnly());
        }
    }
}
