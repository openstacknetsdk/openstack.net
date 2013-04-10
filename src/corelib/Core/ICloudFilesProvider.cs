using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JSIStudios.SimpleRESTServices.Client;
using net.openstack.Core.Domain;

namespace net.openstack.Core
{
    public interface ICloudFilesProvider
    {
        #region Container

        IEnumerable<Container> ListContainers(int? limit = null, string markerId = null, string markerEnd = null, string format = "json", string region = null, CloudIdentity identity = null);
        ObjectStore CreateContainer(string container, string region = null, CloudIdentity identity = null);
        ObjectStore DeleteContainer(string container, string region = null, CloudIdentity identity = null);
        Dictionary<string, string> GetContainerHeader(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);
        Dictionary<string, string> GetContainerMetaData(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);
        ContainerCDN GetContainerCDNHeader(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        IEnumerable<ContainerCDN> ListCDNContainers(int? limit = null, string markerId = null, string markerEnd = null, bool cdnEnabled = false, string region = null, CloudIdentity identity = null);

        Dictionary<string, string> EnableCDNOnContainer(string container, long ttl, string region = null, CloudIdentity identity = null);
        Dictionary<string, string> EnableCDNOnContainer(string container, bool logRetention, string region = null, CloudIdentity identity = null);
        Dictionary<string, string> EnableCDNOnContainer(string container, long ttl, bool logRetention, string region = null, CloudIdentity identity = null);

        Dictionary<string, string> DisableCDNOnContainer(string container, string region = null, CloudIdentity identity = null);


        void UpdateContainerMetadata(string container, Dictionary<string, string> metadata, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);
        void AddContainerHeaders(string container, Dictionary<string, string> headers, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);
        void UpdateContainerCdnHeaders(string container, Dictionary<string, string> headers, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        void EnableStaticWebOnContainer(string container, string index, string error, string css, bool listing, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);
        void EnableStaticWebOnContainer(string container, string index, string error, bool listing, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);
        void EnableStaticWebOnContainer(string container, string css, bool listing, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);
        void EnableStaticWebOnContainer(string container, string index, string error, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);
        void DisableStaticWebOnContainer(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);
        #endregion

        #region Container Objects

        IEnumerable<ContainerObject> ListObjects(string container, int? limit = null, string markerId = null, string markerEnd = null, string format = "json", string region = null, CloudIdentity identity = null);
        void CreateObjectFromFile(string container, string filePath, string objectName, int chunkSize = 65536, Dictionary<string, string> headers = null, string region = null, Action<long> progressUpdated = null, CloudIdentity identity = null);
        void CreateObject(string container, Stream stream, string objectName, int chunkSize = 65536, Dictionary<string, string> headers = null, string region = null, Action<long> progressUpdated = null, CloudIdentity identity = null);
        void GetObjectSaveToFile(string container, string saveDirectory, string objectName, string fileName = null, int chunkSize = 65536, Dictionary<string, string> headers = null, string region = null, bool verifyEtag = false, Action<long> progressUpdated = null, CloudIdentity identity = null);
        void GetObject(string container, string objectName, Stream outputStream, int chunkSize = 65536, Dictionary<string, string> headers = null, string region = null, bool verifyEtag = false, Action<long> progressUpdated = null, CloudIdentity identity = null);
        ObjectStore DeleteObject(string container, string objectNmae, Dictionary<string, string> headers = null, string region = null, CloudIdentity identity = null);
        ObjectStore CopyObject(string sourceContainer, string sourceObjectName, string destinationContainer, string destinationObjectName, Dictionary<string, string> headers = null, string region = null, CloudIdentity identity = null);

        Dictionary<string, string> GetObjectHeaders(string container, string objectName, string format = "json", string region = null, CloudIdentity identity = null);
        Dictionary<string, string> GetObjectMetaData(string container, string objectName, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        ObjectStore PurgeObjectFromCDN(string container, string objectName, string region = null, CloudIdentity identity = null);
        ObjectStore PurgeObjectFromCDN(string container, string objectName, string email, string region = null, CloudIdentity identity = null);
        ObjectStore PurgeObjectFromCDN(string container, string objectName, string[] emails, string region = null, CloudIdentity identity = null);
        #endregion

        #region Accounts

        Dictionary<string, string> GetAccountHeaders(string region = null, bool useInternalUrl = false, CloudIdentity identity = null);
        Dictionary<string, string> GetAccountMetaData(string region = null, bool useInternalUrl = false, CloudIdentity identity = null);
        void UpdateAccountMetadata(Dictionary<string, string> metadata, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);
        void UpdateAccountHeaders(Dictionary<string, string> headers, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        #endregion
    }
}
