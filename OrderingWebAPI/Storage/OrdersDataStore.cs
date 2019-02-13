using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderingWebAPI.Models;

namespace OrderingWebAPI.Storage
{
  // Class to act as a Orders DB context
  public class OrdersDataStore
  {
    private static readonly object mutex = new object();
    private static Dictionary<int, Order> _orderData = new Dictionary<int, Order>();

    public static Dictionary<int, Order> Contents
    {
      get
      {
        lock (mutex)
        {
          return _orderData;
        }
      }
      set
      {
        lock (mutex)
        {
          _orderData = value;
        }
      }
    }

  }
}
