using Microsoft.Extensions.Configuration;

namespace EzDotNetty.Config
{
    public static class Helper
    {
        static Helper()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(ProcessDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
        }

        public static string ProcessDirectory => AppDomain.CurrentDomain.BaseDirectory;

        public static IConfigurationRoot Configuration { get; }
    }
}