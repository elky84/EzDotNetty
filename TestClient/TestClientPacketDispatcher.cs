using Protocols.Response;
using Serilog;
using System.Reflection;
using TestClientShared.NetworkHandler;

namespace TestClient
{
    public class TestClientPacketDispatcher : ClientDispatcher
    {
        public Action<string>? OnLog { get; set; }

        public TestClientForm? TestClientForm { get; set; }

        private readonly Dictionary<string, MethodInfo> FormHandlerMethods;

        public override void Release()
        {
            OnLog = null;
            TestClientForm = null;
            base.Release();
        }

        public TestClientPacketDispatcher()
        {
            var protocolsResponseAssembly = AppDomain.CurrentDomain.GetAssemblies()!
                .FirstOrDefault(x => "Protocols.Response".StartsWith(x!.GetName()!.Name!));

            var requestClasses = protocolsResponseAssembly!.GetTypes()
                .Where(x => x.IsClass && x.FullName!.StartsWith("Protocols.Request"))
                .Select(x => $"On{x.Name}")
                .ToHashSet();

            var methods = typeof(TestClientForm).GetMethods();

            FormHandlerMethods = methods.Where(x =>
            {
                return requestClasses.Contains(x.Name);
            }).ToDictionary(x => x.Name);
        }
        
        private void InvokeMethod<T>(T t) where T : Header
        {
            if (TestClientForm == null)
                return;

            var typeName = $"On{t.GetType().Name}";
            if ( FormHandlerMethods.TryGetValue(typeName, out var handler))
            {
                handler.Invoke(TestClientForm, new object[] { t });
            }
            else
            {
                Log.Error($"InvokeMethod() <Desc:Not found FormHandlerMethods> <Packet:{t}>");
            }
        }

        protected override void OnError(Error error)
        {
            OnLog?.Invoke($"OnError() <Result:{error.Result}> <Message:{error.Message}>");
        }

        protected override void OnLogin(Login login)
        {
            OnLog?.Invoke($"OnLogin() <Result:{login.Result}> <Name:{login.Name}>");
            InvokeMethod(login);
        }

        protected override void OnLogout(Logout logout)
        {
            OnLog?.Invoke($"OnLogout() <Result:{logout.Result}>");
            InvokeMethod(logout);
        }

        protected override void OnEnter(Enter enter)
        {
            OnLog?.Invoke($"OnEnter() <Result:{enter.Result}> <Name:{enter.Name}> <Position:{enter.Position}> <PlayerIndex:{enter.PlayerIndex}>");
            InvokeMethod(enter);
        }

        protected override void OnLeave(Leave leave)
        {
            OnLog?.Invoke($"OnLeave() <Result:{leave.Result}> <PlayerIndex:{leave.PlayerIndex}>");
            InvokeMethod(leave);
        }

        protected override void OnMove(Move move)
        {
            OnLog?.Invoke($"OnMove() <Result:{move.Result}> <Position:{move.Position}> <PlayerIndex:{move.PlayerIndex}>");
            InvokeMethod(move);
        }
    }
}
