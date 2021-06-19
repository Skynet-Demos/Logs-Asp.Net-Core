using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Log4NetDemo.Controllers
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
        private readonly ILog log;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            log = LogManager.GetLogger(typeof(WeatherForecastController));
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
            log.Info("Log4Net: Wirting some information");

            Channel channel = new Channel()
            {
                Name = "Skynet",
                Description = "Subscribe for more videos"
            };
            log.Info("Log4Net: Writing an object: " + JsonConvert.SerializeObject(channel));

            try
            {
                int a = 0, b = 10;
                int c = b / a;
            }
            catch (Exception ex)
            {
                log.Error("Log4Net: Error in the WriteLogs method", ex);
            }
        }
    }

    class Channel
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
