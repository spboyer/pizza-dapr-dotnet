using pizza_client.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pizza_client.Services
{
    public interface IToppingsServiceClient
    {
        public Task<IEnumerable<Topping>> GetToppings();
    }
}