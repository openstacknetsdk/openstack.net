namespace OpenStack.Services.ObjectStorage.V1
{
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the detailed information for a container stored in the Object Storage Service.
    /// </summary>
    /// <seealso cref="IObjectStorageService"/>
    /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/GET_showAccountDetails__v1__account__storage_account_services.html">Show account details and list containers (OpenStack Object Storage API V1 Reference)</seealso>
    /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/GET_listcontainers_v1__account__accountServicesOperations_d1e000.html">Show Account Details and List Containers (Rackspace Cloud Files Developer Guide - API v1)</seealso>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonObject(MemberSerialization.OptIn)]
    public class Container : ExtensibleJsonObject
    {
#pragma warning disable 649 // Field 'fieldName' is never assigned to, and will always have its default value {value}
        /// <summary>
        /// This is the backing field for the <see cref="Name"/> property.
        /// </summary>
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ContainerName _name;

        /// <summary>
        /// This is the backing field for the <see cref="ObjectCount"/> property.
        /// </summary>
        [JsonProperty("count", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private int? _count;

        /// <summary>
        /// This is the backing field for the <see cref="Size"/> property.
        /// </summary>
        [JsonProperty("bytes", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private long? _bytes;
#pragma warning restore 649

        /// <summary>
        /// Initializes a new instance of the <see cref="Container"/> class
        /// during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected Container()
        {
        }

        /// <summary>
        /// Gets the name of the container.
        /// <token>OpenStackNotDefined</token>
        /// </summary>
        /// <value>
        /// <para>The name of the container.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/GET_showAccountDetails__v1__account__storage_account_services.html">Show account details and list containers (OpenStack Object Storage API V1 Reference)</seealso>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/GET_listcontainers_v1__account__accountServicesOperations_d1e000.html">Show Account Details and List Containers (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        public ContainerName Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Gets the number of objects in the container.
        /// <token>OpenStackNotDefined</token>
        /// </summary>
        /// <remarks>
        /// <para>
        /// This field is <see href="http://en.wikipedia.org/wiki/Eventual_consistency">eventually consistent</see>.
        /// </para>
        /// </remarks>
        /// <value>
        /// <para>The number of objects in the container.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/GET_showAccountDetails__v1__account__storage_account_services.html">Show account details and list containers (OpenStack Object Storage API V1 Reference)</seealso>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/GET_listcontainers_v1__account__accountServicesOperations_d1e000.html">Show Account Details and List Containers (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        public int? ObjectCount
        {
            get
            {
                return _count;
            }
        }

        /// <summary>
        /// Gets the total space utilized by the objects in the container.
        /// <token>OpenStackNotDefined</token>
        /// </summary>
        /// <remarks>
        /// <para>
        /// This field is <see href="http://en.wikipedia.org/wiki/Eventual_consistency">eventually consistent</see>.
        /// </para>
        /// </remarks>
        /// <value>
        /// <para>The total space utilized by the objects in the container.</para>
        /// <token>NullIfNotIncluded</token>
        /// </value>
        /// <seealso href="http://docs.openstack.org/api/openstack-object-storage/1.0/content/GET_showAccountDetails__v1__account__storage_account_services.html">Show account details and list containers (OpenStack Object Storage API V1 Reference)</seealso>
        /// <seealso href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/GET_listcontainers_v1__account__accountServicesOperations_d1e000.html">Show Account Details and List Containers (Rackspace Cloud Files Developer Guide - API v1)</seealso>
        public long? Size
        {
            get
            {
                return _bytes;
            }
        }
    }
}
