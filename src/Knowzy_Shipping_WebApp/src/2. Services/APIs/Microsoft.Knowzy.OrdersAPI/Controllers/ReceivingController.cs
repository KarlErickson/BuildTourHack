using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Knowzy.OrdersAPI.Data;

namespace Microsoft.Knowzy.OrdersAPI.Controllers
{
    [Route("api/[controller]")]
    public class ReceivingController : Controller
    {
        private IOrdersStore _ordersStore;
        public ReceivingController(IOrdersStore ordersStore)
        {
            _ordersStore = ordersStore;
        }

        // GET api/Shippping
        [HttpGet]
        public IEnumerable<Domain.Receiving> Get()
        {
            return _ordersStore.GetReceivings();
        }

        // GET api/Receiving/5
        [HttpGet("{orderId}")]
        public Domain.Receiving GetReceiving(string orderId)
        {
            return _ordersStore.GetReceiving(orderId);
        }

        //POST
        [HttpPost]
        public IActionResult Create([FromBody] Domain.Receiving order)
        {
            if (order == null)
            {
                return BadRequest();
            }

            _ordersStore.UpsertOrder(order);

            return CreatedAtRoute("Create", new { id = order.Id }, order);
        }

        // PUT
        [HttpPut("{orderId}")]
        public IActionResult Update(string orderId, [FromBody] Domain.Receiving order)
        {
            if (order == null || order.Id != orderId)
            {
                return BadRequest();
            }

            var dbOrder = _ordersStore.GetReceiving(orderId);
            if (dbOrder == null)
            {
                return NotFound();
            }

            _ordersStore.UpsertOrder(order);
            return new NoContentResult();
        }
    }
}
