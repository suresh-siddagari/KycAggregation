using KycApi.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace KycApi.Service.Implementation
{
    public class DataService : IDataService
    {
        private readonly HttpClient _httpClient;
        private readonly ICacheService _cacheService;
        private readonly bool _enableCache = true;//TODO: move to app.settings.json

        public DataService(HttpClient httpClient, ICacheService cacheService)
        {
            _httpClient = httpClient;
            _cacheService = cacheService;
        }

        // get data if cache enabled + available in cache. Otherwise, make API call and cache the data    
        public async Task<T?> GetData<T>(string url)
        {
            var cacheKey = url.GetHashCode().ToString();

            if (_enableCache)
            {
                var cachedData = _cacheService.Get<T>(cacheKey);
                if (cachedData != null)
                {
                    return cachedData;
                }
            }

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<T>();
                if (_enableCache)
                {
                    _cacheService.Set(cacheKey, data);
                }

                return data;
            }
            return default;
        }
    }
}