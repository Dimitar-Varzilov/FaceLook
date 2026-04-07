using FaceLook.Services.Interfaces;

namespace FaceLook.Services.Core
{
    public class CacheService : ICacheService
    {
        private readonly Dictionary<string, object> _cache = [];

        public void Add(string key, object data)
        {
            _cache.Add(key, data);
        }

        public T? Get<T>(string key) where T : class
        {
            return _cache[key] as T;
        }

        public bool TryGet<T>(string key, out T? data)
        {
            if (_cache.TryGetValue(key, out object? dataInner))
            {
                data = (T)dataInner;
                return true;
            }
            data = default; 
            return false;
        }
    }
}
