namespace OpenStack.Collections
{
    using System;

    /// <summary>
    /// Provides extension methods to support consistent <see cref="Array"/> API operations
    /// across multiple versions of the .NET framework.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    internal static class ArrayExtensions
    {
#if PORTABLE
        /// <summary>
        /// Converts an array of one type to an array of another type.
        /// </summary>
        /// <typeparam name="TInput">The type of the elements of the source array.</typeparam>
        /// <typeparam name="TOutput">The type of the elements of the target array.</typeparam>
        /// <param name="array">The one-dimensional, zero-based <see cref="Array"/> to convert to a target type.</param>
        /// <param name="converter">A <see cref="Func{TInput, TOutput}"/> that converts each element from one type to another type.</param>
        /// <returns>An array of the target type containing the converted elements from the source array.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="array"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="converter"/> is <see langword="null"/>.</para>
        /// </exception>
        public static TOutput[] ConvertAll<TInput, TOutput>(this TInput[] array, Func<TInput, TOutput> converter)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (converter == null)
                throw new ArgumentNullException("converter");

            TOutput[] result = new TOutput[array.Length];
            for (int i = 0; i < array.Length; i++)
                result[i] = converter(array[i]);

            return result;
        }
#else
        /// <summary>
        /// Converts an array of one type to an array of another type.
        /// </summary>
        /// <typeparam name="TInput">The type of the elements of the source array.</typeparam>
        /// <typeparam name="TOutput">The type of the elements of the target array.</typeparam>
        /// <param name="array">The one-dimensional, zero-based <see cref="Array"/> to convert to a target type.</param>
        /// <param name="converter">A <see cref="Converter{TInput, TOutput}"/> that converts each element from one type to another type.</param>
        /// <returns>An array of the target type containing the converted elements from the source array.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="array"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="converter"/> is <see langword="null"/>.</para>
        /// </exception>
        public static TOutput[] ConvertAll<TInput, TOutput>(this TInput[] array, Converter<TInput, TOutput> converter)
        {
            return Array.ConvertAll(array, converter);
        }
#endif
    }
}
