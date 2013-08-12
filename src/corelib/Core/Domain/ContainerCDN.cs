namespace net.openstack.Core.Domain
{
    using net.openstack.Core.Providers;
    using Newtonsoft.Json;

    /// <summary>
    /// Provides the CDN properties for a container in an Object Storage provider.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ContainerCDN
    {
        /// <summary>
        /// The name of the container.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// A streaming URL suitable for use in links to content you want to stream, such as video. If streaming is not available, the value is <c>null</c>.
        /// </summary>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/Streaming-CDN-Enabled_Containers-d1f3721.html">Streaming CDN-Enabled Containers (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        [JsonProperty("cdn_streaming_uri")]
        public string CDNStreamingUri { get; set; }

        /// <summary>
        /// Gets a URL SSL URL for accessing the container on the CDN. If SSL is not available, the value is <c>null</c>.
        /// </summary>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/CDN-Enabled_Containers_Served_via_SSL-d1e2821.html">CDN-Enabled Containers Served through SSL (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        [JsonProperty("cdn_ssl_uri")]
        public string CDNSslUri { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not the container is CDN-Enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if the container is CDN-Enabled; otherwise, <c>false</c>.
        /// </value>
        /// <seealso cref="O:IObjectStorageProvider.EnableCDNOnContainer"/>
        /// <seealso cref="IObjectStorageProvider.DisableCDNOnContainer"/>
        [JsonProperty("cdn_enabled")]
        public bool CDNEnabled { get; set; }

        /// <summary>
        /// Gets the Time To Live (TTL) in seconds for a CDN-Enabled container.
        /// </summary>
        [JsonProperty("ttl")]
        public long Ttl { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not log retention is enabled for a CDN-Enabled container.
        /// </summary>
        /// <remarks>
        /// This setting specifies whether the CDN access logs should be collected and stored in the Cloud Files storage system.
        /// </remarks>
        /// <value>
        /// <c>true</c> if log retention is enabled for the container; otherwise, <c>false</c>.
        /// </value>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/List_CDN-Enabled_Container_Metadata-d1e2711.html">List a CDN-Enabled Container's Metadata (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        [JsonProperty("log_retention")]
        public bool LogRetention { get; set; }

        /// <summary>
        /// Gets a publicly accessible URL for the container, which can be combined with any object name within the container to form the publicly accessible URL for that object for distribution over a CDN system.
        /// </summary>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/List_CDN-Enabled_Container_Metadata-d1e2711.html">List a CDN-Enabled Container's Metadata (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        [JsonProperty("cdn_uri")]
        public string CDNUri { get; set; }

        /// <summary>
        /// Gets a publicly accessible URL for the container for use in streaming content to iOS devices. If iOS streaming is not available for the container, the value is <c>null</c>.
        /// </summary>
        /// <remarks>
        /// The <see cref="CDNIosUri"/> may be combined with any object name within the container to form the publicly accessible URL for streaming that object to iOS devices.
        /// </remarks>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/iOS-Streaming-d1f3725.html">iOS Streaming (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        [JsonProperty("cdn_ios_uri")]
        public string CDNIosUri { get; set; }

    }
}
