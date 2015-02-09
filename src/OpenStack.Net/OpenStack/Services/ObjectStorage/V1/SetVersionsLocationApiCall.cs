namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to set the name of the container where old versions of objects in a
    /// versioned container get placed when the objects are updated.
    /// </summary>
    /// <remarks>
    /// <para>This API call is part of the <see cref="PredefinedObjectStorageExtensions.ObjectVersioning">Object
    /// Versioning</see> extension to the OpenStack Object Storage Service V1.</para>
    /// </remarks>
    /// <seealso cref="IObjectVersioningExtension.PrepareSetVersionsLocationAsync"/>
    /// <seealso cref="ObjectVersioningExtensions.SetVersionsLocationAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class SetVersionsLocationApiCall : DelegatingHttpApiCall<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetVersionsLocationApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="httpApiCall"/> is <see langword="null"/>.
        /// </exception>
        public SetVersionsLocationApiCall(IHttpApiCall<string> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
