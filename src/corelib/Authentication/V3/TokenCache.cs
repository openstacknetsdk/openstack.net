using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using net.openstack.Core.Caching;

namespace OpenStack.Authentication.V3
{
    /// <summary>
    /// Provides a thread-safe cache of <see cref="Token"/> objects. A default shared
    /// instance is available through <see cref="TokenCache.Instance"/>.
    /// </summary>
    public class TokenCache : ICache<Token>
    {
        private static readonly Lazy<TokenCache> _instance = new Lazy<TokenCache>(CreateInstance, true);

        private readonly ConcurrentDictionary<string, Token> _tokenCache = new ConcurrentDictionary<string, Token>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="refreshCallback"></param>
        /// <param name="forceCacheRefresh"></param>
        /// <returns></returns>
        public Token Get(string key, Func<Token> refreshCallback, bool forceCacheRefresh = false)
        {
            Token userAccess;

            if (forceCacheRefresh)
            {
                if (refreshCallback != null)
                {
                    userAccess = _tokenCache.AddOrUpdate(key, k => refreshCallback(), (k, existing) => refreshCallback());
                }
                else
                {
                    Token ignored;
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
                            var pair = new KeyValuePair<string, Token>(key, userAccess);
                            ((ICollection<KeyValuePair<string, Token>>)_tokenCache).Remove(pair);
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
        /// Gets a default instance of <see cref="TokenCache"/>.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static TokenCache Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private static TokenCache CreateInstance()
        {
            return new TokenCache();
        }

        private static bool IsValid(Token userAccess)
        {
            return userAccess != null && !userAccess.IsExpired;
        }
    }
}
