using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SerilogsDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(config).CreateLogger();

            //CreateSerilogLogConfiguration();

            try
            {
                Log.Information("Serilogs: Application is starting...");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Serilogs: Application failed to start");
                throw;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog();

        private static void CreateSerilogLogConfiguration()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                .WriteTo.Console(Serilog.Events.LogEventLevel.Information)
                .WriteTo.File("D:\\LogsDemo\\Serilogs\\log.txt", outputTemplate: "{Timestamp} {Level} {Message}{NewLine:1}{Exception:1}", rollingInterval: RollingInterval.Day)
                .WriteTo.File(new Serilog.Formatting.Json.JsonFormatter(), "D:\\LogsDemo\\Serilogs\\structuredLog.json", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
                .WriteTo.MSSqlServer("[your database connection string]",
                sinkOptions: new Serilog.Sinks.MSSqlServer.MSSqlServerSinkOptions()
                {
                    TableName = "Serilogs",
                    SchemaName = "dbo",
                    AutoCreateSqlTable = true
                }, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
                .WriteTo.EventCollector("[your splunk host]", "[your splunk token]")
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .Enrich.WithProperty("ApplicationName", "Serilogs Demo Application")
                .Enrich.WithProperty("ConfigurationFrom", "Code")
                .CreateLogger();
        }
    }
}
