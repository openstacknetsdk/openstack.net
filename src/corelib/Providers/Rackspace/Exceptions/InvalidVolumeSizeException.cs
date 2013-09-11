using System;
using System.Runtime.Serialization;
using net.openstack.Core.Validators;

namespace net.openstack.Providers.Rackspace.Exceptions
{
    /// <summary>
    /// Represents errors which occur during validation of a block storage volume size.
    /// </summary>
    /// <seealso cref="IBlockStorageValidator.ValidateVolumeSize"/>
    [Serializable]
    public class InvalidVolumeSizeException : Exception
    {
        /// <summary>
        /// Gets the requested volume size.
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidVolumeSizeException"/> class
        /// with the specified volume size.
        /// </summary>
        /// <param name="size">The invalid volume size which was requested.</param>
        public InvalidVolumeSizeException(int size)
            : base(string.Format("The volume size value must be between 100 and 1000. The size requested was: {0}", size))
        {
            Size = size;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidVolumeSizeException"/> class with
        /// serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="info"/> is <c>null</c>.</exception>
        protected InvalidVolumeSizeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
