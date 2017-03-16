using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Flurl.Extensions;
using Flurl.Http;
using OpenStack.Authentication;
using OpenStack.Networking.v2.Serialization;
using OpenStack.ObjectStorage.v1.Serialization;
using OpenStack.Serialization;

namespace OpenStack.ObjectStorage.v1
{
	/// <summary>
	/// Builds requests to the Networking API which can be further customized and then executed.
	/// <para>Intended for custom implementations.</para>
	/// </summary>
	/// <seealso href="http://developer.openstack.org/api-ref-networking-v2.html">OpenStack Networking API v2 Reference</seealso>
	public class ObjectStorageApiBuilder
	{
		/// <summary />
		protected readonly IAuthenticationProvider AuthenticationProvider;

		/// <summary />
		protected readonly ServiceEndpoint Endpoint;

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectStorageApiBuilder"/> class.
		/// </summary>
		/// <param name="serviceType">The service type for the desired networking provider.</param>
		/// <param name="authenticationProvider">The authentication provider.</param>
		/// <param name="region">The region.</param>
		/// <param name="useInternalUrl">if set to <c>true</c> uses the internal URLs specified in the ServiceCatalog, otherwise the public URLs are used.</param>
		public ObjectStorageApiBuilder(IServiceType serviceType, IAuthenticationProvider authenticationProvider, string region, bool useInternalUrl)
		{
			if(serviceType == null)
				throw new ArgumentNullException("serviceType");
			if (authenticationProvider == null)
				throw new ArgumentNullException("authenticationProvider");
			if (string.IsNullOrEmpty(region))
				throw new ArgumentException("region cannot be null or empty", "region");

			AuthenticationProvider = authenticationProvider;
			Endpoint = new ServiceEndpoint(serviceType, authenticationProvider, region, useInternalUrl);
		}

		#region Tenants
		/*
		/// <summary>
		/// Returns details of current Tenant.
		/// </summary>
		/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>
		/// Details of current Tenant.
		/// </returns>
		public async Task<PreparedRequest> GetCurrentTenantAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);
 
			return endpoint
				.Authenticate(AuthenticationProvider)
				.PrepareGet(cancellationToken);
		}
		*/
		#endregion

		#region Containers
		/// <summary>
		/// Lists all containers associated with the current tenant.
		/// </summary>
		/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>
		/// A collection of containers associated with current tenant.
		/// </returns>
		public async Task<PreparedRequest> ListContainersAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);
 
