namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using System.Collections.Immutable;

    /// <summary>
    /// This class provides extension methods for the <see cref="StorageMetadata"/> class.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public static class StorageMetadataExtensions
    {
        /// <summary>
        /// Gets a <see cref="StorageMetadata"/> instance with the same type and metadata from the input object and the
        /// specified header values.
        /// </summary>
        /// <remarks>
        /// <para>This method exposes <see cref="StorageMetadata.WithHeadersImpl"/> in a manner that preserves the
        /// static type of the input <paramref name="storageMetadata"/>.</para>
        /// </remarks>
        /// <typeparam name="TMetadata">The static type of the input storage metadata.</typeparam>
        /// <param name="storageMetadata">The storage metadata object.</param>
        /// <param name="headers">The new headers for the object.</param>
        /// <returns>
        /// A <see cref="StorageMetadata"/> which represents the input <paramref name="storageMetadata"/> with the
        /// specified headers. If <paramref name="headers"/> is the same as the existing
        /// <see cref="StorageMetadata.Headers"/> for the input object, the method may return the
        /// <paramref name="storageMetadata"/> instance itself.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If <paramref name="headers"/> contains two keys which are equivalent using the
        /// <see cref="StringComparer.OrdinalIgnoreCase"/> comparer, but the associated values are different.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="storageMetadata"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="headers"/> is <see langword="null"/>.</para>
        /// </exception>
        public static TMetadata WithHeaders<TMetadata>(this TMetadata storageMetadata, ImmutableDictionary<string, string> headers)
            where TMetadata : StorageMetadata
        {
            if (storageMetadata == null)
                throw new ArgumentNullException("storageMetadata");

            return (TMetadata)storageMetadata.WithHeadersImpl(headers);
        }

        /// <summary>
        /// Gets a <see cref="StorageMetadata"/> instance with the same type and headers from the input object and the
        /// specified metadata values.
        /// </summary>
        /// <remarks>
        /// <para>This method exposes <see cref="StorageMetadata.WithMetadataImpl"/> in a manner that preserves the
        /// static type of the input <paramref name="storageMetadata"/>.</para>
        /// </remarks>
        /// <typeparam name="TMetadata">The static type of the input storage metadata.</typeparam>
        /// <param name="storageMetadata">The storage metadata object.</param>
        /// <param name="metadata">The new metadata for the object.</param>
        /// <returns>
        /// A <see cref="StorageMetadata"/> which represents the input <paramref name="storageMetadata"/> with the
        /// specified metadata. If <paramref name="metadata"/> is the same as the existing
        /// <see cref="StorageMetadata.Metadata"/> for the input object, the method may return the
        /// <paramref name="storageMetadata"/> instance itself.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If <paramref name="metadata"/> contains two keys which are equivalent using the
        /// <see cref="StringComparer.OrdinalIgnoreCase"/> comparer, but the associated values are different.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="storageMetadata"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="metadata"/> is <see langword="null"/>.</para>
        /// </exception>
        public static TMetadata WithMetadata<TMetadata>(this TMetadata storageMetadata, ImmutableDictionary<string, string> metadata)
            where TMetadata : StorageMetadata
        {
            if (storageMetadata == null)
                throw new ArgumentNullException("storageMetadata");

            return (TMetadata)storageMetadata.WithMetadataImpl(metadata);
        }
    }
}
