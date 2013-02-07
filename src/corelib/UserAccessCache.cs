using System;
using System.Collections;
using net.openstack.Core.Domain;

namespace net.openstack
{
    public class UserAccessCache : ICache<UserAccess>
    {
        private readonly Hashtable _tokenCache = new Hashtable();
        private readonly object _tokenLock = new object();

        public UserAccess Get(string key, Func<UserAccess> refreshCallback, bool forceCacheRefresh = false)
        {
            UserAccess userAccess = null;

            lock (_tokenLock)
            {
                //var hashKey = BuildTokenCacheKey(username, region);
                var refreshToken = true;
                if (_tokenCache.ContainsKey(key))
                {
                    if (forceCacheRefresh)
                    {
                        _tokenCache.Remove(key);
                    }
                    else
                    {
                        userAccess = (UserAccess) _tokenCache[key];

                        if (userAccess == null || userAccess.Token == null || userAccess.Token.IsExpired())
                            _tokenCache.Remove(key);
                        else
                            refreshToken = false;
                    }
                }

                if ((refreshToken && refreshCallback != null))
                {
                    userAccess = refreshCallback();
                    if (userAccess != null)
                    {
                        _tokenCache.Add(key, userAccess);
                    }
                }
            }

            return userAccess;
        }

        private static volatile UserAccessCache _instance;
        private static object _contextLock = new object();

        public static UserAccessCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_contextLock)
                    {
                        if(_instance == null)
                            _instance = new UserAccessCache();
                    }
                }

                return _instance;
            }
        }
    }

    public interface ICache<T>
    {
        T Get(string key, Func<T> refreshCallback, bool forceCacheRefresh = false);
    }
}
