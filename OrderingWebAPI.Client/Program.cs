using OrderingWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using OrderingWebAPI.Storage;

namespace OrderingWebAPI.Client
{

  class Program
  {
    static void Main(string[] args)
    {
      RunClient().GetAwaiter().GetResult();
    }

    static async Task RunClient()
    {
      //Setup API Client
      var address = "http://localhost:51444/api/";

      using (var client = new HttpClient())
      {
        client.BaseAddress = new Uri(address);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
          new MediaTypeWithQualityHeaderValue("application/json"));


        var products = await GetProducts(client);
        Console.WriteLine("Welcome to the Ordering API Demo. The available products are:");

        foreach (var product in products)
        {
          Console.WriteLine($"{product.Value.Name}, Price {product.Value.Price}, ID {product.Value.ProductId}");
        }
        Console.WriteLine();
        Console.WriteLine("Setting up order.");

        var url = await CreateOrder(client);
        
        var order = await GetOrderAsync(client, url.PathAndQuery);

        Console.WriteLine($"Order number {order.OrderId} created at {order.CreatedDate.ToShortTimeString()}");
        Console.WriteLine();

        string command = String.Empty;
        do
        {
          if (Int32.TryParse(command, out int product))
          {
            if (products.ContainsKey(product))
            {
              Console.WriteLine($"Please input the quantity for {products[product].Name}");
              command = Console.ReadLine();
              if (Int32.TryParse(command, out int quantity))
              {
                await AddToOrder(client, order.OrderId, product, quantity);
              }
              else
                Console.WriteLine("Invalid quantity.");
            }
            else
              Console.WriteLine($"Product not found.");
          }
          else if (command.ToLowerInvariant().Equals("check"))
          {
            order = await CheckOrder(client, order.OrderId);
            string message = $"Order {order.OrderId} contains";
            foreach (var item in order.ProductsAndQuantities)
            {
              message += $", {item.Value} of {products[item.Key].Name}";
            }

            message += $".Total of {order.Total}";
            Console.WriteLine(message);
          }
          else if (command.ToLowerInvariant().Equals("clear"))
          {
            await ClearOrder(client, order.OrderId);
            Console.WriteLine($"Order items cleared.");
          }
          else if (!command.Equals(String.Empty))
          {
            Console.WriteLine($"Invalid input, please input a valid command.");
          }

          Console.WriteLine(
            "Please select a product id to add or type 'Clear' to clear the order items, 'Exit' to close or 'Check' to see the current order: ");
          command = Console.ReadLine();
        } while (!command.ToLowerInvariant().Equals("exit"));

        Console.WriteLine($"Closing the application...");
      }
    }

    private static async Task<Order> ClearOrder(HttpClient client, int orderId)
    {
      var response = await client.PutAsJsonAsync<Order>($"orders/{orderId}", null);
      if (response.IsSuccessStatusCode)
      {
        return await CheckOrder(client, orderId);
      }

      return null;
    }

    private static async Task<bool> AddToOrder(HttpClient client, int orderId, int product, int quantity)
    {
      var response = await client.PutAsync($"orders/{orderId}/{product}/{quantity}", null);
      if (response.IsSuccessStatusCode)
      {
        return true;
      }
      return false;
    }

    private static async Task<Order> GetOrderAsync(HttpClient client, string pathAndQuery)
    {
      var response = await client.GetAsync(pathAndQuery);
      if (response.IsSuccessStatusCode)
      {
        return await response.Content.ReadAsAsync<Order>();
      }

      return null;
    }


    private static async Task<Order> CheckOrder(HttpClient client, int orderId)
    {
      return await GetOrderAsync(client, $"orders/{orderId}");
    }

    private static async Task<Uri> CreateOrder(HttpClient client)
    {
      var newOrder = new Order()
      {
        OrderId = Environment.TickCount % DateTime.Now.Day
      };
      var response = await client.PostAsJsonAsync("orders", newOrder);
      response.EnsureSuccessStatusCode();
      return response.Headers.Location;
    }

    private static async Task<Dictionary<int, Product>> GetProducts(HttpClient client)
    {
      var response = await client.GetAsync("products");
      if (response.IsSuccessStatusCode)
      {
        return await response.Content.ReadAsAsync<Dictionary<int, Product>>();
      }

      return null;
    }
  }
}

