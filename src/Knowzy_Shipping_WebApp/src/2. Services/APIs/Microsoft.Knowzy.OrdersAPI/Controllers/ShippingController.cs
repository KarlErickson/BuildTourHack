﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Knowzy.OrdersAPI.Data;

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
        public IActionResult Create([FromBody] Domain.Shipping order)
        {
            if (order == null)
            {
                return BadRequest();
            }

            _ordersStore.CreateOrder(order);

            return CreatedAtRoute("Create", new { id = order.Id }, order);
        }

        // PUT
        [HttpPut("{orderId}")]
        public IActionResult Update(string orderId, [FromBody] Domain.Shipping order)
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

            _ordersStore.UpdateOrder(order);
            return new NoContentResult();
        }
    }
}
