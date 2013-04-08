using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JSIStudios.SimpleRESTServices.Client;
using net.openstack.Core.Domain;

namespace net.openstack.Core
{
    public interface IObjectStoreProvider
    {
        #region Container

        IEnumerable<Container> ListContainers(int? limit = null, string markerId = null, string markerEnd = null, string format = "json", string region = null, CloudIdentity identity = null);
        ObjectStore CreateContainer(string container, string region = null, CloudIdentity identity = null);
        ObjectStore DeleteContainer(string container, string region = null, CloudIdentity identity = null);
        Dictionary<string, string> GetContainerHeader(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);
        Dictionary<string, string> GetContainerMetaData(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);
        Dictionary<string, string> GetContainerCDNHeader(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        IEnumerable<ContainerCDN> ListCDNContainers(int? limit = null, string markerId = null, string markerEnd = null, bool cdnEnabled = false, string region = null, CloudIdentity identity = null);

        Dictionary<string, string> EnableCDNOnContainer(string container, long ttl, string region = null, CloudIdentity identity = null);
        Dictionary<string, string> EnableCDNOnContainer(string container, bool logRetention, string region = null, CloudIdentity identity = null);
        Dictionary<string, string> EnableCDNOnContainer(string container, long ttl, bool logRetention, string region = null, CloudIdentity identity = null);

        Dictionary<string, string> DisableCDNOnContainer(string container, string region = null, CloudIdentity identity = null);


        void AddContainerMetadata(string container, Dictionary<string, string> metadata, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);
        void AddContainerHeaders(string container, Dictionary<string, string> headers, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);
        void AddContainerCdnHeaders(string container, Dictionary<string, string> headers, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);

        void EnableStaticWebOnContainer(string container, string index, string error, string css, bool listing, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);
        void EnableStaticWebOnContainer(string container, string index, string error, bool listing, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);
        void EnableStaticWebOnContainer(string container, string css, bool listing, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);
        void EnableStaticWebOnContainer(string container, string index, string error, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);
        void DisableStaticWebOnContainer(string container, string region = null, bool useInternalUrl = false, CloudIdentity identity = null);
        #endregion

        #region Container Objects

        IEnumerable<ContainerObject> GetObjects(string container, int? limit = null, string markerId = null, string markerEnd = null, string format = "json", string region = null, CloudIdentity identity = null);
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

        //string Name { get; }
        //string ObjectVersionLocation { get; }
        //string WebIndex { get; }
        //string WebError { get; }
        //string WebCSS { get; }
        //Dictionary<string, string> Headers { get; }
        //Dictionary<string, string> CdnHeaders { get; }
        //Dictionary<string, string> Metadata { get; }
        //int Retries { get; set; }
        //long ObjectCount { get; }
        //long BytesUsed { get; }
        //long TTL { get; }
        //Uri StorageUrl { get; }
        //Uri CdnManagementUrl { get; }
        //Uri CdnUri { get; }
        //Uri CdnSslUri { get; }
        //Uri CdnStreamingUri { get; }
        //bool CdnEnabled { get; }
        //bool CdnLogRetention { get; }
        //bool WebListingEnabled { get; }
        //StorageObject CreateObject(string object_name);
        //StorageObject GetObject(string object_name);
        //List<StorageObject> GetObjects();
        //List<StorageObject> GetObjects(bool full_listing);
        //List<StorageObject> GetObjects(Dictionary<ObjectQuery, string> query);
        //List<StorageObject> GetObjects(bool full_listing, Dictionary<ObjectQuery, string> query);
        //List<Dictionary<string, string>> GetObjectList();
        //List<Dictionary<string, string>> GetObjectList(bool full_listing);
        //List<Dictionary<string, string>> GetObjectList(Dictionary<ObjectQuery, string> query);
        //List<Dictionary<string, string>> GetObjectList(bool full_listing, Dictionary<ObjectQuery, string> query);
        //void DeleteObject(string object_name);
        //void AddMetadata(Dictionary<string, string> metadata); -- DONE
        //void AddHeaders(Dictionary<string, string> headers); -- DONE
        //void AddCdnHeaders(Dictionary<string, string> headers); -- DONE
        //void EnableStaticWeb(string index, string error, string css, bool listing);
        //void EnableStaticWeb(string index, string error, bool listing);
        //void EnableStaticWeb(string css, bool listing);
        //void EnableStaticWeb(string index, string error);
        //void DisableStaticWeb();
        //void EnableObjectVersioning(string container);
        //void DisableObjectVersioning();
        //void MakePublic();
        //void MakePublic(long ttl);
        //void MakePublic(bool log_retention);
        //void MakePublic(long ttl, bool log_retention);
        //void MakePrivate();
        //void SetTTL(long ttl);
        //void SetCdnLogRetention(bool log_retention);
    }
}
