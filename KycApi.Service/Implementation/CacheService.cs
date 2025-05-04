using KycApi.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KycApi.Service.Implementation
{
    public class CacheService : ICacheService
    {
        private readonly Dictionary<string, object> _inMemoryCache = new();

        public T? Get<T>(string key)
        {
            if (_inMemoryCache.ContainsKey(key))
            {
                return (T)_inMemoryCache[key];
            }
            return default(T);
        }

        public void Set<T>(string key, T value)
        {
            if (value != null)
            {
                _inMemoryCache[key] = value;
            }
        }
    }
}