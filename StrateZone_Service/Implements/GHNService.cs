using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StrateZone_Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Service.Implements
{
    public class GHNService : IGHNService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;
        private readonly string _token;
        private readonly string _shopId;

        public GHNService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;

            _apiUrl = config["GHN:ApiUrl"];
            _token = config["GHN:Token"];
            _shopId = config["GHN:ShopId"];
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, string endpoint, object data = null)
        {
            var request = new HttpRequestMessage(method, $"{_apiUrl}{endpoint}");
            request.Headers.Add("token", _token);
            request.Headers.Add("shopId", _shopId);
            
            if (data != null)
            {
                string jsonData = JsonConvert.SerializeObject(data);
                request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            }

            return request;
        }

        public async Task<string> GetProvincesAsync()
        {
            var request = CreateRequest(HttpMethod.Get, "/master-data/province");
            try
            {
                var response = await _httpClient.SendAsync(request);
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}");
                return $"Error: {ex.Message}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return $"Unexpected error: {ex.Message}";
            }
        }

        public async Task<string> GetServicesAsync()
        {
            var request = CreateRequest(HttpMethod.Get, "/v2/shipping-order/available-services");
            try
            {
                var response = await _httpClient.SendAsync(request);
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}");
                return $"Error: {ex.Message}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return $"Unexpected error: {ex.Message}";
            }
        }

        public async Task<string> CreateOrderAsync(object orderData)
        {
            var request = CreateRequest(HttpMethod.Post, "/v2/shipping-order/create", orderData);
            var response = await _httpClient.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

        public Task<string> UpdateOrderAsync(object orderData)
        {
            throw new NotImplementedException();
        }

        public Task<string> CalculaExpectedDeliveryTime()
        {
            throw new NotImplementedException();
        }
    }
}
