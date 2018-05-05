using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using UCB.JapanCimzia.API.Messages;

namespace UCB.JapanCimzia.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        [HttpGet]
        [Route()]
        public async Task<IList<OrderMessage>> GetOrders()
        {
            var orders = new List<OrderMessage>
            {
                new OrderMessage { Id = 1, Customer = new CustomerMessage { FirstName ="Customer 1" }, Merchand = "Merchand 1"},
                new OrderMessage { Id = 2, Customer = new CustomerMessage { FirstName ="Customer 2" }, Merchand = "Merchand 2"}
            };

            return await Task.FromResult(orders);
        }

        [HttpPost]
        [Route()]
        public async Task AddOrder(OrderMessage orderMessage)
        {
            //save order in db
            await Task.FromResult(true);
        }

        [HttpDelete]
        [Route("{orderId}")]
        public async Task DeleteOrder(int orderId)
        {
            //save order in db
            await Task.FromResult(true);
        }

        [HttpPut]
        [Route("{orderId}")]
        public async Task<OrderMessage> UpdateOrder(int orderId, OrderMessage orderMessage)
        {
            //update order in db
            return await Task.FromResult(orderMessage);
        }
    }
}
