using Serilog;

namespace EzDotNetty.Logging
{
    public static class Loggers
    {
        public static Serilog.Core.Logger? Buff { get; set; }

        public static Serilog.Core.Logger? Message { get; set; }

        public static void Init()
        {
            Buff = New(Serilog.Events.LogEventLevel.Information);
            Message = New(Serilog.Events.LogEventLevel.Information);
        }

        public static Serilog.Core.Logger New(Serilog.Events.LogEventLevel logEventLevel)
        {
            return new LoggerConfiguration()
                .MinimumLevel.Is(logEventLevel)
                .WriteTo.Console()
                .CreateLogger();
        }
    }
}
