using System;
using OrderingWebAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace OrderingWebAPI.Storage
{
  // Class to act as a Products DB context
  public class ProductsDataStorage
  {
    private static readonly object mutex = new object();
    private static Dictionary<int, Product> _products = new Dictionary<int, Product>();


    /// Simulate some existing DB data.
    public static void PopulateProducts()
    {
      var randomGenerator = new Random();
      List<string> productNames = new List<string>(){"Consistency", "Scalability", "Usability", "Flexibility", "API", "Client App"};
      foreach (var id in Enumerable.Range(1, productNames.Count))
      {
        lock (mutex)
        {
          _products.Add(id,
            new Product()
            {
              ProductId = id,
              Name = productNames[id - 1],
              Price = (float)(randomGenerator.NextDouble() + 0.1) * 10
            });
        }
      }
    }

    public static Dictionary<int, Product> Contents
    {
      get
      {
        lock (mutex)
        {
          return _products;
        }
      }
      set
      {
        lock (mutex)
        {
          _products = value;
        }
      }
    }

  }
}
