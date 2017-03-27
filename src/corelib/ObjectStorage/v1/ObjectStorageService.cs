using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Extensions;
using Flurl.Http;
using OpenStack.Authentication;
using OpenStack.Networking.v2.Serialization;
using OpenStack.ObjectStorage.v1.ContentObjectFilters;
using OpenStack.ObjectStorage.v1.Metadata.ContainerMetadata;
using OpenStack.ObjectStorage.v1.Metadata.ContainerObjectMetadata;
using OpenStack.ObjectStorage.v1.Serialization;

namespace OpenStack.ObjectStorage.v1
{
	/// <summary>
	/// The OpenStack Networking Service.
	/// </summary>
	/// <seealso href="https://wiki.openstack.org/wiki/Neutron/APIv2-specification">OpenStack Networking API v2 Overview</seealso>
	/// <seealso href="http://developer.openstack.org/api-ref-networking-v2.html">OpenStack Networking API v2 Reference</seealso>
	public class ObjectStorageService
	{
		internal readonly ObjectStorageApiBuilder _objectStorageApiBuilder;

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectStorageService"/> class.
		/// </summary>
		/// <param name="authenticationProvider">The authentication provider.</param>
		/// <param name="region">The region.</param>
		/// <param name="useInternalUrl">if set to <c>true</c> uses the internal URLs specified in the ServiceCatalog, otherwise the public URLs are used.</param>
		public ObjectStorageService(IAuthenticationProvider authenticationProvider, string region, bool useInternalUrl = false)
		{
			_objectStorageApiBuilder = new ObjectStorageApiBuilder(ServiceType.ObjectStorage, authenticationProvider, region, useInternalUrl);
		}



		#region Containers

		/// <inheritdoc cref="ObjectStorageApiBuilder.ListContainersAsync(CancellationToken)" />
		public async Task<IEnumerable<Container>> ListContainersAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return await _objectStorageApiBuilder
				.ListContainersAsync(cancellationToken)
				.SendAsync()
				.ReceiveJson<ContainerCollection>();
		}
		
		/// <inheritdoc cref="ObjectStorageApiBuilder.GetContainerUrlAsync(string, CancellationToken)" />
		public async Task<string> GetContainerUrlAsync(string containerName, CancellationToken cancellationToken = default(CancellationToken))
		{
			return await _objectStorageApiBuilder
				.GetContainerUrlAsync(containerName, cancellationToken);
		}

		/// <inheritdoc cref="ObjectStorageApiBuilder.GetContainerContentAsync(string, CancellationToken)" />
		public async Task<ContainerItemCollection> GetContainerContentAsync(string containerName, CancellationToken cancellationToken = default(CancellationToken))
		{
			return await _objectStorageApiBuilder
				.GetContainerContentAsync(containerName, cancellationToken)
				.SendAsync()
				.ReceiveJson<ContainerItemCollection>();
		}

		/// <inheritdoc cref="ObjectStorageApiBuilder.GetContainerContentAsync(string, ContentObjectFilterCollection, CancellationToken)" />
		public async Task<ContainerItemCollection> GetContainerContentAsync(string containerName, ContentObjectFilterCollection filterCollection, CancellationToken cancellationToken = default(CancellationToken))
		{
			return await _objectStorageApiBuilder
				.GetContainerContentAsync(containerName, filterCollection, cancellationToken)
				.SendAsync()
				.ReceiveJson<ContainerItemCollection>();
		}

		/// <inheritdoc cref="ObjectStorageApiBuilder.CreateContainerAsync(string, CancellationToken)" />
		public async Task<Container> CreateContainerAsync(string containerName, CancellationToken cancellationToken = default(CancellationToken))
		{
			return await _objectStorageApiBuilder
				.CreateContainerAsync(containerName, cancellationToken)
				.SendAsync()
				.ReceiveJson<Container>();
		}

		/// <inheritdoc cref="ObjectStorageApiBuilder.ReadContainerMetadataAsync(string, CancellationToken)" />
		public async Task<ContainerMetadataCollection> ReadContainerMetadataAsync(string containerName, CancellationToken cancellationToken = default(CancellationToken))
		{
			var requestHeaders = await _objectStorageApiBuilder
				.ReadContainerMetadataAsync(containerName, cancellationToken)
				.SendAsync()
				.ReceiveHeaders();

			var metadataSerializer = new MetadataSerializer<IContainerMetadata>();

			return new ContainerMetadataCollection(metadataSerializer.ParseMetadataHeaders(requestHeaders));
		}

		/// <inheritdoc cref="ObjectStorageApiBuilder.SaveContainerMetadataAsync(string, ContainerMetadataCollection, CancellationToken)" />
		public async Task SaveContainerMetadataAsync(string containerName, ContainerMetadataCollection metadataCollection, CancellationToken cancellationToken = default(CancellationToken))
		{
			await _objectStorageApiBuilder
				.SaveContainerMetadataAsync(containerName, metadataCollection, cancellationToken)
				.SendAsync();
		}

