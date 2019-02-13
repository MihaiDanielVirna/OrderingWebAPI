using OrderingWebAPI.Models;
using OrderingWebAPI.Services;
using OrderingWebAPI.Storage;
using System.Collections.Generic;

namespace OrderingWebAPI.ServicesImplementations
{
  public class ProductsService : IProductsService
  {
    //Unit testing help
    public void GenerateData()
    {
      ProductsDataStorage.PopulateProducts();
    }

    public Product Get(int productId)
    {
      ProductsDataStorage.Contents.TryGetValue(productId, out Product result);
      return result;
    }

    public bool ProductExists(int productId)
    {
      return ProductsDataStorage.Contents.ContainsKey(productId);
    }

    public Dictionary<int, Product> Get()
    {
      return ProductsDataStorage.Contents;
    }
  }
}
