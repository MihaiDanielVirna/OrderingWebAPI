using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrderingWebAPI.Controllers;
using OrderingWebAPI.Models;
using OrderingWebAPI.Services;
using OrderingWebAPI.ServicesImplementations;

namespace OrderingWebAPI.Test
{
  [TestClass]
  public class OrdersControllerTests
  {

    private OrdersController _ordersController;
    private IOrdersService _ordersService;
    private IProductsService _productsService;
    private const int _defaultOrderId = 1;
    private const int _defaultProductId = 1;
    private const int _defaultQuantity = 1;

    public OrdersControllerTests()
    {
      _ordersService = new OrdersService();
      _productsService = new ProductsService();
      _ordersController = new OrdersController(_ordersService, _productsService);
    }


    [TestInitialize]
    public void Setup()
    {
      _ordersService.ClearAll();
    }

    [TestMethod]
    public void CreateOrder_ReturnsOk()
    {
      var resp = _ordersController.Submit(new Order() { OrderId = _defaultOrderId });
      Assert.IsInstanceOfType(resp, typeof(CreatedAtActionResult));
    }

    [TestMethod]
    public void CreateOrder_ReturnsConflict()
    {
      var response = _ordersController.Submit(new Order() { OrderId = _defaultOrderId });
      Assert.IsInstanceOfType(response, typeof(CreatedAtActionResult));

      var result = _ordersController.Submit(new Order() {OrderId = _defaultOrderId });
      Assert.IsInstanceOfType(result, typeof(StatusCodeResult));

      var statusCodeResult = result as StatusCodeResult;
      Assert.AreEqual(statusCodeResult.StatusCode, (int)HttpStatusCode.Conflict);

    }

    [TestMethod]
    public void Placeholder()
    {

    }
    [TestMethod]
    public void GetAllOrders_ReturnsEmptyCollection()
    {
      var response = _ordersController.GetAllOrders();
      Assert.IsNotNull(response);
      Assert.AreEqual(response.Count, 0);
    }


    [TestMethod]
    public void GetAllOrders_ReturnsNotEmptyCollection()
    {
      _ordersController.Submit(new Order() { OrderId = _defaultOrderId });
      var result = _ordersController.GetAllOrders();
      Assert.IsNotNull(result);
      Assert.IsTrue(result.Count > 0);
    }

    [TestMethod]
    public void Get_ReturnsOrder()
    {
      _ordersController.Submit(new Order() { OrderId = _defaultOrderId });
      var result = _ordersController.Get(_defaultOrderId);
      Assert.IsNotNull(result);
      Assert.IsInstanceOfType(result, typeof(Order));
      Assert.AreEqual(result.OrderId, _defaultOrderId);
    }

    [TestMethod]
    public void Get_ReturnsNullForNonExistingOrder()
    {
      _ordersController.Submit(new Order() { OrderId = _defaultOrderId });
      var result = _ordersController.Get(_defaultOrderId * 2);
      Assert.IsNull(result);
    }

    [TestMethod]
    public void Delete_ReturnsTrue()
    {
      _ordersController.Submit(new Order() { OrderId = _defaultOrderId });
      var result = _ordersController.Delete(_defaultOrderId);
      Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    [TestMethod]
    public void Delete_ReturnsNotFound()
    {
      _ordersController.Submit(new Order() { OrderId = _defaultOrderId });
      var result = _ordersController.Delete(_defaultOrderId*2);
      Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public void AddItem_ReturnsOk()
    {
      _productsService.GenerateData();
      _ordersController.Submit(new Order() { OrderId = _defaultOrderId });
      var result = _ordersController.AddItem(_defaultOrderId, _defaultProductId, _defaultQuantity);
      Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    [TestMethod]
    public void Update_ReplacesTheObject()
    {
      var order = new Order() {OrderId = _defaultOrderId};
      _ordersController.Submit(order);

      order.Deleted = true;

      var result = _ordersController.Update(_defaultOrderId, order);
      Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }
  }
}
