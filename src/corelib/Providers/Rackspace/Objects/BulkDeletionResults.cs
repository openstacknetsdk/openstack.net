using System;
using System.Collections.Generic;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects
{
    public class BulkDeletionResults
    {
        public IEnumerable<string> SuccessfulObjects { get; private set; }

        public IEnumerable<BulkDeletionFailedObject> FailedObjects { get; private set; }

        public BulkDeletionResults(IEnumerable<string> successfulObjects, IEnumerable<BulkDeletionFailedObject> failedObjects)
        {
            if (successfulObjects == null)
                throw new ArgumentNullException("successfulObjects");
            if (failedObjects == null)
                throw new ArgumentNullException("failedObjects");

            SuccessfulObjects = successfulObjects;
            FailedObjects = failedObjects;
        }
    }

    public class BulkDeletionFailedObject
    {
        public Status Status { get; private set; }

        public string Object { get; private set; }

        public BulkDeletionFailedObject(string obj, Status status)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (status == null)
                throw new ArgumentNullException("status");
            if (string.IsNullOrEmpty(obj))
                throw new ArgumentException("obj cannot be empty");

            Object = obj;
            Status = Status;
        }
    }
}
