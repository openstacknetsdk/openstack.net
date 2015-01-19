namespace OpenStack.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenStack.Net;

    /// <summary>
    /// Provides a basic implementation of <see cref="ReadOnlyCollectionPage{T}"/> using
    /// a function delegate as the implementation of <see cref="PrepareGetNextPageAsync"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class BasicReadOnlyCollectionPage<T> : ReadOnlyCollectionPage<T>
    {
        /// <summary>
        /// This is the backing field for both <see cref="CanHaveNextPage"/> and <see cref="PrepareGetNextPageAsync"/>.
        /// </summary>
        private readonly Func<CancellationToken, Task<IHttpApiCall<ReadOnlyCollectionPage<T>>>> _prepareGetNextPageAsync;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicReadOnlyCollectionPage{T}"/> class
        /// that is a read-only wrapper around the specified list.
        /// </summary>
        /// <param name="list">The list to wrap.</param>
        /// <param name="prepareGetNextPageAsync">A function that returns a <see cref="Task{TResult}"/> representing the
        /// asynchronous operation to prepare an HTTP API call to get the next page of items in the collection. If
        /// specified, this function implements <see cref="PrepareGetNextPageAsync"/>. If the value is
        /// <see langword="null"/>, then <see cref="CanHaveNextPage"/> will return <see langword="false"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="list"/> is <see langword="null"/>.
        /// </exception>
        public BasicReadOnlyCollectionPage(IList<T> list, Func<CancellationToken, Task<IHttpApiCall<ReadOnlyCollectionPage<T>>>> prepareGetNextPageAsync)
            : base(list)
        {
            _prepareGetNextPageAsync = prepareGetNextPageAsync;
        }

        /// <inheritdoc/>
        public override bool CanHaveNextPage
        {
            get
            {
                return _prepareGetNextPageAsync != null;
            }
        }

        /// <inheritdoc/>
        public override Task<IHttpApiCall<ReadOnlyCollectionPage<T>>> PrepareGetNextPageAsync(CancellationToken cancellationToken)
        {
            if (!CanHaveNextPage)
                throw new InvalidOperationException("Cannot obtain the next page when CanHaveNextPage is false.");

            return _prepareGetNextPageAsync(cancellationToken);
        }
    }
}
