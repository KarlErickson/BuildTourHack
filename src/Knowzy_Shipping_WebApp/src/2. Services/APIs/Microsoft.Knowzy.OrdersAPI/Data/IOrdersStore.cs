using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Knowzy.Domain;

namespace Microsoft.Knowzy.OrdersAPI.Data
{
    public interface IOrdersStore : IDisposable
    {
        Task<bool> Connected();
        IEnumerable<Domain.Shipping> GetShippings();
        Shipping GetShipping(string orderId);
        void CreateOrder(Domain.Order order);
        void UpdateOrder(Domain.Order order);
    }
}