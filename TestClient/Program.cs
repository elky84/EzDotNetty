using EzDotNetty.Bootstrap.Client;
using TestClient.Handler;

namespace TestClient
{
    class Program
    {
        static void Main() => BootstrapHelper.RunClientAsync<TestClientHandler>().Wait();
    }
}
