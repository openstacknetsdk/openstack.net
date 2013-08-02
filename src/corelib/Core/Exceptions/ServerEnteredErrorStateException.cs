using System;
using System.Runtime.Serialization;
using System.Security;
using net.openstack.Core.Domain;
using net.openstack.Core.Providers;

namespace net.openstack.Core.Exceptions
{
    /// <summary>
    /// The exception that is thrown when the server enters an error state during a
    /// call to <see cref="O:IComputeProvider.WaitForServerState"/> or <see cref="O:IComputeProvider.WaitForImageState"/>.
    /// </summary>
    [Serializable]
    public class ServerEnteredErrorStateException : Exception
    {
        /// <summary>
        /// The state of the server or image.
        /// </summary>
        /// <seealso cref="ServerState"/>
        /// <seealso cref="ImageState"/>
        public string Status
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerEnteredErrorStateException"/> class
        /// with the specified error state.
        /// </summary>
        /// <param name="status">The error state entered by the server or image.</param>
        public ServerEnteredErrorStateException(string status)
            : base(string.Format("The server entered an error state: '{0}'", status))
        {
            Status = status;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerEnteredErrorStateException"/> class with
        /// serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="info"/> is <c>null</c>.</exception>
        protected ServerEnteredErrorStateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            Status = (string)info.GetValue("Status", typeof(string));
        }

        /// <inheritdoc/>
        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            base.GetObjectData(info, context);
            info.AddValue("Status", Status);
        }
    }
}
