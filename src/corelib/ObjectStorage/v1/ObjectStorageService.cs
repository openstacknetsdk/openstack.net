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

		/// <inheritdoc cref="ObjectStorageApiBuilder.GetContainerContentAsync(string, CancellationToken)" />
		public async Task<ContainerObjectCollection> GetContainerContentAsync(string containerId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return await _objectStorageApiBuilder
				.GetContainerContentAsync(containerId, cancellationToken)
				.SendAsync()
				.ReceiveJson<ContainerObjectCollection>();
		}

		/// <inheritdoc cref="ObjectStorageApiBuilder.CreateContainerAsync(string, CancellationToken)" />
		public async Task<Container> CreateContainerAsync(string containerId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return await _objectStorageApiBuilder
				.CreateContainerAsync(containerId, cancellationToken)
				.SendAsync()
				.ReceiveJson<Container>();
		}

		/// <inheritdoc cref="ObjectStorageApiBuilder.ReadContainerMetadataAsync(string, CancellationToken)" />
		public async Task<ContainerMetadataCollection> ReadContainerMetadataAsync(string containerId, CancellationToken cancellationToken = default(CancellationToken))
		{
			var requestHeaders = await _objectStorageApiBuilder
				.ReadContainerMetadataAsync(containerId, cancellationToken)
				.SendAsync()
				.ReceiveHeaders();

			var metadataSerializer = new MetadataSerializer<IContainerMetadata>();

			return new ContainerMetadataCollection(metadataSerializer.ParseMetadataHeaders(requestHeaders));
		}

		/// <inheritdoc cref="ObjectStorageApiBuilder.SaveContainerMetadataAsync(string, ContainerMetadataCollection, CancellationToken)" />
		public async Task SaveContainerMetadataAsync(string containerId, ContainerMetadataCollection metadataCollection, CancellationToken cancellationToken = default(CancellationToken))
		{
			await _objectStorageApiBuilder
				.SaveContainerMetadataAsync(containerId, metadataCollection, cancellationToken)
				.SendAsync();
		}

		/// <inheritdoc cref="ObjectStorageApiBuilder.DeleteContainerAsync(string, CancellationToken)" />
		public async Task<HttpResponseMessage> DeleteContainerAsync(string containerId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return await _objectStorageApiBuilder
				.DeleteContainerAsync(containerId, cancellationToken)
				.SendAsync();
		}
		
		#endregion




		#region Container Objects
		
		/// <inheritdoc cref="ObjectStorageApiBuilder.GetContainerObjectAsync(string, string, CancellationToken)" />
		public async Task<IEnumerable<ContainerObject>> GetContainerObjectAsync(string containerId, string objectPath, CancellationToken cancellationToken = default(CancellationToken))
		{
			return await _objectStorageApiBuilder
				.GetContainerObjectAsync(containerId, objectPath, cancellationToken)
				.SendAsync()
				.ReceiveJson<ContainerObjectCollection>();
		}
		
		/// <inheritdoc cref="ObjectStorageApiBuilder.UpdateContainerObjectAsync(string, string, System.IO.Stream, CancellationToken)" />
		public async Task UpdateContainerObjectAsync(string containerId, string objectPath, System.IO.Stream dataStream, CancellationToken cancellationToken = default(CancellationToken))
		{
			await _objectStorageApiBuilder
				.UpdateContainerObjectAsync(containerId, objectPath, dataStream, cancellationToken)
				.SendAsync();
		}
		
		/// <inheritdoc cref="ObjectStorageApiBuilder.UpdateContainerObjectAsync" />
		public async Task UpdateContainerObjectAsync(string containerId, string objectPath, string filePath, CancellationToken cancellationToken = default(CancellationToken))
		{
			using (var dataStream = new System.IO.FileStream(filePath, FileMode.Open, FileAccess.Read))
			{
				await this.UpdateContainerObjectAsync(containerId, objectPath, dataStream, cancellationToken);
			}
		}
		
		/// <inheritdoc cref="ObjectStorageApiBuilder.DeleteContainerObjectAsync(string, string, CancellationToken)" />
		public async Task DeleteContainerObjectAsync(string containerId, string objectPath, CancellationToken cancellationToken = default(CancellationToken))
		{
			await _objectStorageApiBuilder
				.DeleteContainerObjectAsync(containerId, objectPath, cancellationToken)
				.SendAsync();
		}
		
		/// <inheritdoc cref="ObjectStorageApiBuilder.ReadContainerObjectMetadataAsync(string, string, CancellationToken)" />
		public async Task<ContainerObjectMetadataCollection> ReadContainerObjectMetadataAsync(string containerId, string objectPath, CancellationToken cancellationToken = default(CancellationToken))
		{
			// Note: Object metadata are inside both `request.Content.Headers` and `request.Headers`.

			var request = await _objectStorageApiBuilder
				.ReadContainerObjectMetadataAsync(containerId, objectPath, cancellationToken)
				.SendAsync();

			var requestHeaders = new List<KeyValuePair<string, IEnumerable<string>>>();

			requestHeaders.AddRange(request.Headers.Select(item => item));
			requestHeaders.AddRange(request.Content.Headers.Select(item => item));

			var metadataSerializer = new MetadataSerializer<IContainerObjectMetadata>();

			return new ContainerObjectMetadataCollection(metadataSerializer.ParseMetadataHeaders(requestHeaders));
		}
		
		/// <inheritdoc cref="ObjectStorageApiBuilder.ReadContainerObjectMetadataAsync(string, string, CancellationToken)" />
		public async Task<bool> CheckContainerObjectExistsAsync(string containerId, string objectPath, CancellationToken cancellationToken = default(CancellationToken))
		{
			var request = await _objectStorageApiBuilder
				.ReadContainerObjectMetadataAsync(containerId, objectPath, cancellationToken)
				.SendAsync();

			var statusCode = request.StatusCode;

			return statusCode == HttpStatusCode.OK;
		}
		
		/// <inheritdoc cref="ObjectStorageApiBuilder.SaveContainerObjectMetadataAsync(string, string, ContainerObjectMetadataCollection, CancellationToken)" />
		public async Task SaveContainerObjectMetadataAsync(string containerId, string objectPath, ContainerObjectMetadataCollection metadataCollection, CancellationToken cancellationToken = default(CancellationToken))
		{
			await _objectStorageApiBuilder
				.SaveContainerObjectMetadataAsync(containerId, objectPath, metadataCollection, cancellationToken)
				.SendAsync();
		}

		#endregion
	}
}
