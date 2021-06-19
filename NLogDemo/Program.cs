using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Layouts;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLogDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                ConfigurationNLogFromCode();
                logger.Info("NLog: Application is starting...");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "NLog: Application failed to start");
                throw;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Information);
                })
                .UseNLog();

        private static void ConfigurationNLogFromCode()
        {
            var config = new NLog.Config.LoggingConfiguration();

            // targets
            var logfile = new NLog.Targets.FileTarget("fromcode_logfile")
            {
                FileName = @"D:\LogsDemo\NLogs\logsfromcode.txt",
                Layout = "${longdate} ${level} ${message} ${exception:format=tostring} ${event-properties:myProperty}"
            };

            var jsonfile = new NLog.Targets.FileTarget("fromcode_jsonfile")
            {
                FileName = @"D:\LogsDemo\NLogs\jsonlogsfromcode.json",
                Layout = new JsonLayout
                {
                    Attributes =
                    {
                        new JsonAttribute("time", "${longdate}"),
                        new JsonAttribute("level", "${level:upperCase=true}"),
                        new JsonAttribute("logger", "${logger}"),
                        new JsonAttribute("message", "${message}"),
                        new JsonAttribute("exception", "${exception:format=tostring}")
                    }
                }
            };

            var databasequeryfile = new NLog.Targets.DatabaseTarget("fromcode_databasequery")
            {
                ConnectionString = @"server=RAJ-PC\MSSQLSERVERDEV19;Database=SkynetDemo;user id=sa;password=Sa@123",
                CommandText = @"insert into dbo.NLog (MachineName, Logged, Level, Message, Logger, Properties, Callsite, Exception) 
                                values(@MachineName, @Logged, @Level, @Message,@Logger, @Properties, @Callsite, @Exception);",
                Parameters =
                {
                    new NLog.Targets.DatabaseParameterInfo(){ Name = "@MachineName", Layout = "${machinename}"},
                    new NLog.Targets.DatabaseParameterInfo(){ Name = "@Logged", Layout = "${date}"},
                    new NLog.Targets.DatabaseParameterInfo(){ Name = "@Level", Layout = "${level}"},
                    new NLog.Targets.DatabaseParameterInfo(){ Name = "@Message", Layout = "${message}"},
                    new NLog.Targets.DatabaseParameterInfo(){ Name = "@Logger", Layout = "${logger}"},
                    new NLog.Targets.DatabaseParameterInfo(){ Name = "@Properties", Layout = "${all-event-properties:separator=|}"},
                    new NLog.Targets.DatabaseParameterInfo(){ Name = "@Callsite", Layout = "${callsite}"},
                    new NLog.Targets.DatabaseParameterInfo(){ Name = "@Exception", Layout = "${exception:tostring}"}
                }
            };

            var databasespfile = new NLog.Targets.DatabaseTarget("fromcode_databasesp")
            {
                ConnectionString = @"server=RAJ-PC\MSSQLSERVERDEV19;Database=SkynetDemo;user id=sa;password=Sa@123",
                CommandText = @"[dbo].[WriteLog_NLog]",
                CommandType = System.Data.CommandType.StoredProcedure,
                Parameters =
                {
                    new NLog.Targets.DatabaseParameterInfo(){ Name = "@MachineName", Layout = "${machinename}"},
                    new NLog.Targets.DatabaseParameterInfo(){ Name = "@Logged", Layout = "${date}"},
                    new NLog.Targets.DatabaseParameterInfo(){ Name = "@Level", Layout = "${level}"},
                    new NLog.Targets.DatabaseParameterInfo(){ Name = "@Message", Layout = "${message}"},
                    new NLog.Targets.DatabaseParameterInfo(){ Name = "@Logger", Layout = "${logger}"},
                    new NLog.Targets.DatabaseParameterInfo(){ Name = "@Properties", Layout = "${all-event-properties:separator=|}"},
                    new NLog.Targets.DatabaseParameterInfo(){ Name = "@Callsite", Layout = "${callsite}"},
                    new NLog.Targets.DatabaseParameterInfo(){ Name = "@Exception", Layout = "${exception:tostring}"}
                }
            };

            var splunkfile = new NLog.Targets.Splunk.SplunkHttpEventCollector()
            {
                ServerUrl = "http://localhost:8088/services/collector",
                Token = "73f4c508-35da-4d14-9d4b-12a8d820935f",
                Channel = "channel-guid",
                Source = "${logger}",
                SourceType = "_json",
                Index = "",
                RetriesOnError = 0,
                BatchSizeBytes = 0,
                BatchSizeCount = 0,
                IncludeEventProperties = true,
                IncludePositionalParameters = false,
                IncludeMdlc = false,
                MaxConnectionsPerServer = 10,
                IgnoreSslErrors = false,
                ContextProperties =
                {
                    new NLog.Targets.TargetPropertyWithContext(){ Name = "host", Layout = "${machinename}" },
                    new NLog.Targets.TargetPropertyWithContext(){ Name = "threadid", Layout = "${threadid}" },
                    new NLog.Targets.TargetPropertyWithContext(){ Name = "logger", Layout = "${logger}" }
                }
            };

            // rules
            config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logfile);

            config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, jsonfile);

            config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, databasequeryfile);

            config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, databasespfile);

            config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, splunkfile);

            // apply config
            NLog.LogManager.Configuration = config;
        }
    }
}
