using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Knowzy.OrdersAPI.Data;
using System.Threading.Tasks;
using Microsoft.Knowzy.Domain;

namespace Microsoft.Knowzy.OrdersAPI.Controllers
{
    [Route("api/[controller]")]
    public class PostalCarrierController : Controller
    {
        private IOrdersStore _ordersStore;
        public PostalCarrierController(IOrdersStore ordersStore)
        {
            _ordersStore = ordersStore;
        }
        // GET api/PostalCarrier
        [HttpGet]
        public IEnumerable<PostalCarrier> Get()
        {
            return _ordersStore.GetPostalCarriers();
        }
    }
}
