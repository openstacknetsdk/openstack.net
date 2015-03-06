namespace OpenStack.Services.Networking.V2
{
    using OpenStack.Net;
    using ILayer3Extension = Layer3.ILayer3Extension;
    using Layer3Extension = Layer3.Layer3Extension;
    using ILoadBalancerExtension = LoadBalancer.ILoadBalancerExtension;
    using LoadBalancerExtension = LoadBalancer.LoadBalancerExtension;
    using IMeteringExtension = Metering.IMeteringExtension;
    using MeteringExtension = Metering.MeteringExtension;
    using IQuotasExtension = Quotas.IQuotasExtension;
    using QuotasExtension = Quotas.QuotasExtension;
    using ISecurityGroupsExtension = SecurityGroups.ISecurityGroupsExtension;
    using SecurityGroupsExtension = SecurityGroups.SecurityGroupsExtension;

    /// <summary>
    /// This class provides definitions for extensions to the Networking Service V2 which are defined by OpenStack.
    /// </summary>
    /// <remarks>
    /// <para>The service extension definitions provided here may be passed to
    /// <see cref="IExtensibleService{TService}.GetServiceExtension{TExtension}"/> to get an instance of the service
    /// extension.</para>
    /// </remarks>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public static class PredefinedNetworkingExtensions
    {
        /// <summary>
        /// Gets a service definition for the Layer 3 extension to the OpenStack Networking Service.
        /// </summary>
        /// <value>
        /// A <see cref="NetworkingServiceExtensionDefinition{TExtension}"/> representing the Layer 3 extension.
        /// </value>
        /// <seealso cref="ILayer3Extension"/>
        /// <seealso cref="Layer3Extension"/>
        public static NetworkingServiceExtensionDefinition<ILayer3Extension> Layer3
        {
            get
            {
                return Layer3ExtensionDefinition.Instance;
            }
        }

        /// <summary>
        /// Gets a service definition for the Load Balancer extension to the OpenStack Networking Service.
        /// </summary>
        /// <value>
        /// A <see cref="NetworkingServiceExtensionDefinition{TExtension}"/> representing the Layer 3 extension.
        /// </value>
        /// <seealso cref="ILoadBalancerExtension"/>
        /// <seealso cref="LoadBalancerExtension"/>
        public static NetworkingServiceExtensionDefinition<ILoadBalancerExtension> LoadBalancer
        {
            get
            {
                return LoadBalancerExtensionDefinition.Instance;
            }
        }

        /// <summary>
        /// Gets a service definition for the Metering extension to the OpenStack Networking Service.
        /// </summary>
        /// <value>
        /// A <see cref="NetworkingServiceExtensionDefinition{TExtension}"/> representing the Metering extension.
        /// </value>
        /// <seealso cref="IMeteringExtension"/>
        /// <seealso cref="MeteringExtension"/>
        public static NetworkingServiceExtensionDefinition<IMeteringExtension> Metering
        {
            get
            {
                return MeteringExtensionDefinition.Instance;
            }
        }

        /// <summary>
        /// Gets a service definition for the Quotas extension to the OpenStack Networking Service.
        /// </summary>
        /// <value>
        /// A <see cref="NetworkingServiceExtensionDefinition{TExtension}"/> representing the Quotas extension.
        /// </value>
        /// <seealso cref="IQuotasExtension"/>
        /// <seealso cref="QuotasExtension"/>
        public static NetworkingServiceExtensionDefinition<IQuotasExtension> Quotas
        {
            get
            {
                return QuotasExtensionDefinition.Instance;
            }
        }

        /// <summary>
        /// Gets a service definition for the Security Groups extension to the OpenStack Networking Service.
        /// </summary>
        /// <value>
        /// A <see cref="NetworkingServiceExtensionDefinition{TExtension}"/> representing the Security Groups extension.
        /// </value>
        /// <seealso cref="ISecurityGroupsExtension"/>
        /// <seealso cref="SecurityGroupsExtension"/>
        public static NetworkingServiceExtensionDefinition<ISecurityGroupsExtension> SecurityGroups
        {
            get
            {
                return SecurityGroupsExtensionDefinition.Instance;
            }
        }

        /// <summary>
        /// This class provides the internal definition for the <see cref="Layer3"/> property.
        /// </summary>
        /// <threadsafety static="true" instance="true"/>
        /// <preliminary/>
        private class Layer3ExtensionDefinition : NetworkingServiceExtensionDefinition<ILayer3Extension>
        {
            /// <summary>
            /// A singleton instance of the Layer 3 service extension definition.
            /// </summary>
            public static readonly Layer3ExtensionDefinition Instance = new Layer3ExtensionDefinition();

            /// <summary>
            /// Initializes a new instance of the <see cref="Layer3ExtensionDefinition"/> class.
            /// </summary>
            private Layer3ExtensionDefinition()
            {
            }

            /// <inheritdoc/>
            public override string Name
            {
                get
                {
                    return "Layer 3";
                }
            }

            /// <inheritdoc/>
            public override ILayer3Extension CreateDefaultInstance(INetworkingService service, IHttpApiCallFactory httpApiCallFactory)
            {
                return new Layer3Extension(service, httpApiCallFactory);
            }
        }

        /// <summary>
        /// This class provides the internal definition for the <see cref="LoadBalancer"/> property.
        /// </summary>
        /// <threadsafety static="true" instance="true"/>
        /// <preliminary/>
        private class LoadBalancerExtensionDefinition : NetworkingServiceExtensionDefinition<ILoadBalancerExtension>
        {
            /// <summary>
            /// A singleton instance of the Load Balancer service extension definition.
            /// </summary>
            public static readonly LoadBalancerExtensionDefinition Instance = new LoadBalancerExtensionDefinition();

            /// <summary>
            /// Initializes a new instance of the <see cref="LoadBalancerExtensionDefinition"/> class.
            /// </summary>
            private LoadBalancerExtensionDefinition()
            {
            }

            /// <inheritdoc/>
            public override string Name
            {
                get
                {
                    return "Load Balancer";
                }
            }

            /// <inheritdoc/>
            public override ILoadBalancerExtension CreateDefaultInstance(INetworkingService service, IHttpApiCallFactory httpApiCallFactory)
            {
                return new LoadBalancerExtension(service, httpApiCallFactory);
            }
        }

        /// <summary>
        /// This class provides the internal definition for the <see cref="Metering"/> property.
        /// </summary>
        /// <threadsafety static="true" instance="true"/>
        /// <preliminary/>
        private class MeteringExtensionDefinition : NetworkingServiceExtensionDefinition<IMeteringExtension>
        {
            /// <summary>
            /// A singleton instance of the Metering service extension definition.
            /// </summary>
            public static readonly MeteringExtensionDefinition Instance = new MeteringExtensionDefinition();

            /// <summary>
            /// Initializes a new instance of the <see cref="MeteringExtensionDefinition"/> class.
            /// </summary>
            private MeteringExtensionDefinition()
            {
            }

            /// <inheritdoc/>
            public override string Name
            {
                get
                {
                    return "Metering";
                }
            }

            /// <inheritdoc/>
            public override IMeteringExtension CreateDefaultInstance(INetworkingService service, IHttpApiCallFactory httpApiCallFactory)
            {
                return new MeteringExtension(service, httpApiCallFactory);
            }
        }

        /// <summary>
        /// This class provides the internal definition for the <see cref="Quotas"/> property.
        /// </summary>
        /// <threadsafety static="true" instance="true"/>
        /// <preliminary/>
        private class QuotasExtensionDefinition : NetworkingServiceExtensionDefinition<IQuotasExtension>
        {
            /// <summary>
            /// A singleton instance of the Quotas service extension definition.
            /// </summary>
            public static readonly QuotasExtensionDefinition Instance = new QuotasExtensionDefinition();

            /// <summary>
            /// Initializes a new instance of the <see cref="QuotasExtensionDefinition"/> class.
            /// </summary>
            private QuotasExtensionDefinition()
            {
            }

            /// <inheritdoc/>
            public override string Name
            {
                get
                {
                    return "Quotas";
                }
            }

            /// <inheritdoc/>
            public override IQuotasExtension CreateDefaultInstance(INetworkingService service, IHttpApiCallFactory httpApiCallFactory)
            {
                return new QuotasExtension(service, httpApiCallFactory);
            }
        }

        /// <summary>
        /// This class provides the internal definition for the <see cref="SecurityGroups"/> property.
        /// </summary>
        /// <threadsafety static="true" instance="true"/>
        /// <preliminary/>
        private class SecurityGroupsExtensionDefinition : NetworkingServiceExtensionDefinition<ISecurityGroupsExtension>
        {
            /// <summary>
            /// A singleton instance of the Security Groups service extension definition.
            /// </summary>
            public static readonly SecurityGroupsExtensionDefinition Instance = new SecurityGroupsExtensionDefinition();

            /// <summary>
            /// Initializes a new instance of the <see cref="SecurityGroupsExtensionDefinition"/> class.
            /// </summary>
            private SecurityGroupsExtensionDefinition()
            {
            }

            /// <inheritdoc/>
            public override string Name
            {
                get
                {
                    return "SecurityGroups";
                }
            }

            /// <inheritdoc/>
            public override ISecurityGroupsExtension CreateDefaultInstance(INetworkingService service, IHttpApiCallFactory httpApiCallFactory)
            {
                return new SecurityGroupsExtension(service, httpApiCallFactory);
            }
        }
    }
}
