using EzDotNetty.Bootstrap.Server;

namespace TestServer
{
    class Program
    {
        static void Main() => BootstrapHelper.RunServerAsync<Handler.TestServerHandler>().Wait();
    }
}
