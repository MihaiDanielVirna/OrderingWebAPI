using System;
using System.Collections.Generic;
using OrderingWebAPI.Models;
using OrderingWebAPI.Services;
using OrderingWebAPI.Storage;

namespace OrderingWebAPI.ServicesImplementations
{
  public class OrdersService : IOrdersService
  {
    public bool Submit(Order order)
    {
      return OrdersDataStore.Contents.TryAdd(order.OrderId, order);
    }

    //Added mostly for testing purposes
    public void ClearAll()
    {
      OrdersDataStore.Contents.Clear();
    }

    public Order Get(int orderId)
    {
      Order local;
      OrdersDataStore.Contents.TryGetValue(orderId, out local);

      return local;
    }

    public Dictionary<int, Order> Get()
    {
      return OrdersDataStore.Contents;
    }

    public bool Delete(int orderId)
    {
      if (OrdersDataStore.Contents.ContainsKey(orderId))
      {
        OrdersDataStore.Contents[orderId].Deleted = true;
        return true;
      }

      return false;
    }

    public bool AddItem(int orderId, int productId, int quantity)
    {
      var order = OrdersDataStore.Contents[orderId];
      if (order.ProductsAndQuantities.ContainsKey(productId))
      {
        order.ProductsAndQuantities[productId] =
          Math.Max(0, order.ProductsAndQuantities[productId] + quantity);
      }
      else if (quantity > 0)
      {
        order.ProductsAndQuantities.Add(productId, quantity);
      }

      return true;
    }

    public bool UpdateTotal(int orderId, Dictionary<int, Product> products)
    {
      var order = OrdersDataStore.Contents[orderId];
      order.Total = 0;
      foreach (var item in order.ProductsAndQuantities)
      {
        order.Total += products[item.Key].Price * item.Value;
      }

      return true;
    }

    public bool ClearItems(int orderId)
    {
      if (OrdersDataStore.Contents.ContainsKey(orderId))
      {
        OrdersDataStore.Contents[orderId].ProductsAndQuantities = new Dictionary<int, int>();
        OrdersDataStore.Contents[orderId].Total = 0;
      }

      return false;
    }

    public bool Update(int orderId, Order order)
    {
      if (OrdersDataStore.Contents.ContainsKey(orderId))
      {
        return OrdersDataStore.Contents[orderId].Deleted = true;
      }

      return false;
    }
  }
}
