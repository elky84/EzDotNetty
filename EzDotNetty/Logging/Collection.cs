using EzDotNetty.Types;
using EzDotNetty.Logging;
using Serilog;

namespace EzDotNetty.Logging
{
    public static partial class Collection
    {
        public class LoggerInfo
        {
            public Serilog.Core.Logger Logger { get; set; }

            public Serilog.Core.LoggingLevelSwitch Switch { get; set; }

            public LoggerInfo(Serilog.Core.Logger logger, Serilog.Core.LoggingLevelSwitch logLevelSwitch)
            {
                Logger = logger;
                Switch = logLevelSwitch;
            }
        }

        private static readonly Dictionary<LoggerId, LoggerInfo> Infos = new();

        public static void Init<T>() where T : LoggerId
        {
            foreach(var loggerId in Enumeration.GetAll<T>())
            {
                New(loggerId, Serilog.Events.LogEventLevel.Information);
            }
        }
        public static Serilog.Core.Logger? Get(LoggerId loggerId)
        {
            if (!Infos.TryGetValue(loggerId, out var info))
            {
                Log.Logger.Fatal($"Get({loggerId}) Failed. Not registered Logger.");
                return null;
            }

            return info.Logger;
        }

        public static Serilog.Core.LoggingLevelSwitch? LevelSwitch(LoggerId loggerId)
        {
            return Infos.TryGetValue(loggerId, out var info) ? info.Switch : null;
        }

        public static Serilog.Core.LoggingLevelSwitch? LevelSwitch(LoggerId loggerId, Serilog.Events.LogEventLevel logEventLevel)
        {
            if( Infos.TryGetValue(loggerId, out var info))
            {
                info.Switch.MinimumLevel = logEventLevel;
            }
            return info?.Switch;
        }

        public static Serilog.Core.Logger? New(LoggerId loggerId, Serilog.Events.LogEventLevel logEventLevel)
        {
            var logLevelSwitch = new Serilog.Core.LoggingLevelSwitch();
            logLevelSwitch.MinimumLevel = logEventLevel;

            var logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(logLevelSwitch)
                .WriteTo.File($"{loggerId.Name}.log")
                .WriteTo.Console()
                .CreateLogger();

            if(logger != null)
            {
                if(Infos.ContainsKey(loggerId))
                    Infos.Remove(loggerId);

                Infos.Add(loggerId, new LoggerInfo(logger, logLevelSwitch));
            }
            return logger;
        }
    }
}
