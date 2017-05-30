using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Documents.Client;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Documents;
using Microsoft.Knowzy.Domain;

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

        public IEnumerable<Shipping> GetShippings()
        {
            var orders = _client.CreateDocumentQuery<Shipping>(
                _ordersLink,
                "SELECT * FROM orders o WHERE o.type='shipping'",
                _options).ToList();

            if (orders != null && orders.Count() > 0)
                return orders;
            else
                return null;
        }

        public IEnumerable<Receiving> GetReceivings()
        {
            var orders = _client.CreateDocumentQuery<Receiving>(
                _ordersLink,
                "SELECT * FROM orders o WHERE o.type='receiving'",
                _options).ToList();

            if (orders != null && orders.Count() > 0)
                return orders;
            else
                return null;
        }

        public IEnumerable<PostalCarrier> GetPostalCarriers()
        {
            return _client.CreateDocumentQuery<PostalCarrier>(
                _ordersLink,
                "SELECT o.postalCarrier FROM orders o",
                _options).Distinct().ToList();
        }

        public Shipping GetShipping(string orderId)
        {
            return _client.CreateDocumentQuery<Shipping>(
                _ordersLink,
                new SqlQuerySpec
                {
                    QueryText = "SELECT * FROM orders o WHERE (o.id = @orderid)",
                    Parameters = new SqlParameterCollection()
                    {
                          new SqlParameter("@orderid", orderId)
                    }
                }, _options).First();
        }

        public Receiving GetReceiving(string orderId)
        {
            return _client.CreateDocumentQuery<Receiving>(
                _ordersLink,
                new SqlQuerySpec
                {
                    QueryText = "SELECT * FROM orders o WHERE (o.id = @orderid)",
                    Parameters = new SqlParameterCollection()
                    {
                          new SqlParameter("@orderid", orderId)
                    }
                }, _options).First();
        }

        public async Task UpsertOrder(Order order)
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