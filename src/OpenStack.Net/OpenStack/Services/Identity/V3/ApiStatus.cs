﻿namespace OpenStack.Services.Identity.V3
{
    using System;
    using System.Collections.Concurrent;
    using Newtonsoft.Json;
    using OpenStack.ObjectModel;

    /// <summary>
    /// Represents the status of an API version.
    /// </summary>
    /// <remarks>
    /// This class functions as a strongly-typed enumeration of known statuses,
    /// with added support for unknown statuses supported by a server extension.
    /// </remarks>
    /// <seealso cref="DatabaseInstance.Status"/>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    [JsonConverter(typeof(ApiStatus.Converter))]
    public sealed class ApiStatus : ExtensibleEnum<ApiStatus>
    {
        private static readonly ConcurrentDictionary<string, ApiStatus> _types =
            new ConcurrentDictionary<string, ApiStatus>(StringComparer.OrdinalIgnoreCase);
        private static readonly ApiStatus _experimental = FromName("experimental");
        private static readonly ApiStatus _stable = FromName("stable");

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiStatus"/> class with the specified name.
        /// </summary>
        /// <inheritdoc/>
        private ApiStatus(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets the <see cref="ApiStatus"/> instance with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The unique <see cref="ApiStatus"/> instance with the specified name.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is empty.</exception>
        public static ApiStatus FromName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            return _types.GetOrAdd(name, i => new ApiStatus(i));
        }

        /// <summary>
        /// Gets an <see cref="ApiStatus"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static ApiStatus Experimental
        {
            get
            {
                return _experimental;
            }
        }

        /// <summary>
        /// Gets an <see cref="ApiStatus"/> representing <placeholder>placeholder</placeholder>.
        /// </summary>
        public static ApiStatus Stable
        {
            get
            {
                return _stable;
            }
        }

        /// <summary>
        /// Provides support for serializing and deserializing <see cref="ApiStatus"/>
        /// objects to JSON string values.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        /// <preliminary/>
        private sealed class Converter : ConverterBase
        {
            /// <inheritdoc/>
            protected override ApiStatus FromName(string name)
            {
                return ApiStatus.FromName(name);
            }
        }
    }
}
