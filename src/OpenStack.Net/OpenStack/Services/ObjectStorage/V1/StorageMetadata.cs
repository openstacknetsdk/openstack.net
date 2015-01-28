namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using OpenStack.Collections;

    /// <summary>
    /// This is the base class for representing metadata associated with resources in the Object Storage Service.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public abstract class StorageMetadata
    {
        /// <summary>
        /// This is the backing field for the <see cref="Headers"/> property.
        /// </summary>
        private readonly ImmutableDictionary<string, string> _headers;

        /// <summary>
        /// This is the backing field for the <see cref="Metadata"/> property.
        /// </summary>
        private readonly ImmutableDictionary<string, string> _metadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageMetadata"/> class using the metadata present in the
        /// specified response message, and a specified prefix for distinguishing custom metadata from other HTTP
        /// headers.
        /// </summary>
        /// <param name="responseMessage">The HTTP response to extract the metadata from.</param>
        /// <param name="metadataPrefix">The prefix used for HTTP headers representing custom metadata associated with
        /// the resource.</param>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="responseMessage"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="metadataPrefix"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">If <paramref name="metadataPrefix"/> is empty.</exception>
        protected StorageMetadata(HttpResponseMessage responseMessage, string metadataPrefix)
        {
            if (responseMessage == null)
                throw new ArgumentNullException("responseMessage");
            if (metadataPrefix == null)
                throw new ArgumentNullException("metadataPrefix");
            if (string.IsNullOrEmpty(metadataPrefix))
                throw new ArgumentException("metadataPrefix cannot be empty");

            ImmutableDictionary<string, string>.Builder headers = ImmutableDictionary.CreateBuilder<string, string>(StringComparer.OrdinalIgnoreCase);
            ImmutableDictionary<string, string>.Builder metadata = ImmutableDictionary.CreateBuilder<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, IEnumerable<string>> header in responseMessage.Headers)
            {
                if (header.Key.StartsWith(metadataPrefix))
                    metadata.Add(header.Key.Substring(metadataPrefix.Length), DecodeHeaderValue(string.Join(", ", header.Value.ToArray())));
                else
                    headers.Add(header.Key, DecodeHeaderValue(string.Join(", ", header.Value.ToArray())));
            }

            if (responseMessage.Content != null)
            {
                foreach (KeyValuePair<string, IEnumerable<string>> header in responseMessage.Content.Headers)
                {
                    if (header.Key.StartsWith(metadataPrefix))
                        metadata.Add(header.Key.Substring(metadataPrefix.Length), DecodeHeaderValue(string.Join(", ", header.Value.ToArray())));
                    else
                        headers.Add(header.Key, DecodeHeaderValue(string.Join(", ", header.Value.ToArray())));
                }
            }

            _headers = headers.ToImmutable();
            _metadata = metadata.ToImmutable();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageMetadata"/> class with the specified HTTP headers and
        /// custom metadata.
        /// </summary>
        /// <param name="headers">The custom HTTP headers associated with the resource.</param>
        /// <param name="metadata">The custom metadata associated with the resource.</param>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="headers"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="metadata"/> is <see langword="null"/>.</para>
        /// </exception>
        protected StorageMetadata(ImmutableDictionary<string, string> headers, ImmutableDictionary<string, string> metadata)
        {
            if (headers == null)
                throw new ArgumentNullException("headers");
            if (metadata == null)
                throw new ArgumentNullException("metadata");

            _headers = headers.WithComparers(StringComparer.OrdinalIgnoreCase);
            _metadata = metadata.WithComparers(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the prefix which distinguishes metadata values from other HTTP headers.
        /// </summary>
        /// <value>
        /// The prefix which distinguishes metadata values from other HTTP headers.
        /// </value>
        /// <seealso cref="Metadata"/>
        public abstract string MetadataPrefix
        {
            get;
        }

        /// <summary>
        /// Gets the non-metadata HTTP headers associated with the resource.
        /// </summary>
        /// <value>
        /// The non-metadata HTTP headers associated with the resource.
        /// </value>
        public ImmutableDictionary<string, string> Headers
        {
            get
            {
                return _headers;
            }
        }

        /// <summary>
        /// Gets the custom metadata headers associated with the resource.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// <para>The keys of the dictionary returned by this property do not include the HTTP header prefix which
        /// distinguishes custom metadata from other HTTP headers.</para>
        /// </note>
        /// </remarks>
        /// <value>
        /// The custom metadata headers associated with the resource.
        /// </value>
        public ImmutableDictionary<string, string> Metadata
        {
            get
            {
                return _metadata;
            }
        }

        /// <summary>
        /// Encode a string value for use as an HTTP header when applying values in <see cref="Headers"/> or
        /// <see cref="Metadata"/> to a particular <see cref="HttpRequestMessage"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Header values are encoded by converting the input string to a UTF-8 encoded sequence of bytes, followed by
        /// reinterpreting each byte as an ISO-8859-1 character, ensuring Unicode header values are properly processed
        /// by the Object Storage Service.
        /// </para>
        /// </remarks>
        /// <param name="value">The HTTP header value to encode for the Object Storage Service.</param>
        /// <returns>The encoded header value.</returns>
        public static string EncodeHeaderValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            byte[] encodedBytes = Encoding.UTF8.GetBytes(value);
            return new string(encodedBytes.ConvertAll(i => (char)i));
        }

        /// <summary>
        /// Decode a string value returned in an HTTP header from the Object Storage Service.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Header values are decoded by converting the input string to an ISO-8859-1 encoded sequence of bytes,
        /// followed by deserializing the byte array as a UTF-8 encoded string, ensuring that Unicode header values
        /// returned by the Object Storage Service are correctly converted to <see cref="string"/> values.
        /// </para>
        /// </remarks>
        /// <param name="value">The HTTP header value to decode.</param>
        /// <returns>The decoded header value.</returns>
        public static string DecodeHeaderValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            byte[] encodedBytes = value.ToCharArray().ConvertAll(i => (byte)i);
            return Encoding.UTF8.GetString(encodedBytes, 0, encodedBytes.Length);
        }
    }
}
