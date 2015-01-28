namespace OpenStack.ObjectModel
{
    using System;
    using System.Collections.Immutable;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// This class provides extension methods for the <see cref="ExtensibleJsonObject"/> class.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public static class ExtensibleJsonObjectExtensions
    {
        /// <summary>
        /// Gets an <see cref="ExtensibleJsonObject"/> with the same type and properties from the input object and the
        /// specified extension data.
        /// </summary>
        /// <remarks>
        /// <para>This method exposes <see cref="ExtensibleJsonObject.WithExtensionDataImpl"/> in a manner that
        /// preserves the static type of the input <paramref name="extensibleObject"/>.</para>
        /// </remarks>
        /// <typeparam name="T">The static type of the input extensible JSON object.</typeparam>
        /// <param name="extensibleObject">The extensible JSON object.</param>
        /// <param name="extensionData">The new extension data for the object.</param>
        /// <returns>
        /// An <see cref="ExtensibleJsonObject"/> which represents the input <paramref name="extensibleObject"/> with
        /// the specified extension data. If <paramref name="extensionData"/> is the same as the existing
        /// <see cref="ExtensibleJsonObject.ExtensionData"/> for the input object, the method may return the
        /// <paramref name="extensibleObject"/> instance itself.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="extensibleObject"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="extensionData"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If a new object cannot be created from the input object in order to set the extension data.
        /// </exception>
        public static T WithExtensionData<T>(this T extensibleObject, ImmutableDictionary<string, JToken> extensionData)
            where T : ExtensibleJsonObject
        {
            if (extensibleObject == null)
                throw new ArgumentNullException("extensibleObject");

            return (T)extensibleObject.WithExtensionDataImpl(extensionData);
        }

        /// <summary>
        /// Gets an <see cref="ExtensibleJsonObject"/> with the same type and properties from the input object and the
        /// extension data updated to include the specified property.
        /// </summary>
        /// <remarks>
        /// <para>This method simplifies the use of
        /// <see cref="WithExtensionData{T}(T, ImmutableDictionary{string, JToken})"/> in cases where a single property
        /// needs to be added or set on an object, and all other extension properties preserved from the input
        /// object.</para>
        /// </remarks>
        /// <typeparam name="T">The static type of the input extensible JSON object.</typeparam>
        /// <param name="extensibleObject">The extensible JSON object.</param>
        /// <param name="extensionProperty">The extension property to add or set for the object.</param>
        /// <returns>
        /// An <see cref="ExtensibleJsonObject"/> which represents the input <paramref name="extensibleObject"/> with
        /// the specified property included in the <see cref="ExtensibleJsonObject.ExtensionData"/> for the object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="extensibleObject"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="extensionProperty"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If a new object cannot be created from the input object in order to set the extension data.
        /// </exception>
        public static T WithExtensionData<T>(this T extensibleObject, JProperty extensionProperty)
            where T : ExtensibleJsonObject
        {
            if (extensibleObject == null)
                throw new ArgumentNullException("extensibleObject");
            if (extensionProperty == null)
                throw new ArgumentNullException("extensionProperty");

            ImmutableDictionary<string, JToken> extensionData = extensibleObject.ExtensionData.SetItem(extensionProperty.Name, extensionProperty.Value);
            return extensibleObject.WithExtensionData(extensionData);
        }

        /// <summary>
        /// Gets an <see cref="ExtensibleJsonObject"/> with the same type and properties from the input object and the
        /// extension data updated to include the specified property.
        /// </summary>
        /// <remarks>
        /// <para>This method simplifies the use of
        /// <see cref="WithExtensionData{T}(T, ImmutableDictionary{string, JToken})"/> in cases where a single property
        /// needs to be added or set on an object, and all other extension properties preserved from the input
        /// object.</para>
        /// </remarks>
        /// <typeparam name="T">The static type of the input extensible JSON object.</typeparam>
        /// <param name="extensibleObject">The extensible JSON object.</param>
        /// <param name="name">The name of the extension property to add or set for the object.</param>
        /// <param name="value">The value of the extension property to add or set for the object.</param>
        /// <returns>
        /// An <see cref="ExtensibleJsonObject"/> which represents the input <paramref name="extensibleObject"/> with
        /// the specified property included in the <see cref="ExtensibleJsonObject.ExtensionData"/> for the object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="extensibleObject"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="name"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para>If <paramref name="name"/> is empty.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If a new object cannot be created from the input object in order to set the extension data.
        /// </exception>
        public static T WithExtensionData<T>(this T extensibleObject, string name, JToken value)
            where T : ExtensibleJsonObject
        {
            if (extensibleObject == null)
                throw new ArgumentNullException("extensibleObject");
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name cannot be empty");

            ImmutableDictionary<string, JToken> extensionData = extensibleObject.ExtensionData.SetItem(name, value);
            return extensibleObject.WithExtensionData(extensionData);
        }
    }
}
