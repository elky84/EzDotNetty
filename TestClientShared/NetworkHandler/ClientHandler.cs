using DotNetty.Transport.Channels;
using Serilog;
using System.Reflection;
using ZeroFormatter;

namespace TestClientShared.NetworkHandler
{

    public class ClientHandler : EzDotNetty.Handler.Client.NetworkHandler
    {
        private readonly MethodInfo DeserializeMethod;

        private readonly MethodInfo PublishMethod;

        private readonly Assembly? ProtocolsResponseAssembly;

        public ClientDispatcher? ClientDispatcher { get; set; }

        public Action<IChannelHandlerContext>? OnConnect { get; set; }

        public Action<IChannelHandlerContext>? OnClose { get; set; }

        public ClientHandler() : base()
        {
            ProtocolsResponseAssembly = AppDomain.CurrentDomain.GetAssemblies()!
                .FirstOrDefault(x => "Protocols.Response".StartsWith(x!.GetName()!.Name!));

            DeserializeMethod = typeof(ClientHandler).GetMethod("Deserialize")!;
            PublishMethod = typeof(ClientHandler).GetMethod("Publish")!;
        }

        public void Release()
        {
            OnConnect = null;
            OnClose = null;
        }

        protected override void OnChannelActive(IChannelHandlerContext context)
        {
            OnConnect?.Invoke(context);
        }

        protected override void OnChannelUnregistered(IChannelHandlerContext context)
        {
            OnClose?.Invoke(context);
        }

        public static T Deserialize<T>(byte[] bytes) where T : new()
        {
            return ZeroFormatterSerializer.Deserialize<T>(bytes);
        }

        protected override void OnReceive(IChannelHandlerContext context, int id, byte[] bytes)
        {
            var responseType = ProtocolsResponseAssembly!.GetType($"Protocols.Response.{(Protocols.Id.Response)id}");
            var genericMethod = DeserializeMethod.MakeGenericMethod(responseType!);

            var protocol = genericMethod.Invoke(this, new object[] { bytes });
            var genericPublishMethod = PublishMethod.MakeGenericMethod(responseType!);

            try
            {
                genericPublishMethod.Invoke(this, new object[] { protocol! });
            }
            catch (TargetInvocationException exception)
            {
                Log.Logger.Error($"TargetInvocationException. {exception.Message}");
            }
        }
    }
}