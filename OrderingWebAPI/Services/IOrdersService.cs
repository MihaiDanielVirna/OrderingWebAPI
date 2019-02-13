using OrderingWebAPI.Models;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.JsonPatch;

namespace OrderingWebAPI.Services
{
  public interface IOrdersService
  {
    bool Submit(Order order);
    bool Update(int orderId, Order order);
    bool Delete(int orderId);
    Order Get(int orderId);
    Dictionary<int, Order> Get();
    bool AddItem(int orderId, int productId, int quantity);
    bool UpdateTotal(int orderId, Dictionary<int, Product> products);
    bool ClearItems(int orderId);
    void ClearAll();
  }
}
