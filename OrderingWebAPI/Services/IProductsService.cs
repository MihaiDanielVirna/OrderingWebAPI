using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderingWebAPI.Models;

namespace OrderingWebAPI.Services
{
    public interface IProductsService
    {
      void GenerateData();
      Product Get(int productId);
      bool ProductExists(int productId);
      Dictionary<int, Product> Get();
    }
}
