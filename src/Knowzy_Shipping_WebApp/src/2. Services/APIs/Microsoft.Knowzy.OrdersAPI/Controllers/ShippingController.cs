﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Knowzy.OrdersAPI.Data;
using System.Threading.Tasks;

namespace Microsoft.Knowzy.OrdersAPI.Controllers
{
    [Route("api/[controller]")]
    public class ShippingController : Controller
    {
        private IOrdersStore _ordersStore;
        public ShippingController(IOrdersStore ordersStore)
        {
            _ordersStore = ordersStore;
        }
        // GET api/Shippping
        [HttpGet]
        public IEnumerable<Domain.Shipping> Get()
        {
            return _ordersStore.GetShippings();
        }

        // GET api/Shipping/5
        [HttpGet("{orderId}")]
        public Domain.Shipping GetShipping(string orderId)
        {
            return _ordersStore.GetShipping(orderId);
        }

        //POST
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Domain.Shipping order)
        {
            if (order == null)
            {
                return BadRequest();
            }

            var result = await _ordersStore.UpsertAsync(order);

            return CreatedAtRoute("Create", new { id = order.Id }, result);
        }

        // PUT
        [HttpPut("{orderId}")]
        public async Task<IActionResult> Update(string orderId, [FromBody] Domain.Shipping order)
        {
            if (order == null || order.Id != orderId)
            {
                return BadRequest();
            }

            var dbOrder = _ordersStore.GetShipping(orderId);
            if (dbOrder == null)
            {
                return NotFound();
            }

            var result = await _ordersStore.UpsertAsync(order);
            return new NoContentResult();
        }

        [HttpDelete("{orderId}")]
        public async Task<IActionResult> Delete(string orderId)
        {
            await _ordersStore.DeleteOrderAsync(orderId);
            return new NoContentResult();
        }
    }
}
