using Microsoft.Extensions.Configuration;
using Serilog;

namespace EzDotNetty.Logging
{
    public static class LogConfiguration
    {
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
    }
}
