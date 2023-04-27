using Microsoft.Extensions.Configuration;
using Serilog;
using System.Collections.Concurrent;

namespace EzDotNetty.Logging
{
    public static class Loggers
    {
        private static ConcurrentDictionary<string, Serilog.Core.Logger> Container { get; } = new ConcurrentDictionary<string, Serilog.Core.Logger>();

        public static void Initialize()
        {
            if (File.Exists("serilog.json"))
            {
                var seriLogJson = new ConfigurationBuilder()
                                      .AddJsonFile("serilog.json")
                                      .Build();

                Log.Logger = new LoggerConfiguration()
                                .ReadFrom.Configuration(seriLogJson)
                                .CreateLogger();
            }
            else
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.Console()
                    .WriteTo.File($"logs/{System.Diagnostics.Process.GetCurrentProcess().ProcessName}_.log", rollingInterval: RollingInterval.Day)
                    .CreateLogger();
            }
        }


        private static Serilog.Core.Logger? ForContext(string name, Serilog.Events.LogEventLevel logEventLevel = Serilog.Events.LogEventLevel.Information)
        {
            if (Container.TryGetValue(name, out var logger))
            {
                return logger;
            }

            Serilog.Core.LoggingLevelSwitch? logLevelSwitch = new()
            {
                MinimumLevel = logEventLevel
            };

            logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(logLevelSwitch)
                .WriteTo.File($"logs/{name}.log")
                .WriteTo.Console()
                .CreateLogger();

            Container.TryAdd(name, logger);
            return logger;
        }

        public static Serilog.Core.Logger? ForContext(Type type, Serilog.Events.LogEventLevel logEventLevel = Serilog.Events.LogEventLevel.Information)
        {
            return ForContext(type.Name, logEventLevel);
        }
    }
}
