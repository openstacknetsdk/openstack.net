using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.openstack.Core.Domain;

namespace net.openstack.Core
{
    public interface IObjectStoreProvider
    {
        #region Container

        IEnumerable<Container> ListContainers(CloudIdentity identity, int? limit = null, string markerId = null, string markerEnd = null, string format = "json", string region = null);
        ObjectStore CreateContainer(CloudIdentity identity, string container, string region = null);
        ObjectStore DeleteContainer(CloudIdentity identity, string container, string region = null);

        #endregion

        #region Container Objects

        IEnumerable<ContainerObject> GetObjects(CloudIdentity identity, string containerName, int? limit = null, string markerId = null, string markerEnd = null, string format = "json", string region = null);

        #endregion
    }
}
