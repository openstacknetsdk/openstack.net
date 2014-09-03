namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to set the name of the container where old versions of objects in a
    /// versioned container get placed when the objects are updated.
    /// </summary>
    /// <remarks>
    /// <para>This API call is part of the <see cref="PredefinedObjectStorageExtensions.ObjectVersioning">Object
    /// Versioning</see> extension to the OpenStack Object Storage Service V1.</para>
    /// </remarks>
    /// <seealso cref="IObjectVersioningExtension.PrepareGetVersionsLocationAsync"/>
    /// <seealso cref="ObjectVersioningExtensions.GetVersionsLocationAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class GetVersionsLocationApiCall : DelegatingHttpApiCall<ContainerName>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetVersionsLocationApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpApiCall"/> is <see langword="null"/>.
        /// </exception>
        public GetVersionsLocationApiCall(IHttpApiCall<ContainerName> httpApiCall)
            : base(httpApiCall)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetVersionsLocationApiCall"/> class using the default behavior
        /// of modifying a <see cref="GetContainerMetadataApiCall"/> to produce the desired behavior.
        /// </summary>
        /// <param name="httpApiCall">The prepared <see cref="GetContainerMetadataApiCall"/> call.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpApiCall"/> is <see langword="null"/>.
        /// </exception>
        public GetVersionsLocationApiCall(GetContainerMetadataApiCall httpApiCall)
            : this(WrapGetContainerMetadataCall(httpApiCall))
        {
        }

        /// <summary>
        /// Extract the <see cref="ObjectVersioningExtensions.VersionsLocation"/> header from the
        /// <see cref="ContainerMetadata"/> provided by a <see cref="GetContainerMetadataApiCall"/> API call.
        /// </summary>
        /// <param name="response">The raw response to the HTTP API call.</param>
        /// <param name="result">The deserialized <see cref="ContainerMetadata"/> provided by the HTTP API call.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// <para>The name of the container where old versions of objects in a versioned container get placed when the
        /// objects are updated.</para>
        /// <para>-or-</para>
        /// <para><see langword="null"/> if <paramref name="result"/> is <see langword="null"/>, or if the
        /// <see cref="ObjectVersioningExtensions.VersionsLocation"/> header is not set for the container.</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="response"/> is <see langword="null"/>.
        /// </exception>
        /// <see cref="ObjectVersioningExtensions.GetVersionsLocation(ContainerMetadata)"/>
        protected static ContainerName SelectResult(HttpResponseMessage response, ContainerMetadata result, CancellationToken cancellationToken)
        {
            if (response == null)
                throw new ArgumentNullException("response");

            if (result == null)
                return null;

            return result.GetVersionsLocation();
        }

        /// <summary>
        /// Create an <see cref="IHttpApiCall{T}"/> instance providing the behavior for the
        /// <see cref="GetVersionsLocationApiCall"/> API call by applying the <see cref="SelectResult"/> transformation
        /// to the result of a <see cref="GetContainerMetadataApiCall"/> API call.
        /// </summary>
        /// <param name="httpApiCall">The prepared <see cref="GetContainerMetadataApiCall"/> call.</param>
        /// <returns>An <see cref="IHttpApiCall{T}"/> providing the behavior for the
        /// <see cref="GetVersionsLocationApiCall"/> API call.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpApiCall"/> is <see langword="null"/>.
        /// </exception>
        protected static IHttpApiCall<ContainerName> WrapGetContainerMetadataCall(GetContainerMetadataApiCall httpApiCall)
        {
            return new TransformHttpApiCall<ContainerMetadata, ContainerName>(httpApiCall, SelectResult);
        }
    }
}
