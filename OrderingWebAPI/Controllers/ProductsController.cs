using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using OrderingWebAPI.Models;
using OrderingWebAPI.Services;

namespace OrderingWebAPI.Controllers
{
  [Route("api/[controller]")]
  public class ProductsController : Controller
  {
    private readonly IProductsService _productsService;

    public ProductsController(IProductsService productsService)
    {
      _productsService = productsService;
    }
    
    /// <summary>
    /// Gets all available products.
    /// </summary>
    /// <returns>Returns <see cref="Dictionary{TKey,TValue}"/> with products indexed by ProductId. </returns>
    [HttpGet]
    public Dictionary<int, Product> GetAllProducts()
    {
      return _productsService.Get();
    }
  }
}