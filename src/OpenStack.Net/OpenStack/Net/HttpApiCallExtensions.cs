namespace OpenStack.Net
{
    using System;

    /// <summary>
    /// This class provides extension methods for <see cref="IHttpApiCall{T}"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public static class HttpApiCallExtensions
    {
        /// <summary>
        /// Returns the input typed as <see cref="IHttpApiCall{T}"/>.
        /// </summary>
        /// <remarks>
        /// <para>The <see cref="AsHttpApiCall{T}"/> method has no effect other than to change the compile-time type of
        /// <paramref name="apiCall"/> from a type that implements <see cref="IHttpApiCall{T}"/> to
        /// <see cref="IHttpApiCall{T}"/> itself.</para>
        /// </remarks>
        /// <typeparam name="T">The type returned by the HTTP API call.</typeparam>
        /// <param name="apiCall">The API call to type as <see cref="IHttpApiCall{T}"/>.</param>
        /// <returns>The input API call typed as <see cref="IHttpApiCall{T}"/>.</returns>
        public static IHttpApiCall<T> AsHttpApiCall<T>(this IHttpApiCall<T> apiCall)
        {
            return apiCall;
        }

        /// <summary>
        /// Projects the result of an HTTP API call into a new form.
        /// </summary>
        /// <typeparam name="TSource">The type of object returned by the HTTP API call.</typeparam>
        /// <typeparam name="TResult">The type of object returned by <paramref name="selector"/>.</typeparam>
        /// <param name="source">The input HTTP API call to transform.</param>
        /// <param name="selector">A transform function to apply to the result of <paramref name="source"/>.</param>
        /// <returns>An <see cref="IHttpApiCall{T}"/> whose result is the result of invoking the transform function on
        /// the result of <paramref name="source"/>.</returns>
        public static IHttpApiCall<TResult> Select<TSource, TResult>(this IHttpApiCall<TSource> source, Func<TSource, TResult> selector)
        {
            return new DelegatingHttpApiCall<TSource, TResult>(source, selector);
        }
    }
}
