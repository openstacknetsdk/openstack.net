using System;
using System.Collections;
using net.openstack.Core.Domain;

namespace net.openstack
{
    public class IdentityTokenCache : ICache<IdentityToken>
    {
        private readonly Hashtable _tokenCache = new Hashtable();
        private readonly object _tokenLock = new object();

        public IdentityToken Get(string key, Func<IdentityToken> refreshCallback)
        {
            IdentityToken token = null;

            lock (_tokenLock)
            {
                //var hashKey = BuildTokenCacheKey(username, region);
                var refreshToken = true;
                if (_tokenCache.ContainsKey(key))
                {
                    token = (IdentityToken)_tokenCache[key];

                    if (token.IsExpired())
                        _tokenCache.Remove(key);
                    else
                        refreshToken = false;
                }

                if (refreshToken && refreshCallback != null)
                {
                    token = refreshCallback();
                    if (token != null)
                    {
                        _tokenCache.Add(key, token);
                    }
                }
            }

            if (token == null)
                return null;

            return token;
        }
    }

    public interface ICache<T>
    {
        T Get(string key, Func<IdentityToken> refreshCallback);
    }
}
