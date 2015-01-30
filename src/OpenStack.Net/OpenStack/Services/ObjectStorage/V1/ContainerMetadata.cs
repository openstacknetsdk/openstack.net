namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using System.Collections.Immutable;
    using System.Net.Http;

    /// <summary>
    /// This class extends <see cref="StorageMetadata"/> to represent metadata associated with a container in the Object
    /// Storage service.
    /// </summary>
    /// <threadsafety static="true" instance="true"/>
    /// <preliminary/>
    public class ContainerMetadata : StorageMetadata
    {
        /// <summary>
        /// The prefix to apply to HTTP headers representing custom metadata associated with a container.
        /// </summary>
        public static readonly string ContainerMetadataPrefix = "X-Container-Meta-";

        /// <summary>
        /// An empty, immutable instance of <see cref="ContainerMetadata"/>. This is the backing
        /// field for the <see cref="Empty"/> property.
        /// </summary>
        private static readonly ContainerMetadata _emptyMetadata =
            new ContainerMetadata(ImmutableDictionary<string, string>.Empty, ImmutableDictionary<string, string>.Empty);

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerMetadata"/> class using the metadata present in the
        /// specified response message.
        /// </summary>
        /// <param name="responseMessage">The HTTP response to extract the container metadata from.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="responseMessage"/> is <see langword="null"/>.
        /// </exception>
        public ContainerMetadata(HttpResponseMessage responseMessage)
            : base(responseMessage, ContainerMetadataPrefix)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerMetadata"/> class with the specified HTTP headers and
        /// custom container metadata.
        /// </summary>
        /// <param name="headers">The custom HTTP headers associated with the container.</param>
        /// <param name="metadata">The custom metadata associated with the container.</param>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="headers"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="metadata"/> is <see langword="null"/>.</para>
        /// </exception>
        public ContainerMetadata(ImmutableDictionary<string, string> headers, ImmutableDictionary<string, string> metadata)
            : base(headers, metadata)
        {
        }

        /// <summary>
        /// Gets an empty <see cref="ContainerMetadata"/> instance.
        /// </summary>
        /// <value>
        /// An empty <see cref="ContainerMetadata"/> instance.
        /// </value>
        public static ContainerMetadata Empty
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
                return ContainerMetadataPrefix;
            }
        }
    }
}
