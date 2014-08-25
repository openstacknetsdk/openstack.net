namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;

    /// <summary>
    /// This class extends <see cref="StorageMetadata"/> to represent metadata associated with an object in the Object
    /// Storage service.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class ObjectMetadata : StorageMetadata
    {
        /// <summary>
        /// The prefix to apply to HTTP headers representing custom metadata associated with an object.
        /// </summary>
        public static readonly string ObjectMetadataPrefix = "X-Object-Meta-";

        /// <summary>
        /// An empty, immutable instance of <see cref="ObjectMetadata"/>. This is the backing
        /// field for the <see cref="Empty"/> property.
        /// </summary>
        private static readonly ObjectMetadata _emptyMetadata =
            new ObjectMetadata(new Dictionary<string, string>(), new Dictionary<string, string>());

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectMetadata"/> class using the metadata present in the
        /// specified response message.
        /// </summary>
        /// <param name="responseMessage">The HTTP response to extract the object metadata from.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="responseMessage"/> is <see langword="null"/>.
        /// </exception>
        public ObjectMetadata(HttpResponseMessage responseMessage)
            : base(responseMessage, ObjectMetadataPrefix)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectMetadata"/> class with the specified HTTP headers and
        /// custom object metadata.
        /// </summary>
        /// <param name="headers">The custom HTTP headers associated with the object.</param>
        /// <param name="metadata">The custom metadata associated with the object.</param>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="headers"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="metadata"/> is <see langword="null"/>.</para>
        /// </exception>
        public ObjectMetadata(IDictionary<string, string> headers, IDictionary<string, string> metadata)
            : base(headers, metadata)
        {
        }

        /// <summary>
        /// Gets an empty <see cref="ObjectMetadata"/> instance.
        /// </summary>
        /// <value>
        /// An empty <see cref="ObjectMetadata"/> instance.
        /// </value>
        public static ObjectMetadata Empty
        {
            get
            {
                return _emptyMetadata;
            }
        }

        /// <inheritdoc/>
        public override string MetadataPrefix
        {
            get
            {
                return ObjectMetadataPrefix;
            }
        }
    }
}
