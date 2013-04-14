using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JSIStudios.SimpleRESTServices.Client;
using net.openstack.Core.Domain;
using net.openstack.Core.Exceptions;
using net.openstack.Providers.Rackspace.Exceptions;

namespace net.openstack.Core
{
    public interface ICloudFilesProvider
    {
        #region Container
        /// <summary>
        /// Lists the containers.
        /// </summary>
        /// <param name="limit">The limit.<remarks>[Optional]</remarks></param>
        /// <param name="marker">The marker.<remarks>[Optional]</remarks></param>
        /// <param name="markerEnd">The marker end.<remarks>[Optional]</remarks></param>
        /// <param name="region">The region to fetch Containers.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <returns>IEnumerable of <see cref="net.openstack.Core.Domain.Container"/></returns>
        IEnumerable<Container> ListContainers(int? limit = null, string marker = null, string markerEnd = null, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Creates the container.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="region">The region, in which to create Container.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <returns><see cref="ObjectStore"/></returns>
        ObjectStore CreateContainer(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Deletes the container.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="region">The region, in which to delete Container.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <returns><see cref="ObjectStore"/></returns>
        ObjectStore DeleteContainer(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Gets the container header.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="region">The region, in which to get Container Header.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <returns>Dictionary&lt;string,string&gt;</returns>
        Dictionary<string, string> GetContainerHeader(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Gets the container meta data.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="region">The region, in which to get Container Meta Data.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <returns>Dictionary&lt;string,string&gt;</returns>
        Dictionary<string, string> GetContainerMetaData(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Gets the container CDN header.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="region">The region, in which to get Container CDN Header.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <returns><see cref="ContainerCDN"/></returns>
        ContainerCDN GetContainerCDNHeader(string container, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Lists the CDN containers.
        /// </summary>
        /// <param name="limit">The limit.<remarks>[Optional]</remarks></param>
        /// <param name="marker">The marker.<remarks>[Optional]</remarks></param>
        /// <param name="markerEnd">The marker end.<remarks>[Optional]</remarks></param>
        /// <param name="cdnEnabled">if set to <c>true</c> lists CDN enabled containers.</param>
        /// <param name="region">The region, in which to List CDN Container.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <returns>IEnumerable of <see cref="net.openstack.Core.Domain.ContainerCDN"/></returns>
        IEnumerable<ContainerCDN> ListCDNContainers(int? limit = null, string markerId = null, string markerEnd = null, bool cdnEnabled = false, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Enables the CDN on container.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="ttl">The TTL in seconds.<c>[Range 900 to 1577836800]</c></param>
        /// <param name="region">The region, in which to enable CDN Container.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="TTLLengthException">TTL range must be 900 to 1577836800 seconds TTL:  + ttl.ToString(CultureInfo.InvariantCulture)</exception>
        /// <returns>Dictionary&lt;string,string&gt; of CDN Headers</returns>
        Dictionary<string, string> EnableCDNOnContainer(string container, long ttl, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Enables the CDN on container.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="logRetention">if set to <c>true</c> enables log retention on container.</param>
        /// <param name="region">The region, in which to enable CDN Container.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <returns>Dictionary&lt;string,string&gt; of CDN Headers</returns>
        Dictionary<string, string> EnableCDNOnContainer(string container, bool logRetention, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Enables the CDN on container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="ttl">The TTL in seconds.<c>[Range 900 to 1577836800]</c></param>
        /// <param name="logRetention">if set to <c>true</c> enables log retention on container.</param>
        /// <param name="region">The region, in which to enable CDN Container.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <returns>Dictionary&lt;string,string&gt; of CDN Headers</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="TTLLengthException">TTL range must be 900 to 1577836800 seconds TTL:  + ttl.ToString(CultureInfo.InvariantCulture)</exception>
        Dictionary<string, string> EnableCDNOnContainer(string container, long ttl, bool logRetention, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Disables the CDN on container.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="region">The region, in which to disable CDN Container.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The identity <remarks>[Nullable]</remarks></param>
        /// <returns>Dictionary&lt;string,string&gt; of CDN Headers</returns>
        Dictionary<string, string> DisableCDNOnContainer(string container, string region = null, CloudIdentity identity = null);


        /// <summary>
        /// Updates the container metadata.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="metadata">The metadata. <remarks>Dictionary&lt;string,string&gt;</remarks></param>
        /// <param name="region">The region, in which to update Container Meta Data.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <exception cref="System.ArgumentNullException"></exception>
        void UpdateContainerMetadata(string container, Dictionary<string, string> metadata, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Adds the container headers.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="headers">The headers.<remarks>Dictionary&lt;string,string&gt;</remarks></param>
        /// <param name="region">The region, in which to add Container Header.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <exception cref="System.ArgumentNullException"></exception>
        void AddContainerHeaders(string container, Dictionary<string, string> headers, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Updates the container CDN headers.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="headers">The headers.<remarks>Dictionary&lt;string,string&gt;</remarks></param>
        /// <param name="region">The region, in which to update CDN Container Headers.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="CDNNotEnabledException"></exception>
        void UpdateContainerCdnHeaders(string container, Dictionary<string, string> headers, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Enables the static web on container.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="index">Value for <c>x-container-meta-web-index</c>.</param>
        /// <param name="error">Value for <c>x-container-meta-web-error</c>.</param>
        /// <param name="css">Value for <c>x-container-meta-web-listings-css</c>.</param>
        /// <param name="listing">Value for <c>x-container-meta-web-listings</c></param>
        /// <param name="region">The region, in which to enable static web on Container.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <exception cref="CDNNotEnabledException"></exception>
        void EnableStaticWebOnContainer(string container, string index, string error, string css, bool listing, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Enables the static web on container.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="index">Value for <c>x-container-meta-web-index</c>.</param>
        /// <param name="error">Value for <c>x-container-meta-web-error</c>.</param>
        /// <param name="listing">Value for <c>x-container-meta-web-listings</c></param>
        /// <param name="region">The region, in which to enable static web on Container.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <exception cref="CDNNotEnabledException"></exception>
        void EnableStaticWebOnContainer(string container, string index, string error, bool listing, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Enables the static web on container.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="css">Value for <c>x-container-meta-web-listings-css</c>.</param>
        /// <param name="listing">Value for <c>x-container-meta-web-listings</c></param>
        /// <param name="region">The region, in which to enable static web on Container.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <exception cref="CDNNotEnabledException"></exception>
        void EnableStaticWebOnContainer(string container, string css, bool listing, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Enables the static web on container.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="index">Value for <c>x-container-meta-web-index</c>.</param>
        /// <param name="error">Value for <c>x-container-meta-web-error</c>.</param>
        /// <param name="region">The region, in which to enable static web on Container.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <exception cref="CDNNotEnabledException"></exception>
        void EnableStaticWebOnContainer(string container, string index, string error, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Disables the static web on container.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="region">The region, in which to disable static web on Container.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <exception cref="CDNNotEnabledException"></exception>
        void DisableStaticWebOnContainer(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);
        #endregion

        #region Container Objects

        /// <summary>
        /// Gets the object headers.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="region">The region, in which to get Object Headers.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <returns>Dictionary&lt;string,string&gt; of CDN Headers</returns>
        Dictionary<string, string> GetObjectHeaders(string container, string objectName, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Gets the object meta data.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="region">The region, in which to get Object Meta Data.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        ///<param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <returns>Dictionary&lt;string,string&gt; of Meta data</returns>
        Dictionary<string, string> GetObjectMetaData(string container, string objectName, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Lists the objects.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="limit">The limit.<remarks>[Optional]</remarks></param>
        /// <param name="marker">The marker.<remarks>[Optional]</remarks></param>
        /// <param name="markerEnd">The marker end.<remarks>[Optional]</remarks></param>
        /// <param name="region">The region, in which to list Objects.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <returns>IEnumerable of <see cref="net.openstack.Core.Domain.ContainerObject"/></returns>
        IEnumerable<ContainerObject> ListObjects(string container, int? limit = null, string marker = null, string markerEnd = null, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Creates the object from file.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="filePath">The file path.<remarks>Example c:\folder1\folder2\image_name.jpeg</remarks></param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="chunkSize">Chunk size.<remarks>[Default = 4096]</remarks> </param>
        /// <param name="headers">The headers. <remarks>[Optional]</remarks></param>
        /// <param name="region">The region, in which to create Object.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="progressUpdated">The progress updated. <see cref="Action&lt;T&gt;"/> </param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        void CreateObjectFromFile(string container, string filePath, string objectName, int chunkSize = 65536, Dictionary<string, string> headers = null, string region = null, Action<long> progressUpdated = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Creates the object.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="stream">The stream. <see cref="Stream"/></param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="chunkSize">Chunk size.<remarks>[Default = 4096]</remarks> </param>
        /// <param name="headers">The headers. <remarks>[Optional]</remarks></param>
        /// <param name="region">The region, in which to create Object.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="progressUpdated">The progress updated. <see cref="Action&lt;T&gt;"/> </param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        void CreateObject(string container, Stream stream, string objectName, int chunkSize = 65536, Dictionary<string, string> headers = null, string region = null, Action<long> progressUpdated = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Gets the object.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="outputStream">The output stream.<see cref="Stream"/></param>
        /// <param name="chunkSize">Chunk size.<remarks>[Default = 4096]</remarks> </param>
        /// <param name="headers">The headers. <remarks>[Optional]</remarks></param>
        /// <param name="region">The region, in which to get Object.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="verifyEtag">if set to <c>true</c> will verify etag.</param>
        /// <param name="progressUpdated">The progress updated. <see cref="Action&lt;T&gt;"/> </param>
        ///<param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <exception cref="InvalidETagException"></exception>
        void GetObject(string container, string objectName, Stream outputStream, int chunkSize = 65536, Dictionary<string, string> headers = null, string region = null, bool verifyEtag = false, Action<long> progressUpdated = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Gets the object save to file.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="saveDirectory">The save directory path. <remarks>Example c:\user\</remarks></param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="fileName">Name of the file.<remarks>Example image_name1.jpeg</remarks></param>
        /// <param name="chunkSize">Chunk size.<remarks>[Default = 4096]</remarks> </param>
        /// <param name="headers">The headers. <remarks>[Optional]</remarks></param>
        /// <param name="region">The region, in which to get Object.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="verifyEtag">if set to <c>true</c> will verify etag.</param>
        /// <param name="progressUpdated">The progress updated. <see cref="Action&lt;T&gt;"/> </param>
        ///<param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        void GetObjectSaveToFile(string container, string saveDirectory, string objectName, string fileName = null, int chunkSize = 65536, Dictionary<string, string> headers = null, string region = null, bool verifyEtag = false, Action<long> progressUpdated = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Copies the object.
        /// </summary>
        /// <param name="sourceContainer">The source container name.</param>
        /// <param name="sourceObjectName">Name of the source object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="destinationContainer">The destination container name.</param>
        /// <param name="destinationObjectName">Name of the destination object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="headers">The headers. <remarks>[Optional]</remarks></param>
        /// <param name="region">The region, in which to copy Object.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        ///<param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <returns><see cref="ObjectStore"/></returns>
        ObjectStore CopyObject(string sourceContainer, string sourceObjectName, string destinationContainer, string destinationObjectName, Dictionary<string, string> headers = null, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Deletes the object.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="headers">The headers. <remarks>[Optional]</remarks></param>
        /// <param name="region">The region, in which to delete Object.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <returns><see cref="ObjectStore"/></returns>
        ObjectStore DeleteObject(string container, string objectName, Dictionary<string, string> headers = null, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Purges the object from CDN.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="region">The region, in which to purge CDN Object.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <returns><see cref="ObjectStore"/></returns>
        /// <exception cref="CDNNotEnabledException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        ObjectStore PurgeObjectFromCDN(string container, string objectName, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Purges the object from CDN.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="email">Email Address.</param>
        /// <param name="region">The region, in which to purge CDN Object.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <returns><see cref="ObjectStore"/></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="CDNNotEnabledException"></exception>
        ObjectStore PurgeObjectFromCDN(string container, string objectName, string email, string region = null, CloudIdentity identity = null);

        /// <summary>
        /// Purges the object from CDN.
        /// </summary>
        /// <param name="container">The container name.</param>
        /// <param name="objectName">Name of the object.<remarks>Example image_name.jpeg</remarks></param>
        /// <param name="emails">string[] of email address.</param>
        /// <param name="region">The region, in which to purge CDN Object.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <returns><see cref="ObjectStore"/></returns>
        /// <exception cref="CDNNotEnabledException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        ObjectStore PurgeObjectFromCDN(string container, string objectName, string[] emails, string region = null, CloudIdentity identity = null);
        #endregion

        #region Accounts

        /// <summary>
        /// Gets the account headers.
        /// </summary>
        /// <param name="region">The region, in which to get account Header.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <returns>Dictionary&lt;string,string&gt; of headers</returns>
        Dictionary<string, string> GetAccountHeaders(string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Gets the account meta data.
        /// </summary>
        /// <param name="region">The region, in which to get account Meta Data.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <returns>Dictionary&lt;string,string&gt; of meta data</returns>
        Dictionary<string, string> GetAccountMetaData(string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Updates the account metadata.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="region">The region, in which to update Account Meta Data.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <exception cref="System.ArgumentNullException"></exception>
        void UpdateAccountMetadata(Dictionary<string, string> metadata, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        /// <summary>
        /// Updates the account headers.
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <param name="region">The region, in which to update Account Header.<remarks>[Optional]: If not specified, the users default region will be used.</remarks></param>
        /// <param name="useInternalUrl">if set to <c>true</c> uses ServiceNet URL.</param>
        /// <param name="identity">The users Cloud Identity. <see cref="CloudIdentity"/> <remarks>[Optional]: If not specified, the default identity given in the constructor will be used.</remarks> </param>
        /// <exception cref="System.ArgumentNullException"></exception>
        void UpdateAccountHeaders(Dictionary<string, string> headers, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        #endregion
    }
}
