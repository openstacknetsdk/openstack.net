namespace OpenStack.Services.ObjectStorage.V1
{
    using System;
    using OpenStack.Net;

    /// <summary>
    /// This class provides definitions for extensions to the Object Storage Service V1 which are defined by OpenStack.
    /// </summary>
    /// <remarks>
    /// <para>The service extension definitions provided here may be passed to
    /// <see cref="IExtensibleService{TService}.GetServiceExtension{TExtension}"/> to get an instance of the service
    /// extension.</para>
    /// </remarks>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public static class PredefinedObjectStorageExtensions
    {
        /// <summary>
        /// Gets a service definition for the Extract Archive extension to the OpenStack Object Storage Service.
        /// </summary>
        /// <value>
        /// An <see cref="ObjectStorageServiceExtensionDefinition{TExtension}"/> representing the Extract Archive
        /// extension.
        /// </value>
        /// <seealso cref="IExtractArchiveExtension"/>
        /// <seealso cref="ExtractArchiveExtension"/>
        public static ObjectStorageServiceExtensionDefinition<IExtractArchiveExtension> ExtractArchive
        {
            get
            {
                return ExtractArchiveExtensionDefinition.Instance;
            }
        }

        /// <summary>
        /// Gets a service definition for the Form POST middleware extension to the OpenStack Object Storage Service.
        /// </summary>
        /// <value>
        /// An <see cref="ObjectStorageServiceExtensionDefinition{TExtension}"/> representing the Form POST middleware
        /// extension.
        /// </value>
        /// <seealso cref="IFormPostExtension"/>
        /// <seealso cref="FormPostExtension"/>
        public static ObjectStorageServiceExtensionDefinition<IFormPostExtension> FormPost
        {
            get
            {
                return FormPostExtensionDefinition.Instance;
            }
        }

        /// <summary>
        /// Gets a service definition for the Object Versioning extension to the OpenStack Object Storage Service.
        /// </summary>
        /// <value>
        /// An <see cref="ObjectStorageServiceExtensionDefinition{TExtension}"/> representing the Object Versioning
        /// extension.
        /// </value>
        /// <seealso cref="IObjectVersioningExtension"/>
        /// <seealso cref="ObjectVersioningExtension"/>
        public static ObjectStorageServiceExtensionDefinition<IObjectVersioningExtension> ObjectVersioning
        {
            get
            {
                return ObjectVersioningExtensionDefinition.Instance;
            }
        }

        /// <summary>
        /// This class provides the internal definition for the <see cref="ExtractArchive"/> property.
        /// </summary>
        /// <threadsafety static="true" instance="true"/>
        /// <preliminary/>
        private class ExtractArchiveExtensionDefinition : ObjectStorageServiceExtensionDefinition<IExtractArchiveExtension>
        {
            /// <summary>
            /// A singleton instance of the Extract Archive service extension definition.
            /// </summary>
            public static readonly ExtractArchiveExtensionDefinition Instance = new ExtractArchiveExtensionDefinition();

            /// <summary>
            /// Initializes a new instance of the <see cref="ExtractArchiveExtensionDefinition"/> class.
            /// </summary>
            private ExtractArchiveExtensionDefinition()
            {
            }

            /// <inheritdoc/>
            public override string Name
            {
                get
                {
                    return "Extract Archive";
                }
            }

            /// <inheritdoc/>
            public override IExtractArchiveExtension CreateDefaultInstance(IObjectStorageService service, IHttpApiCallFactory httpApiCallFactory)
            {
                return new ExtractArchiveExtension(service, httpApiCallFactory);
            }
        }

        /// <summary>
        /// This class provides the internal definition for the <see cref="FormPost"/> property.
        /// </summary>
        /// <threadsafety static="true" instance="true"/>
        /// <preliminary/>
        private class FormPostExtensionDefinition : ObjectStorageServiceExtensionDefinition<IFormPostExtension>
        {
            /// <summary>
            /// A singleton instance of the Form POST middleware service extension definition.
            /// </summary>
            public static readonly FormPostExtensionDefinition Instance = new FormPostExtensionDefinition();

            /// <summary>
            /// Initializes a new instance of the <see cref="FormPostExtensionDefinition"/> class.
            /// </summary>
            private FormPostExtensionDefinition()
            {
            }

            /// <inheritdoc/>
            public override string Name
            {
                get
                {
                    return "Form POST";
                }
            }

            /// <inheritdoc/>
            public override IFormPostExtension CreateDefaultInstance(IObjectStorageService service, IHttpApiCallFactory httpApiCallFactory)
            {
#if !PORTABLE || WINRT
                return new FormPostExtension(service, httpApiCallFactory);
#else
                throw new NotSupportedException("The Form POST extension is currently not supported on this platform.");
#endif
            }
        }

        /// <summary>
        /// This class provides the internal definition for the <see cref="ObjectVersioning"/> property.
        /// </summary>
        /// <threadsafety static="true" instance="true"/>
        /// <preliminary/>
        private class ObjectVersioningExtensionDefinition : ObjectStorageServiceExtensionDefinition<IObjectVersioningExtension>
        {
            /// <summary>
            /// A singleton instance of the Object Versioning service extension definition.
            /// </summary>
            public static readonly ObjectVersioningExtensionDefinition Instance = new ObjectVersioningExtensionDefinition();

            /// <summary>
            /// Initializes a new instance of the <see cref="ObjectVersioningExtensionDefinition"/> class.
            /// </summary>
            private ObjectVersioningExtensionDefinition()
            {
            }

            /// <inheritdoc/>
            public override string Name
            {
                get
                {
                    return "Object Versioning";
                }
            }

            /// <inheritdoc/>
            public override IObjectVersioningExtension CreateDefaultInstance(IObjectStorageService service, IHttpApiCallFactory httpApiCallFactory)
            {
                return new ObjectVersioningExtension(service, httpApiCallFactory);
            }
        }
    }
}
