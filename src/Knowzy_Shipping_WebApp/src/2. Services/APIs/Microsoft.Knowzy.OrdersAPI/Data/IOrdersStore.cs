using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Knowzy.Domain;

namespace Microsoft.Knowzy.OrdersAPI.Data
{
    public interface IOrdersStore : IDisposable
    {
        Task<bool> Connected();
        IEnumerable<Shipping> GetShippings();
        Shipping GetShipping(string orderId);
        IEnumerable<Receiving> GetReceivings();
        Receiving GetReceiving(string orderId);
        Task UpsertOrder(Order order);
        IEnumerable<PostalCarrier> GetPostalCarriers();
    }
}