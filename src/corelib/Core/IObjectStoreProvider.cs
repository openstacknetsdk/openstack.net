using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSIStudios.SimpleRESTServices.Client;
using net.openstack.Core.Domain;

namespace net.openstack.Core
{
    public interface IObjectStoreProvider
    {
        #region Container

        IEnumerable<Container> ListContainers(CloudIdentity identity, int? limit = null, string markerId = null, string markerEnd = null, string format = "json", string region = null);
        ObjectStore CreateContainer(CloudIdentity identity, string container, string region = null);
        ObjectStore DeleteContainer(CloudIdentity identity, string container, string region = null);
        Dictionary<string, string> GetHeaderForContainer(CloudIdentity identity, string container, string region = null, bool useInternalUrl = false);
        Dictionary<string, string> GetMetaDataForContainer(CloudIdentity identity, string container, string region = null, bool useInternalUrl = false);
        Dictionary<string, string> GetCDNHeaderForContainer(CloudIdentity identity, string container, string region = null, bool useInternalUrl = false);

        IEnumerable<ContainerCDN> ListCDNContainers(CloudIdentity identity, int? limit = null, string markerId = null, string markerEnd = null, bool cdnEnabled = false, string region = null);

        Dictionary<string, string> EnableCDNOnContainer(CloudIdentity identity, string container, long ttl, string region = null);
        Dictionary<string, string> EnableCDNOnContainer(CloudIdentity identity, string container, bool logRetention, string region = null);
        Dictionary<string, string> EnableCDNOnContainer(CloudIdentity identity, string container, long ttl, bool logRetention, string region = null);

        Dictionary<string, string> DisableCDNOnContainer(CloudIdentity identity, string container, string region = null);


        void AddContainerMetadata(CloudIdentity identity, string container, Dictionary<string, string> metadata, string region = null, bool useInternalUrl = false);
        void AddContainerHeaders(CloudIdentity identity, string container, Dictionary<string, string> headers, string region = null, bool useInternalUrl = false);
        void AddContainerCdnHeaders(CloudIdentity identity, string container, Dictionary<string, string> headers, string region = null, bool useInternalUrl = false);

        #endregion

        #region Container Objects

        IEnumerable<ContainerObject> GetObjects(CloudIdentity identity, string container, int? limit = null, string markerId = null, string markerEnd = null, string format = "json", string region = null);
        IEnumerable<HttpHeader> GetObjectHeaders(CloudIdentity identity, string container, string objectName, string format = "json", string region = null);

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
