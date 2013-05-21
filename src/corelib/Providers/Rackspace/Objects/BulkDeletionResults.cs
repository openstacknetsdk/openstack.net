using System.Collections.Generic;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects
{
    public class BulkDeletionResults
    {
        public IEnumerable<string> SuccessfulObjects { get; set; }

        public IEnumerable<BulkDeletionFailedObject> FailedObjects { get; set; } 
    }

    public class BulkDeletionFailedObject
    {
        public Status Status { get; set; }

        public string Object { get; set; }
    }
}
