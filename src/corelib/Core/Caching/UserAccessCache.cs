using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using net.openstack.Core.Domain;

namespace net.openstack.Core.Caching
{
    public class UserAccessCache : ICache<UserAccess>
    {
        private static readonly Lazy<UserAccessCache> _instance = new Lazy<UserAccessCache>(CreateInstance, true);

        private readonly ConcurrentDictionary<string, UserAccess> _tokenCache = new ConcurrentDictionary<string, UserAccess>();

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

    public interface ICache<T>
    {
        T Get(string key, Func<T> refreshCallback, bool forceCacheRefresh = false);
    }
}
