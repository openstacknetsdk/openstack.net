namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to update the metadata associated with an account in the Object Storage
    /// Service.
    /// </summary>
    /// <seealso cref="IObjectStorageService.PrepareUpdateAccountMetadataAsync"/>
    /// <seealso cref="ObjectStorageServiceExtensions.UpdateAccountMetadataAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class UpdateAccountMetadataApiCall : DelegatingHttpApiCall<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateAccountMetadataApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpApiCall"/> is <see langword="null"/>.
        /// </exception>
        public UpdateAccountMetadataApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
