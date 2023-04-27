using ServerShared.Service;
using EzDotNetty.Bootstrap.Server;
using System;
using System.Threading.Tasks;
using TestServer.Handler;

namespace TestServer
{
    internal static class Program
    {
        private static async Task Main()
        {
            ServerService.Register();

            //커스텀 LoggerId 추가시
            //EzDotNetty.Logging.Collection.Init<ServerShared.Logging.LoggerId>();

            try
            {
                var channel = await BootstrapHelper.RunServerAsync<TestServerHandler>();
                Console.ReadKey();
                await channel.CloseAsync();
            }
            finally
            {
                await BootstrapHelper.GracefulCloseAsync();
            }
        }
    }
}