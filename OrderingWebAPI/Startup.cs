using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderingWebAPI.Models;
using OrderingWebAPI.Services;
using OrderingWebAPI.ServicesImplementations;
using OrderingWebAPI.Storage;
using Swashbuckle.AspNetCore.Swagger;

namespace OrderingWebAPI
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc();
      services.AddScoped<IOrdersService, OrdersService>();
      services.AddScoped<IProductsService, ProductsService>();
      services.AddSwaggerGen(option =>
      {
        option.SwaggerDoc("v1", new Info
        {
          Title = "Ordering API Documentation",
          Version = "v1"
        });
         // Set the comments path for the Swagger JSON and UI.
        var xmlPath = $@"{System.AppDomain.CurrentDomain.BaseDirectory}\OrderingWebAPI.xml";
        option.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
      });
      //Populate with random data
      ProductsDataStorage.PopulateProducts();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseStaticFiles();
      app.UseSwagger();
      app.UseSwaggerUI(option => { option.SwaggerEndpoint("/swagger/v1/swagger.json", "Ordering API Documentation"); });
      app.UseMvc();
    }
  }
}
