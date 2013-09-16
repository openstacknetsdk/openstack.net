namespace net.openstack.Core.Exceptions
{
    using System;
    using net.openstack.Core.Domain;

    /// <summary>
    /// Represents errors that occur when a volume enters an error state while waiting
    /// on it to enter a particular state.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    public class VolumeEnteredErrorStateException : Exception
    {
        /// <summary>
        /// Gets the error state the volume entered.
        /// </summary>
        public VolumeState Status
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VolumeEnteredErrorStateException"/> with the
        /// specified volume state.
        /// </summary>
        /// <param name="status">The erroneous volume state.</param>
        public VolumeEnteredErrorStateException(VolumeState status)
            : base(string.Format("The volume entered an error state: '{0}'", status))
        {
            Status = status;
        }
    }
}
