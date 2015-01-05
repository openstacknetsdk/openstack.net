namespace OpenStack.Net
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides extension methods to support consistent <see cref="Uri"/> API operations
    /// across multiple versions of the .NET framework.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    internal static class UriExtensions
    {
        /// <summary>
        /// Gets an array of individual segments of the <see cref="Uri.AbsolutePath"/> of a <see cref="Uri"/>.
        /// </summary>
        /// <remarks>
        /// The array returned by this method has the following properties.
        /// <list type="bullet">
        /// <item>The first item in the array is the root segment <c>/</c>.</item>
        /// <item>Each item in the array <em>except</em> the last ends with a trailing <c>/</c>.</item>
        /// <item>The last item in the array only contains a trailing <c>/</c> if the absolute path of the URI ends with a <c>/</c>.</item>
        /// </list>
        /// </remarks>
        /// <param name="uri">The <see cref="Uri"/> instance.</param>
        /// <returns>The individual path segments of the URI (see remarks).</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="uri"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="uri"/> is a relative URI.</exception>
        public static string[] GetSegments(this Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            string path = uri.AbsolutePath;
            List<string> segments = new List<string>();
            int index = -1;
            while (true)
            {
                int previous = index;
                index = path.IndexOf('/', index + 1);
                if (index == -1)
                {
                    if (previous < path.Length - 1)
                        segments.Add(path.Substring(previous + 1, path.Length - previous - 1));
                    break;
                }

                segments.Add(path.Substring(previous + 1, index - previous));
            }

            return segments.ToArray();
        }
    }
}
