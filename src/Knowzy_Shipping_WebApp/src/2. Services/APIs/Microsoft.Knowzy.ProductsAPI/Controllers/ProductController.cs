using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Knowzy.ProductsAPI.Data;
using System.Threading.Tasks;
using Microsoft.Knowzy.Domain;

namespace Microsoft.Knowzy.ProductsAPI.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private IProductsStore _ProductsStore;
        public ProductController(IProductsStore ProductsStore)
        {
            _ProductsStore = ProductsStore;
        }
        // GET api/Product
        [HttpGet]
        public IEnumerable<Product> Get()
        {
            return _ProductsStore.GetProducts();
        }

        // GET api/Product/5
        [HttpGet("{ProductId}")]
        public Product GetProduct(string ProductId)
        {
            return _ProductsStore.GetProduct(ProductId);
        }

        //POST
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Product Product)
        {
            if (Product == null)
            {
                return BadRequest();
            }

            var result = await _ProductsStore.UpsertAsync(Product);

            return CreatedAtRoute("Create", new { id = Product.Id }, result);
        }
    }
}
