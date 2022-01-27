using EzDotnetty.Logging;
using Serilog;

namespace EzDotNetty.Logging
{
    public static partial class Collection
    {
        private static readonly Dictionary<LoggerId, Serilog.Core.Logger?> Loggers = new();

        public static void Init()
        {
            New(LoggerId.Buff, Serilog.Events.LogEventLevel.Information);
            New(LoggerId.Message, Serilog.Events.LogEventLevel.Information);
        }
        public static Serilog.Core.Logger? Get(LoggerId loggerId)
        {
            return Loggers.TryGetValue(loggerId, out var logger) ? logger : null;
        }

        public static Serilog.Core.Logger? New(LoggerId loggerId, Serilog.Events.LogEventLevel logEventLevel)
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Is(logEventLevel)
                .WriteTo.File($"{loggerId.Name}.log")
                .WriteTo.Console()
                .CreateLogger();

            if(logger != null)
            {
                Loggers.Add(loggerId, logger);
            }

            return logger;
        }
    }
}
