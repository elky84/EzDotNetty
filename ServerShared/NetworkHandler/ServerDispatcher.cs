using LightInject;
using Protocols;
using Serilog;
using ServerShared.Model;
using ServerShared.Service;
using ServerShared.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ZeroFormatter;

namespace ServerShared.NetworkHandler
{
    public class ServerDispatcher
    {
        [Inject]
        public SessionService SessionService { get; set; }

        [Inject]
        public PacketDispatcher PacketDispatcher { get; set; }

        public MessageWorker MessageWorker { get; set; } = new MessageWorker();

        private readonly Dictionary<string, MethodInfo> DispatcherMethodInfo = new();

        private readonly MethodInfo DeserializeMethod;

        private readonly Assembly ProtocolsRequestAssembly;

        public ServerDispatcher()
        {
            DeserializeMethod = typeof(ServerDispatcher).GetMethod("Deserialize");
            ProtocolsRequestAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(x => "Protocols.Request".StartsWith(x.GetName().Name));

            var requestClasses = ProtocolsRequestAssembly.GetTypes()
                .Where(x => x.IsClass && x.FullName.StartsWith("Protocols.Request"))
                .Select(x => x.Name)
                .ToHashSet();

            DispatcherMethodInfo = typeof(PacketDispatcher).GetMethods().Where(x =>
            {
                var parameters = x.GetParameters();
                return requestClasses.Contains(x.Name) && parameters[0].ParameterType == typeof(Session);
            }).ToDictionary(x => x.Name);

            MessageWorker.MessageCallback = Call;
            MessageWorker.Start();
        }

        public void Push(Message message)
        {
            MessageWorker.Push(message);
        }

        public bool Call(Message message)
        {
            try
            {
                if (message.Action != null)
                {
                    message.Action.Invoke();
                    return true;
                }
                else
                {
                    return DispatchSession(message);
                }

            }
            catch (Exception e)
            {
                if (e.InnerException?.GetType() == typeof(LogicException))
                {
                    var logicException = (LogicException)e.InnerException;
                    Log.Error($"exception catched. <Message:{logicException.Message}\n StackTrace:{logicException.StackTrace}>");
                    message.Session.Send(new Protocols.Response.Error
                    {
                        Result = logicException.Result,
                        Message = logicException.Message
                    });
                }
                else
                {
                    Log.Error($"exception catched. <Message:{e.Message}\n StackTrace:{e.StackTrace}>");
                    message.Session.Send(new Protocols.Response.Error
                    {
                        Result = Protocols.Code.Result.Exception,
                        Message = e.Message
                    });
                }
                return false;
            }
        }

        public static T Deserialize<T>(byte[] bytes) where T : new()
        {
            return ZeroFormatterSerializer.Deserialize<T>(bytes);
        }

        private bool DispatchSession(Message message)
        {
            var session = message.Session;
            var room = session.Room;

            var typeName = message.Id.ToString();
            if (false == DispatcherMethodInfo.TryGetValue(typeName, out var method))
            {
                Log.Error($"Not Find DispatcherMethod <Session:{session}> <TypeName:{typeName}> <Room:{room}>");
                return false;
            }

            var requestType = ProtocolsRequestAssembly.GetType($"Protocols.Request.{typeName}");
            var genericMethod = DeserializeMethod.MakeGenericMethod(requestType);
            return (bool)method.Invoke(PacketDispatcher, new object[] {
                session,
                genericMethod.Invoke(this, new object[] { message.Data })
            });
        }
    }
}
