namespace net.openstack.Core.Exceptions
{
    using System;
    using net.openstack.Core.Domain;

    /// <summary>
    /// Represents errors that occur when a snapshot enters an error state while waiting
    /// on it to enter a particular state.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    public class SnapshotEnteredErrorStateException : Exception
    {
        /// <summary>
        /// Gets the error state the snapshot entered.
        /// </summary>
        public SnapshotState Status
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnapshotEnteredErrorStateException"/> with the
        /// specified snapshot state.
        /// </summary>
        /// <param name="status">The erroneous snapshot state.</param>
        public SnapshotEnteredErrorStateException(SnapshotState status)
            : base(string.Format("The snapshot entered an error state: '{0}'", status))
        {
            Status = status;
        }
    }
}
