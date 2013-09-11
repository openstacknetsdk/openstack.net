using System;
using System.Collections.Generic;
using net.openstack.Core.Domain;

namespace net.openstack.Providers.Rackspace.Objects
{
    /// <summary>
    /// Represents the detailed results of a bulk deletion operation.
    /// </summary>
    [Serializable]
    public class BulkDeletionResults
    {
        /// <summary>
        /// Gets a collection objects which were successfully deleted.
        /// </summary>
        public IEnumerable<string> SuccessfulObjects { get; private set; }

        /// <summary>
        /// Gets a collection of <see cref="BulkDeletionFailedObject"/> objects providing
        /// the name and status of objects which could not be deleted during the bulk
        /// deletion operation.
        /// </summary>
        public IEnumerable<BulkDeletionFailedObject> FailedObjects { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkDeletionResults"/> class
        /// with the specified collections of successful and failed objects.
        /// </summary>
        /// <param name="successfulObjects">The objects which were successfully deleted.</param>
        /// <param name="failedObjects">The objects which could not be deleted.</param>
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

    /// <summary>
    /// Describes an object which could not be deleted by a bulk deletion operation,
    /// along with a status providing the reason why the deletion failed.
    /// </summary>
    [Serializable]
    public class BulkDeletionFailedObject
    {
        /// <summary>
        /// Gets a <see cref="Status"/> object describing the reason the object
        /// could not be deleted.
        /// </summary>
        public Status Status { get; private set; }

        /// <summary>
        /// Gets the name of the object which could not be deleted.
        /// </summary>
        public string Object { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkDeletionFailedObject"/> class
        /// with the specified object name and status.
        /// </summary>
        /// <param name="obj">The name of the object which could not be deleted.</param>
        /// <param name="status">A <see cref="Status"/> object describing the reason the object could not be deleted.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="obj"/> is <c>null</c>.
        /// <para>-or-</para>
        /// <para>If <paramref name="status"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">If <paramref name="obj"/> is empty.</exception>
        public BulkDeletionFailedObject(string obj, Status status)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (status == null)
                throw new ArgumentNullException("status");
            if (string.IsNullOrEmpty(obj))
                throw new ArgumentException("obj cannot be empty");

            Object = obj;
            Status = status;
        }
    }
}
