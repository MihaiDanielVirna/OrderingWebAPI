using System;
using System.Collections.Generic;

namespace OrderingWebAPI.Models
{
  public class Order
  {
    public int OrderId { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public Dictionary<int, int> ProductsAndQuantities { get; set; } = new Dictionary<int, int>();
    public bool Deleted { get; set; }
    public double Total { get; set; } = 0;
  }
}
