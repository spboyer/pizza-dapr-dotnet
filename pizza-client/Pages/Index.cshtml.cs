using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using pizza_client.Model;
using pizza_client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace pizza_client.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IToppingsServiceClient _toppingsServiceClient;

        public IndexModel(ILogger<IndexModel> logger, IToppingsServiceClient toppingsServiceClient)
        {
            _toppingsServiceClient = toppingsServiceClient;
            _logger = logger;
        }

        public async Task OnGet()
        {
            var toppings = await _toppingsServiceClient.GetToppings();
            ViewData["Toppings"] = toppings;

            //var httpClient = DaprClient.CreateInvokeHttpClient();

            //var toppings = await httpClient.GetAsync("http://toppings-api/toppings");
            //toppings.EnsureSuccessStatusCode();

            //using var responseStream = await toppings.Content.ReadAsStreamAsync();
            //ViewData["Toppings"] = await JsonSerializer.DeserializeAsync<IEnumerable<Topping>>(responseStream);

            //var toppings = await _daprClient
            //    .InvokeMethodAsync<IEnumerable<Model.Topping>>(HttpMethod.Get,"toppings-api","toppings");

            //ViewData["Toppings"] = toppings;
        }
    }
}
