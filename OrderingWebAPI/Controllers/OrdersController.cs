using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using OrderingWebAPI.Models;
using OrderingWebAPI.Services;

namespace OrderingWebAPI.Controllers
{
  [Route("api/[controller]")]
  public class OrdersController : Controller
  {
    private readonly IOrdersService _ordersService;
    private readonly IProductsService _productsService;

    public OrdersController(IOrdersService ordersService, IProductsService productsService)
    {
      _ordersService = ordersService;
      _productsService = productsService;
    }

    /// <summary>
    /// Gets all orders stored.
    /// </summary>
    /// <returns>Returns <see cref="Dictionary{TKey,TValue}"/> with orders indexed by OrderId. </returns>
    [HttpGet]
    public Dictionary<int, Order> GetAllOrders()
    {
      return _ordersService.Get();
    }

    /// <summary>
    /// Get the order based on Order ID.
    /// </summary>
    /// <param name="orderId">The ID of the Order to retrieve.</param>
    /// <returns>Order object if found</returns>
    [HttpGet("{orderId}")]
    public Order Get(int orderId)
    {
      return _ordersService.Get(orderId);
    }

    /// <summary>
    /// Adds the order.
    /// </summary>
    /// <param name="order">The order to be added</param>
    /// <response code="201">Order Created.</response>
    /// <response code="409">Order exists already.</response>
    [HttpPost]
    [ProducesResponseType(typeof(CreatedAtActionResult), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(StatusCodeResult), (int)HttpStatusCode.Conflict)]
    public IActionResult Submit([FromBody] Order order)
    {
      if (_ordersService.Submit(order))
        return CreatedAtAction(nameof(Get), new {order.OrderId}, order);

      return StatusCode((int) HttpStatusCode.Conflict);
    }

    /// <summary>
    /// Add the item with the specified quantity for an Order;
    /// </summary>
    /// <param name="orderId">The Order ID to update.</param>
    /// <param name="productId">The Product ID to increase the quantity for.</param>
    /// <param name="quantity">Quantity of the product.</param>
    /// <returns></returns>
    /// <response code="200">Item added.</response>
    /// <response code="404">Product does not exist.</response>
    /// <response code="409">The server could not fulfill the request.</response>
    [HttpPut("{orderid}/{productId}/{quantity}")]
    [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(StatusCodeResult), (int)HttpStatusCode.Conflict)]
    public IActionResult AddItem(int orderId, int productId, int quantity)
    {
      var product = _productsService.Get(productId);
      if (product != null)
      {
        if (_ordersService.AddItem(orderId, product.ProductId, quantity) &&
            _ordersService.UpdateTotal(orderId, _productsService.Get()))
          return Ok(_ordersService.Get(orderId));
        return StatusCode((int)HttpStatusCode.Conflict);
      }

      return NotFound();
    }


    /// <summary>
    /// Delete the order.
    /// </summary>
    /// <param name="orderId">The ID of the order to delete.</param>
    /// <returns>True if the order was deleted.</returns>
    [HttpDelete("{orderId}")]
    [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    public IActionResult Delete(int orderId)
    {
      if (_ordersService.Delete(orderId))
      {
        return Ok(true);
      }

      return NotFound();
    }

    /// <summary>
    /// Overwrite the order with the passed in object.
    /// </summary>
    /// <param name="orderId">The Order ID.</param>
    /// <param name="order">Order object that will replace the current one. If null it will clear the order items</param>
    /// <response code="200">Order updated.</response>
    /// <response code="404">Order does not exist.</response>
    [HttpPut("{orderId}")]
    [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
    public IActionResult Update(int orderId, [FromBody] Order order)
    {
      if (order == null)
      {
        if (_ordersService.ClearItems(orderId))
          return Ok(_ordersService.Get(orderId));
      }
      if(_ordersService.Update(orderId, order))
        return Ok(_ordersService.Get(orderId));

      return NotFound();
    }
  }
}
