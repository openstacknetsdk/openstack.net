using System;
using System.Runtime.Serialization;
using net.openstack.Core.Domain;

namespace net.openstack.Core.Exceptions
{
    /// <summary>
    /// The exception that is thrown when the server enters an error state during a
    /// call to <see cref="O:IComputeProvider.WaitForImageState"/>.
    /// </summary>
    [Serializable]
    public class ImageEnteredErrorStateException : Exception
    {
        [NonSerialized]
        private ExceptionData _state;

        /// <summary>
        /// The state of the image.
        /// </summary>
        /// <seealso cref="ImageState"/>
        public ImageState Status
        {
            get
            {
                return _state.Status;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageEnteredErrorStateException"/> class
        /// with the specified error state.
        /// </summary>
        /// <param name="status">The error state entered by the image.</param>
        public ImageEnteredErrorStateException(ImageState status)
            : base(string.Format("The image entered an error state: '{0}'", status))
        {
            _state.Status = status;
            SerializeObjectState += (ex, args) => args.AddSerializedState(_state);
        }

        [Serializable]
        private struct ExceptionData : ISafeSerializationData
        {
            public ImageState Status
            {
                get;
                set;
            }

            void ISafeSerializationData.CompleteDeserialization(object deserialized)
            {
                ((ImageEnteredErrorStateException)deserialized)._state = this;
            }
        }
    }
}
