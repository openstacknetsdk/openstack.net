﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using net.openstack.Core.Domain;

namespace net.openstack.Core.Caching
{
    /// <summary>
    /// Provides a thread-safe cache of <see cref="UserAccess"/> objects. A default shared
    /// instance is available through <see cref="UserAccessCache.Instance"/>.
    /// </summary>
    public class UserAccessCache : ICache<UserAccess>
    {
        private static readonly Lazy<UserAccessCache> _instance = new Lazy<UserAccessCache>(CreateInstance, true);

        private readonly ConcurrentDictionary<string, UserAccess> _tokenCache = new ConcurrentDictionary<string, UserAccess>();

        /// <summary>
        /// Gets a <see cref="UserAccess"/> cached for a particular key, updating the value if necessary.
        /// </summary>
        /// <remarks>
        /// This method returns a previously cached <see cref="UserAccess"/> when possible. If any
        /// of the following conditions are met, the <paramref name="refreshCallback"/> function
        /// will be called to obtain a new value for <paramref name="key"/> which is then added to
        /// the cache, replacing any previously cached value.
        /// 
        /// <list type="bullet">
        /// <item>The cache does not contain any value associated with <paramref name="key"/>.</item>
        /// <item><paramref name="forceCacheRefresh"/> is <c>true</c>.</item>
        /// <item>The previously cached <see cref="UserAccess"/> for <paramref name="key"/> has expired
        /// (see <see cref="IdentityToken.IsExpired()"/>).</item>
        /// </list>
        ///
        /// <para>If any of the above conditions is met and <paramref name="refreshCallback"/> is <c>null</c>,
        /// the previously cached value for <paramref name="key"/>, if any, is removed from the cache
        /// and the method returns <c>null</c>.</para>
        /// </remarks>
        /// <param name="key">The key.</param>
        /// <param name="refreshCallback">A function which returns a new value for the specified <paramref name="key"/>,
        /// or <c>null</c> if no update function available (see remarks). This function may perform a synchronous
        /// authentication to an <see cref="IIdentityProvider"/>.</param>
        /// <param name="forceCacheRefresh">If <c>true</c>, the value associated with <paramref name="key"/> will be always be refreshed by calling <paramref name="refreshCallback"/>, even if a value is already cached.</param>
        /// <returns>
        /// The cached <see cref="UserAccess"/> associated with the specified <paramref name="key"/>.
        /// If no cached value is available (see remarks), the method returns <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        public UserAccess Get(string key, Func<UserAccess> refreshCallback, bool forceCacheRefresh = false)
        {
            UserAccess userAccess;

            if (forceCacheRefresh)
            {
                if (refreshCallback != null)
                {
                    userAccess = _tokenCache.AddOrUpdate(key, k => refreshCallback(), (k, existing) => refreshCallback());
                }
                else
                {
                    UserAccess ignored;
                    _tokenCache.TryRemove(key, out ignored);
                    userAccess = null;
                }
            }
            else
            {
                if (_tokenCache.TryGetValue(key, out userAccess))
                {
                    if (!IsValid(userAccess))
                    {
                        bool remove = true;
                        if (refreshCallback != null)
                        {
                            /* Attempt to update the key. Call IsValid on existing in case another thread
                             * performed a concurrent update and the result is now valid.
                             */
                            userAccess = _tokenCache.AddOrUpdate(key, k => refreshCallback(), (k, existing) => IsValid(existing) ? existing : refreshCallback());
                            remove = userAccess == null;
                        }

                        if (remove)
                        {
                            /* only remove the key if it was updated to null or no refresh callback was given,
                             * and make sure to check the value before actually removing the entry.
                             * see: http://blogs.msdn.com/b/pfxteam/archive/2011/04/02/10149222.aspx
                             */
                            var pair = new KeyValuePair<string, UserAccess>(key, userAccess);
                            ((ICollection<KeyValuePair<string, UserAccess>>)_tokenCache).Remove(pair);
                        }
                    }
                }
                else
                {
                    userAccess = _tokenCache.AddOrUpdate(key, k => refreshCallback(), (k, existing) => refreshCallback());
                }
            }

            return userAccess;
        }

        /// <summary>
        /// Gets a default instance of <see cref="UserAccessCache"/>.
        /// </summary>
        public static UserAccessCache Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private static UserAccessCache CreateInstance()
        {
            return new UserAccessCache();
        }

        private static bool IsValid(UserAccess userAccess)
        {
            IdentityToken token = userAccess != null ? userAccess.Token : null;
            return token != null && !token.IsExpired();
        }
    }

    /// <summary>
    /// Represents a thread-safe cache of objects identified by string keys.
    /// </summary>
    /// <typeparam name="T">Type type of objects stored in the cache.</typeparam>
    public interface ICache<T>
    {
        /// <summary>
        /// Gets a value cached for a particular key, updating the value if necessary.
        /// </summary>
        /// <remarks>
        /// This method returns a previously cached value when possible. If any of the following
        /// conditions are met, the <paramref name="refreshCallback"/> function will be called to
        /// obtain a new value for <paramref name="key"/> which is then added to the cache,
        /// replacing any previously cached value.
        /// 
        /// <list type="bullet">
        /// <item>The cache does not contain any value associated with <paramref name="key"/>.</item>
        /// <item><paramref name="forceCacheRefresh"/> is <c>true</c>.</item>
        /// <item>The previously cached value for <paramref name="key"/> is no longer valid. The exact
        /// algorithm for determining whether or not a value is valid in implementation-defined.</item>
        /// </list>
        ///
        /// <para>If any of the above conditions is met and <paramref name="refreshCallback"/> is <c>null</c>,
        /// the previously cached value for <paramref name="key"/>, if any, is removed from the cache
        /// and the default value for <typeparamref name="T"/> is returned.</para>
        /// </remarks>
        /// <param name="key">The key.</param>
        /// <param name="refreshCallback">A function which returns a new value for the specified <paramref name="key"/>, or <c>null</c> if no update function available (see remarks).</param>
        /// <param name="forceCacheRefresh">If <c>true</c>, the value associated with <paramref name="key"/> will be always be refreshed by calling <paramref name="refreshCallback"/>, even if a value is already cached.</param>
        /// <returns>
        /// The cached value associated with the specified <paramref name="key"/>. If no cached value is
        /// available (see remarks), the method returns the default value for <typeparamref name="T"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
        T Get(string key, Func<T> refreshCallback, bool forceCacheRefresh = false);
    }
}
