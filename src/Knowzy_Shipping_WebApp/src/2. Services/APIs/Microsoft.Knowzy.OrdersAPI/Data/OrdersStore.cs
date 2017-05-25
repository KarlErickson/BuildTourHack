using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Documents.Client;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Knowzy.OrdersAPI.Data
{
    public class OrdersStore : IOrdersStore
    {
        private readonly DocumentClient _client;
        private Uri _ordersLink;
        private FeedOptions _options = new FeedOptions();

        public OrdersStore(IConfiguration config)
        {
            var EndpointUri = config["COSMOSDB_ENDPOINT"];
            var PrimaryKey = config["COSMOSDB_KEY"];
            _client = new DocumentClient(new Uri(EndpointUri), PrimaryKey);
            //Make sure the below values match your set up
            _ordersLink = UriFactory.CreateDocumentCollectionUri("knowzydb", "orders");
            _options.EnableCrossPartitionQuery = true;
        }

        public async Task<bool> Connected()
        {
            try
            {
                var db = await _client.GetDatabaseAccountAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<Domain.Shipping> GetShippings()
        {
            var orders = _client.CreateDocumentQuery<Domain.Shipping>(
                _ordersLink,
                "SELECT * FROM orders o WHERE o.type='shipping'",
                _options).ToList();

            if (orders != null && orders.Count() > 0)
                return orders;
            else
                return null;
        }

        public Domain.Shipping GetShipping(string orderId)
        {
            var orders = _client.CreateDocumentQuery<Domain.Shipping>(
                _ordersLink,
                $"SELECT * FROM orders o WHERE o.id='{orderId}'",
                _options).ToList();

            if (orders != null && orders.Count() > 0)
                return orders.First();
            else
                return null;
        }

        public async void CreateOrder(Domain.Order order)
        {
             await _client.CreateDocumentAsync(_ordersLink.ToString(), order);
        }

        public async void UpdateOrder(Domain.Order order)
        {
            await _client.UpsertDocumentAsync(_ordersLink.ToString(), order);
        }

        private bool disposedValue = false; // To detect redundant calls
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _client.Dispose();
                }
                disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }
    }
}