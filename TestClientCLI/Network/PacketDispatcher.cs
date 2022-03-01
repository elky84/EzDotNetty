using Protocols.Response;
using Serilog;
using TestClientShared.NetworkHandler;

namespace TestClient.NetworkHandler
{
    public class PacketDispatcher : ClientDispatcher
    {
        protected override void OnError(Error error)
        {
            Log.Logger.Information($"OnError() <Result:{error.Result}> <Message:{error.Message}>");
        }

        protected override void OnLogin(Login login)
        {
            Log.Logger.Information($"OnLogin() <Result:{login.Result}> <Name:{login.Name}>");
        }

        protected override void OnLogout(Logout logout)
        {
            Log.Logger.Information($"OnLogout() <Result:{logout.Result}>");
        }

        protected override void OnEnter(Enter enter)
        {
            Log.Logger.Information($"OnEnter() <Result:{enter.Result}>");
        }

        protected override void OnLeave(Leave leave)
        {
            Log.Logger.Information($"OnLeave() <Result:{leave.Result}>");
        }

        protected override void OnMove(Move move)
        {
            Log.Logger.Information($"OnMove() <Result:{move.Result}>");
        }
    }
}
