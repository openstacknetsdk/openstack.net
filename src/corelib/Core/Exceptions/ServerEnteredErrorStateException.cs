using System;
using System.Runtime.Serialization;
using net.openstack.Core.Domain;

namespace net.openstack.Core.Exceptions
{
    /// <summary>
    /// The exception that is thrown when the server enters an error state during a
    /// call to <see cref="O:IComputeProvider.WaitForServerState"/>.
    /// </summary>
    [Serializable]
    public class ServerEnteredErrorStateException : Exception
    {
        [NonSerialized]
        private ExceptionData _state;

        /// <summary>
        /// The state of the server.
        /// </summary>
        /// <seealso cref="ServerState"/>
        public ServerState Status
        {
            get
            {
                return _state.Status;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerEnteredErrorStateException"/> class
        /// with the specified error state.
        /// </summary>
        /// <param name="status">The error state entered by the server.</param>
        public ServerEnteredErrorStateException(ServerState status)
            : base(string.Format("The server entered an error state: '{0}'", status))
        {
            _state.Status = status;
            SerializeObjectState += (ex, args) => args.AddSerializedState(_state);
        }

        [Serializable]
        private struct ExceptionData : ISafeSerializationData
        {
            public ServerState Status
            {
                get;
                set;
            }

            void ISafeSerializationData.CompleteDeserialization(object deserialized)
            {
                ((ServerEnteredErrorStateException)deserialized)._state = this;
            }
        }
    }
}
