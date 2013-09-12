using System;
using System.Runtime.Serialization;
using net.openstack.Core.Providers;
using net.openstack.Providers.Rackspace.Objects;

namespace net.openstack.Providers.Rackspace.Exceptions
{
    /// <summary>
    /// Represents errors which occur when an <see cref="IIdentityProvider"/> is
    /// requested for a <see cref="CloudInstance"/> which is not current supported.
    /// </summary>
    [Serializable]
    public class UnknownGeographyException : NotSupportedException
    {
        [NonSerialized]
        private ExceptionData _state;

        /// <summary>
        /// Gets the requested geography.
        /// </summary>
        /// <seealso cref="CloudInstance"/>
        public string Geo
        {
            get
            {
                return _state.Geo;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownGeographyException"/> class
        /// with the specified geography.
        /// </summary>
        /// <param name="geo">The requested geography which is not supported.</param>
        public UnknownGeographyException(string geo)
            : base(string.Format("Unknown Geography: {0}", geo))
        {
            _state.Geo = geo;
            SerializeObjectState += (ex, args) => args.AddSerializedState(_state);
        }

        [Serializable]
        private struct ExceptionData : ISafeSerializationData
        {
            public string Geo
            {
                get;
                set;
            }

            void ISafeSerializationData.CompleteDeserialization(object deserialized)
            {
                ((UnknownGeographyException)deserialized)._state = this;
            }
        }
    }
}
