using System;
using System.Runtime.Serialization;
using System.Security;
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
        /// <summary>
        /// Gets the requested geography.
        /// </summary>
        /// <seealso cref="CloudInstance"/>
        public string Geo { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownGeographyException"/> class
        /// with the specified geography.
        /// </summary>
        /// <param name="geo">The requested geography which is not supported.</param>
        public UnknownGeographyException(string geo) : base(string.Format("Unknown Geography: {0}", geo))
        {
            Geo = geo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownGeographyException"/> class with
        /// serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="info"/> is <c>null</c>.</exception>
        protected UnknownGeographyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            Geo = (string)info.GetValue("Geo", typeof(string));
        }

        /// <inheritdoc/>
        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            base.GetObjectData(info, context);
            info.AddValue("Geo", Geo);
        }
    }
}