			return endpoint
				.SetQueryParam("format", "json")
				.Authenticate(AuthenticationProvider)
				.PrepareGet(cancellationToken);
		}
		
		/// <summary>
		/// Gets the container content of specified container.
		/// </summary>
		/// <param name="containerName">The container identifier.</param>
		/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>
		/// The content of container.
		/// </returns>
		public virtual async Task<PreparedRequest> GetContainerContentAsync(string containerName, CancellationToken cancellationToken = default(CancellationToken))
		{
			Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);
			
			return endpoint
				.SetQueryParam("format", "json")
				.AppendPathSegments(containerName)
				.Authenticate(AuthenticationProvider)
				.PrepareGet(cancellationToken);
		}
		
		/// <summary>
		/// Gets the container content of specified container.
		/// </summary>
		/// <param name="containerName">The container identifier.</param>
		/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>
		/// The content of container.
		/// </returns>
		public virtual async Task<PreparedRequest> CreateContainerAsync(string containerName, CancellationToken cancellationToken = default(CancellationToken))
		{
			return await new Task<PreparedRequest>(() =>
			{
				throw new NotImplementedException();
			});
		}
		
		/// <summary>
		/// Gets the metadata from container.
		/// </summary>
		/// <param name="containerName">The container identifier.</param>
		/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>
		/// The content of container.
		/// </returns>
		public virtual async Task<PreparedRequest> ReadContainerMetadataAsync(string containerName, CancellationToken cancellationToken = default(CancellationToken))
		{
			Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);

			return endpoint
				.AppendPathSegments(containerName)
				.Authenticate(AuthenticationProvider)
				.PrepareHead(cancellationToken);
		}

		/// <summary>
		/// Update metadata to container.
		/// </summary>
		/// <param name="containerName">The container identifier.</param>
		/// <param name="metadataCollection">The metadata collection.</param>
		/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>
		/// The content of container.
		/// </returns>
		public virtual async Task<PreparedRequest> SaveContainerMetadataAsync(string containerName, ContainerMetadataCollection metadataCollection, CancellationToken cancellationToken = default(CancellationToken))
		{
			Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);

			return endpoint
				.AppendPathSegments(containerName)
				.Authenticate(AuthenticationProvider)
				.PreparePostContentHeader(metadataCollection.ToKeyValuePairs(), cancellationToken);
		}

		/// <summary>
		/// Deletes the specified container.
		/// </summary>
		/// <param name="containerName">The container identifier.</param>
		/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		public virtual async Task<PreparedRequest> DeleteContainerAsync(string containerName, CancellationToken cancellationToken = default(CancellationToken))
		{
			Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);

			return (PreparedRequest)endpoint
				.AppendPathSegments(containerName)
				.Authenticate(AuthenticationProvider)
				.PrepareDelete(cancellationToken)
				.AllowHttpStatus(HttpStatusCode.NotFound);
		}

		#endregion


		#region Container Objects

		/// <summary>
		/// Gets the object in specified container.
		/// </summary>
		/// <param name="containerName">The container identifier.</param>
		/// <param name="objectPath">The path of object.</param>
		/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>
		/// The content of container.
		/// </returns>
		public virtual async Task<PreparedRequest> GetContainerObjectAsync(string containerName, string objectPath, CancellationToken cancellationToken = default(CancellationToken))
		{
			Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);
			
			return endpoint
				.AppendPathSegments(containerName, objectPath)
				.SetQueryParam("format", "json")
				.Authenticate(AuthenticationProvider)
				.PrepareGet(cancellationToken);
		}

		/// <summary>
		/// Add or update the object to specified container.
		/// </summary>
		/// <param name="containerName">The container identifier.</param>
		/// <param name="objectPath">The path of object.</param>
		/// <param name="dataStream">Stream to obtains content of Object</param>
		/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>
		/// Nothing
		/// </returns>
		public virtual async Task<PreparedRequest> UpdateContainerObjectAsync(string containerName, string objectPath, System.IO.Stream dataStream, CancellationToken cancellationToken = default(CancellationToken))
		{
			Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);

			return endpoint
				.AppendPathSegments(containerName, objectPath)
				.Authenticate(AuthenticationProvider)
				.SettingTimeout(new TimeSpan(1, 0, 0, 0, 0))
				.PreparePutStream(dataStream, cancellationToken);
		}
		
		/// <summary>
		/// Delete the object in specified container.
		/// </summary>
		/// <param name="containerName">The container identifier.</param>
		/// <param name="objectPath">The path of object.</param>
		/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>
		/// Nothing
		/// </returns>
		public virtual async Task<PreparedRequest> DeleteContainerObjectAsync(string containerName, string objectPath, CancellationToken cancellationToken = default(CancellationToken))
		{
			Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);

			return endpoint
				.AppendPathSegments(containerName, objectPath)
				.Authenticate(AuthenticationProvider)
				.PrepareDelete(cancellationToken);
		}
		
		/// <summary>
		/// Get the metadata of object in specified container.
		/// </summary>
		/// <param name="containerName">The container identifier.</param>
		/// <param name="objectPath">The path of object.</param>
		/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>
		/// Metadata of object
		/// </returns>
		public virtual async Task<PreparedRequest> ReadContainerObjectMetadataAsync(string containerName, string objectPath, CancellationToken cancellationToken = default(CancellationToken))
		{
			Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);

			return endpoint
				.AppendPathSegments(containerName, objectPath)
				.Authenticate(AuthenticationProvider)
				.PrepareHead(cancellationToken)
				.AllowHttpStatus(HttpStatusCode.NotFound);
		}

		/// <summary>
		/// Save the metadata of object in specified container.
		/// </summary>
		/// <param name="containerName">The container identifier.</param>
		/// <param name="objectPath">The path of object.</param>
		/// <param name="metadataCollection">The metadata to save in object.</param>
		/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>
		/// Metadata of object
		/// </returns>
		public virtual async Task<PreparedRequest> SaveContainerObjectMetadataAsync(string containerName, string objectPath, ContainerObjectMetadataCollection metadataCollection, CancellationToken cancellationToken = default(CancellationToken))
		{
			Url endpoint = await Endpoint.GetEndpoint(cancellationToken).ConfigureAwait(false);

			return endpoint
				.AppendPathSegments(containerName, objectPath)
				.Authenticate(AuthenticationProvider)
				.PreparePostContentHeader(metadataCollection.ToKeyValuePairs(), cancellationToken);
		}


		#endregion

	}
}
