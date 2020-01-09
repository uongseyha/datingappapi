using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingAppAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DatingAppAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly EFContext _dbContext;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, EFContext eFContext)
        {
            _logger = logger;
            _dbContext = eFContext;
        }

        [HttpGet]
        public IActionResult Get()
        {
            //Product product = new Product();
            //product.Name = "Pen Drive";
            //product.Description = "Pen Description";
            //_dbContext.Add(product);

            //product = new Product();
            //product.Name = "Memory Card";
            //product.Description = "Memory Description";
            //_dbContext.Add(product);

            //_dbContext.SaveChanges();

            //var rng = new Random();
            //return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            //{
            //    Date = DateTime.Now.AddDays(index),
            //    TemperatureC = rng.Next(-20, 55),
            //    Summary = Summaries[rng.Next(Summaries.Length)]
            //})
            //.ToArray();

            return Ok(_dbContext.Products.ToList());
            
        }

        
    }
}
