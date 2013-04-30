using System.Collections.Generic;
using JSIStudios.SimpleRESTServices.Client;

namespace net.openstack.Core
{
    public interface IObjectStorageMetadataProcessor
    {
        Dictionary<string, Dictionary<string, string>> ProcessMetadata(IList<HttpHeader> httpHeaders);
    }
}
