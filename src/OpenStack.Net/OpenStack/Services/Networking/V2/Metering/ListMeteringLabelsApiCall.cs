namespace OpenStack.Services.Networking.V2.Metering
{
    using OpenStack.Collections;
    using OpenStack.Net;

    /// <summary>
    /// This class represents an HTTP API call to <placeholder>ListMeteringLabels</placeholder> with the OpenStack Networking Service V2.
    /// </summary>
    /// <seealso cref="IMeteringExtension.PrepareListMeteringLabelsAsync"/>
    /// <seealso cref="MeteringExtensions.ListMeteringLabelsAsync"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class ListMeteringLabelsApiCall : DelegatingHttpApiCall<ReadOnlyCollectionPage<MeteringLabel>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListMeteringLabelsApiCall"/> class
        /// with the behavior provided by another <see cref="IHttpApiCall{T}"/> instance.
        /// </summary>
        /// <param name="httpApiCall">The <see cref="IHttpApiCall{T}"/> providing the behavior for the API call.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="httpApiCall"/> is <see langword="null"/>.</exception>
        public ListMeteringLabelsApiCall(IHttpApiCall<ReadOnlyCollectionPage<MeteringLabel>> httpApiCall)
            : base(httpApiCall)
        {
        }
    }
}
