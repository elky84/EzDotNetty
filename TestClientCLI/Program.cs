using EzDotNetty.Bootstrap.Client;
using System;
using System.Threading.Tasks;
using TestClient.NetworkHandler;
using TestClientShared.NetworkHandler;

namespace TestClient
{
    internal static class Program
    {
        private static async Task Main()
        {
            try
            {
                var channel = await BootstrapHelper.RunClientAsync<ClientHandler>((handler) => handler.ClientDispatcher = new PacketDispatcher());
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
