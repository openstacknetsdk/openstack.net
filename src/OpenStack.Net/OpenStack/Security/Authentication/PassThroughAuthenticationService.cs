namespace OpenStack.Security.Authentication
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Rackspace.Threading;

    /// <summary>
    /// This class defines an <see cref="IAuthenticationService"/> which passes HTTP requests through unaltered.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public class PassThroughAuthenticationService : IAuthenticationService
    {
        /// <summary>
        /// This is the backing field for the <see cref="BaseAddress"/> property.
        /// </summary>
        private readonly Uri _baseAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="PassThroughAuthenticationService"/> class with the specified
        /// fixed base address.
        /// </summary>
        /// <param name="baseAddress">The base address to return from <see cref="GetBaseAddressAsync"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="baseAddress"/> is <see langword="null"/>.
        /// </exception>
        public PassThroughAuthenticationService(Uri baseAddress)
        {
            if (baseAddress == null)
                throw new ArgumentNullException("baseAddress");

            _baseAddress = baseAddress;
        }

        /// <summary>
        /// Gets the base address to use for services requested by calls to <see cref="GetBaseAddressAsync"/>.
        /// </summary>
        /// <value>
        /// The base address to use for services requested by calls to <see cref="GetBaseAddressAsync"/>.
        /// </value>
        protected Uri BaseAddress
        {
            get
            {
                return _baseAddress;
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This authentication service does not perform any authentication operations or modify the
        /// <paramref name="requestMessage"/>.
        /// </remarks>
        public virtual Task AuthenticateRequestAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            if (requestMessage == null)
                throw new ArgumentNullException("requestMessage");

            return CompletedTask.Default;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// The base implementation for this authentication service simply returns <see cref="BaseAddress"/>.
        /// </remarks>
        public virtual Task<Uri> GetBaseAddressAsync(string serviceType, string serviceName, string region, bool internalAddress, CancellationToken cancellationToken)
        {
            return CompletedTask.FromResult(BaseAddress);
        }
    }
}
