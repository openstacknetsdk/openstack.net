namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using System.Collections.Immutable;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenStack.Net;
    using Rackspace.Threading;

#if !PORTABLE || WINRT
    using System.Text;
    using OpenStack.Collections;

#if WINRT
    using System.Runtime.InteropServices.WindowsRuntime;
    using Windows.Security.Cryptography;
    using Windows.Security.Cryptography.Core;
    using Windows.Storage.Streams;
#else
    using CngAlgorithm = System.Security.Cryptography.CngAlgorithm;
    using HMAC = System.Security.Cryptography.HMAC;
#endif
#endif

    /// <summary>
    /// This class provides the default implementation of the Form POST middleware extension to the OpenStack Object
    /// Storage Service V1.
    /// </summary>
    /// <remarks>
    /// <note type="note">
    /// <para>This class should not be instantiated directly. It should be obtained by passing
    /// <see cref="PredefinedObjectStorageExtensions.FormPost"/> to
    /// <see cref="IExtensibleService{TService}.GetServiceExtension{TExtension}"/>.</para>
    /// </note>
    /// </remarks>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class FormPostExtension : ServiceExtension<IObjectStorageService>, IFormPostExtension
    {
        /// <summary>
        /// The Epoch used as a reference for Unix timestamps and file times.
        /// </summary>
        protected static readonly DateTimeOffset Epoch = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.Zero);

        /// <summary>
        /// Initializes a new instance of the <see cref="FormPostExtension"/> class using the specified Object
        /// Storage Service client and HTTP API call factory.
        /// </summary>
        /// <param name="service">The <see cref="IObjectStorageService"/> instance.</param>
        /// <param name="httpApiCallFactory">The factory to use for creating new HTTP API calls for the
        /// extension.</param>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="service"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="httpApiCallFactory"/> is <see langword="null"/>.</para>
        /// </exception>
        public FormPostExtension(IObjectStorageService service, IHttpApiCallFactory httpApiCallFactory)
            : base(service, httpApiCallFactory)
        {
        }

        /// <inheritdoc/>
        public virtual Task<FormPostSupportedApiCall> PrepareFormPostSupportedAsync(CancellationToken cancellationToken)
        {
            return Service.PrepareGetObjectStorageInfoAsync(cancellationToken)
                .Select(task => new FormPostSupportedApiCall(task.Result));
        }

        /// <inheritdoc/>
        public virtual Task<Tuple<Uri, ImmutableDictionary<string, string>>> CreateFormPostUriAsync(ContainerName container, ObjectName objectPrefix, string key, DateTimeOffset expiration, Uri redirectUri, long maxFileSize, int maxFileCount, CancellationToken cancellationToken)
        {
#if PORTABLE && !WINRT
            throw new NotSupportedException("The Form POST extension is not supported on the current platform.");
#else
            if (container == null)
                throw new ArgumentNullException("container");
            if (objectPrefix == null)
                throw new ArgumentNullException("objectPrefix");
            if (key == null)
                throw new ArgumentNullException("key");
            if (redirectUri == null)
                throw new ArgumentNullException("redirectUri");
            if (maxFileSize < 0)
                throw new ArgumentOutOfRangeException("maxFileSize");
            if (maxFileCount <= 0)
                throw new ArgumentOutOfRangeException("maxFileCount");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty");

            Func<Task<GetObjectMetadataApiCall>> resource = () => Service.PrepareGetObjectMetadataAsync(container, objectPrefix, cancellationToken);
            Func<Task<GetObjectMetadataApiCall>, Task<Tuple<Uri, ImmutableDictionary<string, string>>>> body =
                resourceTask =>
                {
                    Uri resourceAddress = resourceTask.Result.RequestMessage.RequestUri;

                    StringBuilder message = new StringBuilder();
                    message.Append(resourceAddress.PathAndQuery).Append('\n');
                    message.Append(redirectUri.AbsoluteUri).Append('\n');
                    message.Append(maxFileSize).Append('\n');
                    message.Append(maxFileCount).Append('\n');
                    message.Append(ToTimestamp(expiration));

                    byte[] hash;

#if WINRT
                    IBuffer messageBuffer = CryptographicBuffer.ConvertStringToBinary(message.ToString(), BinaryStringEncoding.Utf8);
                    IBuffer keyBuffer = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);
                    MacAlgorithmProvider provider = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha1);
                    CryptographicKey cryptographicKey = provider.CreateKey(keyBuffer);
                    IBuffer signatureBuffer = CryptographicEngine.Sign(cryptographicKey, messageBuffer);
                    hash = signatureBuffer.ToArray();
#else
                    using (HMAC hmac = HMAC.Create())
                    {
                        hmac.HashName = CngAlgorithm.Sha1.Algorithm;
                        hmac.Key = Encoding.UTF8.GetBytes(key);
                        hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message.ToString()));
                    }
#endif

                    string sig = string.Join(string.Empty, hash.ConvertAll(i => i.ToString("x2")));

                    ImmutableDictionary<string, string>.Builder fields = ImmutableDictionary.CreateBuilder<string, string>();
                    fields.Add("expires", ToTimestamp(expiration).ToString());
                    fields.Add("redirect", redirectUri.AbsoluteUri);
                    fields.Add("max_file_size", maxFileSize.ToString());
                    fields.Add("max_file_count", maxFileCount.ToString());
                    fields.Add("signature", sig);

                    return CompletedTask.FromResult(Tuple.Create(resourceAddress, fields.ToImmutable()));
                };

            return TaskBlocks.Using(resource, body);
#endif
        }

        /// <summary>
        /// Converts a <see cref="DateTimeOffset"/> value to a Unix-style timestamp (number of seconds since the
        /// <see cref="Epoch"/>).
        /// </summary>
        /// <param name="dateTimeOffset">The <see cref="DateTimeOffset"/> to convert.</param>
        /// <returns>The number of seconds since the <see cref="Epoch"/> until the time indicated by
        /// <paramref name="dateTimeOffset"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="dateTimeOffset"/> occurs before <see cref="Epoch"/>.
        /// </exception>
        protected long ToTimestamp(DateTimeOffset dateTimeOffset)
        {
            if (dateTimeOffset < Epoch)
                throw new ArgumentOutOfRangeException("Cannot convert a time before the epoch (January 1, 1970, 00:00 UTC) to a timestamp.", "dateTimeOffset");

            return (long)(dateTimeOffset - Epoch).TotalSeconds;
        }
    }
}
