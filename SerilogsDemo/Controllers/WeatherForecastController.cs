using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SerilogsDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            WriteLogs();


            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        private void WriteLogs()
        {
            Log.Information("Serilg: Writing some information");

            Channel channel = new Channel()
            {
                Name = "Skynet",
                Description = "Subscribe for more videos"
            };
            Log.Information("Serilog: Writing an object, {Channel}", JsonConvert.SerializeObject(channel));

            try
            {
                int a = 0, b = 10;
                int c = b / a;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Serilog: Error in WriteLogs method");
            }
        }
    }

    class Channel
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