		/// <inheritdoc cref="ObjectStorageApiBuilder.DeleteContainerAsync(string, CancellationToken)" />
		public async Task<HttpResponseMessage> DeleteContainerAsync(string containerName, CancellationToken cancellationToken = default(CancellationToken))
		{
			return await _objectStorageApiBuilder
				.DeleteContainerAsync(containerName, cancellationToken)
				.SendAsync();
		}
		
		#endregion




		#region Container Objects
		
		/// <inheritdoc cref="ObjectStorageApiBuilder.GetContainerObjectAsync(string, string, CancellationToken)" />
		public async Task<Stream> GetContainerObjectAsync(string containerName, string objectPath, CancellationToken cancellationToken = default(CancellationToken))
		{
			return await _objectStorageApiBuilder
				.GetContainerObjectAsync(containerName, objectPath, cancellationToken)
				.SendAsync()
				.ReceiveStream();
		}
		
		/// <inheritdoc cref="ObjectStorageApiBuilder.UpdateContainerObjectAsync(string, string, System.IO.Stream, CancellationToken)" />
		public async Task UpdateContainerObjectAsync(string containerName, string objectPath, System.IO.Stream dataStream, CancellationToken cancellationToken = default(CancellationToken))
		{
			await _objectStorageApiBuilder
				.UpdateContainerObjectAsync(containerName, objectPath, dataStream, cancellationToken)
				.SendAsync();
		}
		
		/// <inheritdoc cref="ObjectStorageApiBuilder.UpdateContainerObjectAsync" />
		public async Task UpdateContainerObjectAsync(string containerName, string objectPath, string filePath, CancellationToken cancellationToken = default(CancellationToken))
		{
			using (var dataStream = new System.IO.FileStream(filePath, FileMode.Open, FileAccess.Read))
			{
				await this.UpdateContainerObjectAsync(containerName, objectPath, dataStream, cancellationToken);
			}
		}
		
		/// <inheritdoc cref="ObjectStorageApiBuilder.DeleteContainerObjectAsync(string, string, CancellationToken)" />
		public async Task DeleteContainerObjectAsync(string containerName, string objectPath, CancellationToken cancellationToken = default(CancellationToken))
		{
			await _objectStorageApiBuilder
				.DeleteContainerObjectAsync(containerName, objectPath, cancellationToken)
				.SendAsync();
		}
		
		/// <inheritdoc cref="ObjectStorageApiBuilder.DeleteContainerObjectListAsync(string, IEnumerable{string}, CancellationToken)" />
		public async Task<BulkDeleteResult> DeleteContainerObjectListAsync(string containerName, IEnumerable<string> objectPathList, CancellationToken cancellationToken = default(CancellationToken))
		{
			return await _objectStorageApiBuilder
				.DeleteContainerObjectListAsync(containerName, objectPathList, cancellationToken)
				.SendAsync()
				.ReceiveJson<BulkDeleteResult>();
		}
		
		/// <inheritdoc cref="ObjectStorageApiBuilder.ReadContainerObjectMetadataAsync(string, string, CancellationToken)" />
		public async Task<ContainerObjectMetadataCollection> ReadContainerObjectMetadataAsync(string containerName, string objectPath, CancellationToken cancellationToken = default(CancellationToken))
		{
			// Note: Object metadata are inside both `request.Content.Headers` and `request.Headers`.

			var request = await _objectStorageApiBuilder
				.ReadContainerObjectMetadataAsync(containerName, objectPath, cancellationToken)
				.SendAsync();

			var requestHeaders = new List<KeyValuePair<string, IEnumerable<string>>>();

			requestHeaders.AddRange(request.Headers.Select(item => item));
			requestHeaders.AddRange(request.Content.Headers.Select(item => item));

			var metadataSerializer = new MetadataSerializer<IContainerObjectMetadata>();

			return new ContainerObjectMetadataCollection(metadataSerializer.ParseMetadataHeaders(requestHeaders));
		}
		
		/// <inheritdoc cref="ObjectStorageApiBuilder.ReadContainerObjectMetadataAsync(string, string, CancellationToken)" />
		public async Task<bool> CheckContainerObjectExistsAsync(string containerName, string objectPath, CancellationToken cancellationToken = default(CancellationToken))
		{
			var request = await _objectStorageApiBuilder
				.ReadContainerObjectMetadataAsync(containerName, objectPath, cancellationToken)
				.SendAsync();

			var statusCode = request.StatusCode;

			return (int)statusCode < 300;
		}
		
		/// <inheritdoc cref="ObjectStorageApiBuilder.SaveContainerObjectMetadataAsync(string, string, ContainerObjectMetadataCollection, CancellationToken)" />
		public async Task SaveContainerObjectMetadataAsync(string containerName, string objectPath, ContainerObjectMetadataCollection metadataCollection, CancellationToken cancellationToken = default(CancellationToken))
		{
			await _objectStorageApiBuilder
				.SaveContainerObjectMetadataAsync(containerName, objectPath, metadataCollection, cancellationToken)
				.SendAsync();
		}

		#endregion
	}
}
