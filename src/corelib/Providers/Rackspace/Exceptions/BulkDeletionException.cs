using System;
using net.openstack.Providers.Rackspace.Objects;

namespace net.openstack.Providers.Rackspace.Exceptions
{
    public class BulkDeletionException : Exception
    {
        public BulkDeletionResults Results { get; private set; }
        
        public BulkDeletionException(string status, BulkDeletionResults results)
            : base(string.Format("The bulk deletion operation did not complete successfully. Status code: {0}", status))
        {
            Results = results;
        }
    }
}
