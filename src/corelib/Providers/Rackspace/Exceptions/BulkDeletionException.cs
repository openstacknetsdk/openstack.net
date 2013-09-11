using System;
using System.Runtime.Serialization;
using System.Security;
using net.openstack.Providers.Rackspace.Objects;

namespace net.openstack.Providers.Rackspace.Exceptions
{
    /// <summary>
    /// Represents errors which occur during a bulk delete operation.
    /// </summary>
    /// <seealso cref="CloudFilesProvider.BulkDelete"/>
    [Serializable]
    public class BulkDeletionException : Exception
    {
        /// <summary>
        /// Gets the detailed results of the bulk delete operation.
        /// </summary>
        public BulkDeletionResults Results { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkDeletionException"/> class
        /// with the specified status and results.
        /// </summary>
        /// <param name="status">A description of the status of the operation.</param>
        /// <param name="results">The results of the bulk delete operation.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="status"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="results"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="status"/> is empty.
        /// </exception>
        public BulkDeletionException(string status, BulkDeletionResults results)
            : base(string.Format("The bulk deletion operation did not complete successfully. Status: {0}", status))
        {
            if (status == null)
                throw new ArgumentNullException("status");
            if (results == null)
                throw new ArgumentNullException("results");
            if (string.IsNullOrEmpty(status))
                throw new ArgumentException("status cannot be empty");

            Results = results;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkDeletionException"/> class with
        /// serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="info"/> is <c>null</c>.</exception>
        protected BulkDeletionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            Results = (BulkDeletionResults)info.GetValue("BulkDeletionResults", typeof(BulkDeletionResults));
        }

        /// <inheritdoc/>
        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            base.GetObjectData(info, context);
            info.AddValue("BulkDeletionResults", Results);
        }
    }
}
