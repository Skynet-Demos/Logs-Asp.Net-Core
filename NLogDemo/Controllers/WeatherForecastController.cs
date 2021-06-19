using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLogDemo.Controllers
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
        private readonly NLog.Logger nLogger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            nLogger = NLog.LogManager.GetCurrentClassLogger();
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
            nLogger.Info("NLog: Writing some information");

            Channel channel = new Channel()
            {
                Name = "Skynet",
                Description = "Subscribe for more videos"
            };
            nLogger.Info("NLog: Writing an object, {Channel}", JsonConvert.SerializeObject(channel));

            try
            {
                int a = 0, b = 10;
                int c = b / a;
            }
            catch (Exception ex)
            {
                nLogger.Error(ex, "NLog: Error in the WriteLogs method");
            }
        }
    }

    public class Channel
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
