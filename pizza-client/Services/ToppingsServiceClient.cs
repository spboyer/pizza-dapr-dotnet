using pizza_client.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace pizza_client.Services
{
    public class ToppingsServiceClient : IToppingsServiceClient
    {
        private readonly HttpClient _httpClient;

        public ToppingsServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IEnumerable<Topping>> GetToppings() 
        {
            var response = await _httpClient.GetAsync("toppings");
            response.EnsureSuccessStatusCode();

            using var items = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<IEnumerable<Topping>>(items);
        }
    }
}
