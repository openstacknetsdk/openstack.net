namespace OpenStack.Net
{
    using System;
    using System.Collections.Generic;
    using OpenStack.Collections;
    using Rackspace.Net;
    using BitArray = System.Collections.BitArray;
    using Encoding = System.Text.Encoding;
    using MatchEvaluator = System.Text.RegularExpressions.MatchEvaluator;
    using NumberStyles = System.Globalization.NumberStyles;
    using Regex = System.Text.RegularExpressions.Regex;
    using RegexOptions = System.Text.RegularExpressions.RegexOptions;
    using StringBuilder = System.Text.StringBuilder;

    /// <summary>
    /// Provides static utility methods for encoding and decoding text within
    /// RFC 3986 URIs.
    /// </summary>
    /// <seealso href="http://www.ietf.org/rfc/rfc3986">RFC 3986: URI Generic Syntax</seealso>
    /// <threadsafety static="true" instance="false"/>
    /// <preliminary/>
    public static class UriUtility
    {
        private static readonly RegexOptions DefaultRegexOptions;

        /// <summary>
        /// A regular expression pattern for the RFC 6570 <c>pct-encoded</c> rule.
        /// </summary>
        private const string PctEncodedPatternString = @"(?:%[a-fA-F0-9]{2})";

        /// <summary>
        /// A regular expression pattern for the RFC 6570 <c>varchar</c> rule.
        /// </summary>
        private const string VarCharPatternString = @"(?:[a-zA-Z0-9_]|" + PctEncodedPatternString + @")";

        /// <summary>
        /// A regular expression pattern for the RFC 6570 <c>varname</c> rule.
        /// </summary>
        private const string VarNamePatternString = @"(?:" + VarCharPatternString + @"(?:\.?" + VarCharPatternString + @")*)";

        private static readonly BitArray _unreservedCharacters;
        private static readonly BitArray _generalDelimiters;
        private static readonly BitArray _subDelimiters;
        private static readonly BitArray _reservedCharacters;
        private static readonly BitArray _allowedHostCharacters;
        private static readonly BitArray _allowedPathCharacters;
        private static readonly BitArray _allowedQueryCharacters;
        private static readonly BitArray _allowedFragmentCharacters;

        /// <summary>
        /// This is the regular expression for the RFC 6570 <c>varname</c> rule.
        /// </summary>
        private static readonly Regex VarName = new Regex("^" + VarNamePatternString + "$", DefaultRegexOptions);

        /// <summary>
        /// This is the regular expression for a single percent-encoded character.
        /// </summary>
        private static readonly Regex _percentEncodedPattern = new Regex(PctEncodedPatternString, DefaultRegexOptions);

        static UriUtility()
        {
#if PORTABLE
            if (!Enum.TryParse("Compiled", out DefaultRegexOptions))
                DefaultRegexOptions = RegexOptions.None;
#else
            DefaultRegexOptions = RegexOptions.Compiled;
#endif

            _unreservedCharacters = new BitArray(256);
            for (char i = 'a'; i <= 'z'; i++)
                _unreservedCharacters.Set(i, true);
            for (char i = 'A'; i <= 'Z'; i++)
                _unreservedCharacters.Set(i, true);
            for (char i = '0'; i <= '9'; i++)
                _unreservedCharacters.Set(i, true);
            _unreservedCharacters.Set('-', true);
            _unreservedCharacters.Set('.', true);
            _unreservedCharacters.Set('_', true);
            _unreservedCharacters.Set('~', true);

            _generalDelimiters = new BitArray(256);
            _generalDelimiters.Set(':', true);
            _generalDelimiters.Set('/', true);
            _generalDelimiters.Set('?', true);
            _generalDelimiters.Set('#', true);
            _generalDelimiters.Set('[', true);
            _generalDelimiters.Set(']', true);
            _generalDelimiters.Set('@', true);

            _subDelimiters = new BitArray(256);
            _subDelimiters.Set('!', true);
            _subDelimiters.Set('$', true);
            _subDelimiters.Set('&', true);
            _subDelimiters.Set('(', true);
            _subDelimiters.Set(')', true);
            _subDelimiters.Set('*', true);
            _subDelimiters.Set('+', true);
            _subDelimiters.Set(',', true);
            _subDelimiters.Set(';', true);
            _subDelimiters.Set('=', true);
            _subDelimiters.Set('\'', true);

            _reservedCharacters = new BitArray(256).Or(_generalDelimiters).Or(_subDelimiters);

            _allowedHostCharacters = new BitArray(256).Or(_unreservedCharacters).Or(_subDelimiters);

            _allowedPathCharacters = new BitArray(256).Or(_unreservedCharacters).Or(_subDelimiters);
            _allowedPathCharacters.Set(':', true);
            _allowedPathCharacters.Set('@', true);

            _allowedQueryCharacters = new BitArray(256).Or(_allowedPathCharacters);
            _allowedQueryCharacters.Set('/', true);
            _allowedQueryCharacters.Set('?', true);

            _allowedFragmentCharacters = new BitArray(256).Or(_allowedPathCharacters);
            _allowedFragmentCharacters.Set('/', true);
            _allowedFragmentCharacters.Set('?', true);
        }

        /// <summary>
        /// Add a query parameter with the specified name and value to a URI.
        /// </summary>
        /// <remarks>
        /// <para>This method always adds a new query parameter to the end of the query string, even
        /// if the URI already contains one or more query parameters with the specified parameter
        /// name.</para>
        ///
        /// <note>
        /// <para>If necessary, characters in the <paramref name="parameter"/> name are percent-encoded
        /// to conform to the requirements of a <c>varname</c> defined by RFC 6570.</para>
        /// </note>
        /// </remarks>
        /// <param name="uri">The URI to add a query parameter to.</param>
        /// <param name="parameter">The name of the query parameter to add.</param>
        /// <param name="value">The value of the query parameter.</param>
        /// <returns>A <see cref="Uri"/> representing the input <paramref name="uri"/> with the specified query parameter added to the end of the query string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="uri"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="parameter"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="value"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">If <paramref name="parameter"/> is empty.</exception>
        public static Uri AddQueryParameter(Uri uri, string parameter, string value)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");
            if (parameter == null)
                throw new ArgumentNullException("parameter");
            if (value == null)
                throw new ArgumentNullException("value");
            if (string.IsNullOrEmpty(parameter))
                throw new ArgumentException("parameter cannot be empty");

            string originalQuery = uri.Query;

            if (!VarName.IsMatch(parameter))
            {
                // convert the parameter into a form compatible with RFC 6570
                parameter = UriDecode(parameter);
                StringBuilder builder = new StringBuilder();

                byte[] bytes = Encoding.UTF8.GetBytes(parameter);
                for (int i = 0; i < bytes.Length; i++)
                {
                    byte b = bytes[i];
                    if ((b >= 'a' && b <= 'z')
                        || (b >= 'A' && b <= 'Z')
                        || (b >= '0' && b <= '9')
                        || b == '_'
                        || (i > 0 && i < bytes.Length - 1 && b == '.'))
                    {
                        builder.Append((char)b);
                    }
                    else
                    {
                        builder.Append('%').Append(b.ToString("X2"));
                    }
                }

                parameter = builder.ToString();
            }

            UriTemplate queryTemplate;
            if (string.IsNullOrEmpty(originalQuery))
            {
                // URI does not already contain query parameters
                queryTemplate = new UriTemplate("{?" + parameter + "}");
            }
            else
            {
                // URI already contains query parameters
                queryTemplate = new UriTemplate("{&" + parameter + "}");
            }

            var parameters = new Dictionary<string, string> { { parameter, value } };
            Uri queryUri = queryTemplate.BindByName(parameters);
            UriKind uriKind = uri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative;
            return new Uri(uri.OriginalString + queryUri.OriginalString, uriKind);
        }

        /// <summary>
        /// Sets a URI query parameter to a specific value, adding or replacing the existing value
        /// as appropriate.
        /// </summary>
        /// <remarks>
        /// <para>This method is a convenience method for calling <see cref="RemoveQueryParameter"/> followed
        /// by <see cref="AddQueryParameter"/>.</para>
        /// </remarks>
        /// <param name="uri">The URI to add a query parameter to.</param>
        /// <param name="parameter">The name of the query parameter to add or update.</param>
        /// <param name="value">The value of the query parameter.</param>
        /// <returns>
        /// A <see cref="Uri"/> representing the input <paramref name="uri"/> with all previous instances
        /// (if any) of the query parameter with the specified name removed, and a new query parameter
        /// with the specified name and value added to the query string.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="uri"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="parameter"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="value"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">If <paramref name="parameter"/> is empty.</exception>
        public static Uri SetQueryParameter(Uri uri, string parameter, string value)
        {
            return AddQueryParameter(RemoveQueryParameter(uri, parameter), parameter, value);
        }

        /// <summary>
        /// Remove all query parameters matching a particular parameter name from a URI.
        /// </summary>
        /// <remarks>
        /// <para>This method handles percent-encoded octets as UTF-8 encoded characters in both the
        /// <paramref name="parameter"/> argument and the query string.</para>
        /// </remarks>
        /// <param name="uri">The URI.</param>
        /// <param name="parameter">The name of the parameter to remove.</param>
        /// <returns>A <see cref="Uri"/> representing the input <paramref name="uri"/> with all query parameters with a name matching <paramref name="parameter"/> removed from the query string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para>If <paramref name="uri"/> is <see langword="null"/>.</para>
        /// <para>-or-</para>
        /// <para>If <paramref name="parameter"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">If <paramref name="parameter"/> is empty.</exception>
        public static Uri RemoveQueryParameter(Uri uri, string parameter)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");
            if (parameter == null)
                throw new ArgumentNullException("parameter");
            if (string.IsNullOrEmpty(parameter))
                throw new ArgumentException("parameter cannot be empty");

            string query = uri.Query;
            if (string.IsNullOrEmpty(query))
                return uri;

            StringBuilder expressionBuilder = new StringBuilder();
            byte[] encodedParameter = Encoding.UTF8.GetBytes(UriDecode(parameter));
            expressionBuilder.Append("[?&]");
            foreach (byte b in encodedParameter)
            {
                string escapedUpper = b.ToString("X2");
                expressionBuilder.Append("(?:");
                if (b == '&' || b == '?' || b == '=')
                {
                    // these character can only appear percent-encoded in a parameter name
                    expressionBuilder.Append('%');
                }
                else
                {
                    expressionBuilder.Append(Regex.Escape(((char)b).ToString()));
                    expressionBuilder.Append("|%");
                }

                foreach (var escaped in escapedUpper)
                {
                    if (escaped != char.ToLowerInvariant(escaped))
                        expressionBuilder.Append('[').Append(escaped).Append(char.ToLowerInvariant(escaped)).Append(']');
                    else
                        expressionBuilder.Append(escaped);
                }

                expressionBuilder.Append(")");
            }

            expressionBuilder.Append("=[^?&]*");

            Regex expression = new Regex(expressionBuilder.ToString());
            UriBuilder uriBuilder = new UriBuilder(uri);
            string modifiedQuery = expression.Replace(uriBuilder.Query, string.Empty);

            // setting the UriBuilder.Query property adds a leading `?` even if one is already present in `modifiedQuery`
            if (!string.IsNullOrEmpty(modifiedQuery))
                modifiedQuery = modifiedQuery.Substring(1);

            uriBuilder.Query = modifiedQuery;
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Decodes the text of a URI by unescaping any percent-encoded character sequences and
        /// then evaluating the result using the default <see cref="Encoding.UTF8"/> encoding.
        /// </summary>
        /// <remarks>
        /// <para>This method calls <see cref="UriDecode(string, Encoding)"/> using the default
        /// <see cref="Encoding.UTF8"/> encoding.</para>
        /// </remarks>
        /// <param name="text">The encoded URI.</param>
        /// <returns>The decoded URI text.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> is <see langword="null"/>.</exception>
        public static string UriDecode(string text)
        {
            return UriDecode(text, Encoding.UTF8);
        }

        /// <summary>
        /// Decodes the text of a URI by unescaping any percent-encoded character sequences and
        /// then evaluating the result using the specified encoding.
        /// </summary>
        /// <param name="text">The encoded URI.</param>
        /// <param name="encoding">The encoding to use for Unicode characters in the URI. If this value is <see langword="null"/>, the <see cref="Encoding.UTF8"/> encoding will be used.</param>
        /// <returns>The decoded URI text.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> is <see langword="null"/>.</exception>
        public static string UriDecode(string text, Encoding encoding)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            encoding = encoding ?? Encoding.UTF8;
            MatchEvaluator matchEvaluator =
                match =>
                {
                    string hexValue = match.Value.Substring(1);
                    return ((char)byte.Parse(hexValue, NumberStyles.HexNumber)).ToString();
                };
            string decodedText = _percentEncodedPattern.Replace(text, matchEvaluator);
            byte[] data = decodedText.ToCharArray().ConvertAll(c => (byte)c);
            return encoding.GetString(data, 0, data.Length);
        }

        /// <summary>
        /// Encodes text for inclusion in a URI using the <see cref="Encoding.UTF8"/> encoding.
        /// </summary>
        /// <remarks>
        /// <para>This method calls <see cref="UriEncode(string, UriPart, Encoding)"/> using the default
        /// <see cref="Encoding.UTF8"/> encoding.</para>
        /// </remarks>
        /// <param name="text">The text to encode for inclusion in a URI.</param>
        /// <param name="uriPart">A <see cref="UriPart"/> value indicating where in the URI the specified text will be included.</param>
        /// <returns>The URI-encoded text, suitable for the specified URI part.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="uriPart"/> is not a valid <see cref="UriPart"/>.</exception>
        public static string UriEncode(string text, UriPart uriPart)
        {
            return UriEncode(text, uriPart, Encoding.UTF8);
        }

        /// <summary>
        /// Encodes text for inclusion in a URI.
        /// </summary>
        /// <param name="text">The text to encode for inclusion in a URI.</param>
        /// <param name="uriPart">A <see cref="UriPart"/> value indicating where in the URI the specified text will be included.</param>
        /// <param name="encoding">The encoding to use for Unicode characters in the URI. If this value is <see langword="null"/>, the <see cref="Encoding.UTF8"/> encoding will be used.</param>
        /// <returns>The URI-encoded text, suitable for the specified URI part.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="text"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="uriPart"/> is not a valid <see cref="UriPart"/>.</exception>
        public static string UriEncode(string text, UriPart uriPart, Encoding encoding)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            encoding = encoding ?? Encoding.UTF8;
            switch (uriPart)
            {
            case UriPart.Any:
                return UriEncodeAny(text, encoding);

            case UriPart.AnyUrl:
                return UriEncodeAnyUrl(text, encoding);

            case UriPart.Host:
                return UriEncodeHost(text, encoding);

            case UriPart.Path:
                return UriEncodePath(text, encoding);

            case UriPart.PathSegment:
                return UriEncodePathSegment(text, encoding);

            case UriPart.Query:
                return UriEncodeQuery(text, encoding);

            case UriPart.QueryValue:
                return UriEncodeQueryValue(text, encoding);

            case UriPart.Fragment:
                return UriEncodeFragment(text, encoding);

            default:
                throw new ArgumentException("The specified uriPart is not valid.", "uriPart");
            }
        }

        private static string UriEncodeAny(string text, Encoding encoding)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            if (text.Length == 0)
                return text;

            StringBuilder builder = new StringBuilder();
            byte[] data = encoding.GetBytes(text);
            for (int i = 0; i < data.Length; i++)
            {
                if (_unreservedCharacters[data[i]])
                    builder.Append((char)data[i]);
                else
                    builder.Append('%').Append(data[i].ToString("x2"));
            }

            return builder.ToString();
        }

        private static string UriEncodeAnyUrl(string text, Encoding encoding)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            if (text.Length == 0)
                return text;

            StringBuilder builder = new StringBuilder();
            byte[] data = encoding.GetBytes(text);
            for (int i = 0; i < data.Length; i++)
            {
                if (_unreservedCharacters[data[i]])
                {
                    builder.Append((char)data[i]);
                }
                else
                {
                    switch ((char)data[i])
                    {
                    case '(':
                    case ')':
                    case '*':
                    case '!':
                        builder.Append((char)data[i]);
                        break;

                    default:
                        builder.Append('%').Append(data[i].ToString("x2"));
                        break;
                    }
                }
            }

            return builder.ToString();
        }

        private static string UriEncodeHost(string text, Encoding encoding)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            if (text.Length == 0)
                return text;

            StringBuilder builder = new StringBuilder();
            byte[] data = encoding.GetBytes(text);
            for (int i = 0; i < data.Length; i++)
            {
                if (_allowedHostCharacters[data[i]])
                    builder.Append((char)data[i]);
                else
                    builder.Append('%').Append(data[i].ToString("x2"));
            }

            return builder.ToString();
        }

        private static string UriEncodePath(string text, Encoding encoding)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            if (text.Length == 0)
                return text;

            StringBuilder builder = new StringBuilder();
            byte[] data = encoding.GetBytes(text);
            for (int i = 0; i < data.Length; i++)
            {
                if (_allowedPathCharacters[data[i]] || data[i] == '/')
                    builder.Append((char)data[i]);
                else
                    builder.Append('%').Append(data[i].ToString("x2"));
            }

            return builder.ToString();
        }

        private static string UriEncodePathSegment(string text, Encoding encoding)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            if (text.Length == 0)
                return text;

            StringBuilder builder = new StringBuilder();
            byte[] data = encoding.GetBytes(text);
            for (int i = 0; i < data.Length; i++)
            {
                if (_allowedPathCharacters[data[i]])
                    builder.Append((char)data[i]);
                else
                    builder.Append('%').Append(data[i].ToString("x2"));
            }

            return builder.ToString();
        }

        private static string UriEncodeQuery(string text, Encoding encoding)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            if (text.Length == 0)
                return text;

            StringBuilder builder = new StringBuilder();
            byte[] data = encoding.GetBytes(text);
            for (int i = 0; i < data.Length; i++)
            {
                if (_allowedQueryCharacters[data[i]])
                    builder.Append((char)data[i]);
                else
                    builder.Append('%').Append(data[i].ToString("x2"));
            }

            return builder.ToString();
        }

        private static string UriEncodeQueryValue(string text, Encoding encoding)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            if (text.Length == 0)
                return text;

            StringBuilder builder = new StringBuilder();
            byte[] data = encoding.GetBytes(text);
            for (int i = 0; i < data.Length; i++)
            {
                if (_allowedQueryCharacters[data[i]] && data[i] != '&')
                    builder.Append((char)data[i]);
                else
                    builder.Append('%').Append(data[i].ToString("x2"));
            }

            return builder.ToString();
        }

        private static string UriEncodeFragment(string text, Encoding encoding)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            if (text.Length == 0)
                return text;

            StringBuilder builder = new StringBuilder();
            byte[] data = encoding.GetBytes(text);
            for (int i = 0; i < data.Length; i++)
            {
                if (_allowedFragmentCharacters[data[i]])
                    builder.Append((char)data[i]);
                else
                    builder.Append('%').Append(data[i].ToString("x2"));
            }

            return builder.ToString();
        }
    }
}
