using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Serilog;

namespace EzDotNetty.Handler.Client
{
    public abstract class NetworkHandler : ChannelHandlerAdapter
    {
        private static readonly Dictionary<Type, List<object>> _actions = new();

        protected Random Random = new();

        protected NetworkHandler() : base()
        {
        }

        public static int Subscribe<T>(Action<T> callback, bool topPriority = false)
        {
            try
            {
                lock (_actions)
                {
                    if (_actions.ContainsKey(typeof(T)) == false)
                    {
                        _actions.Add(typeof(T), new List<object>());
                    }
                    else
                    {
                        if (_actions[typeof(T)].Contains(callback))
                        {
                            Log.Logger.Error("Callback already subscribed..");
                            return 0;
                        }
                    }

                    if (topPriority)
                    {
                        _actions[typeof(T)].Insert(0, callback);
                    }
                    else
                    {
                        _actions[typeof(T)].Add(callback);
                    }
                }

                return callback.GetHashCode();
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Exception: {Ex}", ex.Message);
                return 0;
            }
        }

        public static void Unsubscribe<T>(Action<T> callback)
        {
            try
            {
                lock (_actions)
                {
                    if (_actions.TryGetValue(typeof(T), out var actions) == false)
                    {
                        return;
                    }

                    if (actions.Contains(callback) == false)
                    {
                        return;
                    }

                    if (actions.Remove(callback) == false)
                    {
                        Log.Logger.Error("remove subscription error for {Name}", typeof(T).Name);

                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Exception: {Ex}", ex.Message);
            }
        }

        public static void Unsubscribe(Type type, int hash)
        {
            try
            {
                lock (_actions)
                {
                    if (_actions.TryGetValue(type, out var actions) == false)
                    {
                        Log.Logger.Error("remove subscription error for {TypeName} {Hash}", type.Name, hash);
                        return;
                    }

                    var action = actions.FirstOrDefault(x => x.GetHashCode() == hash);
                    if (action == null)
                    {
                        Log.Logger.Error("remove subscription error for {TypeName} {Hash}", type.Name, hash);
                        return;
                    }

                    actions.Remove(action);
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Exception: {Ex}", ex.Message);
            }
        }

        // ReSharper disable once UnusedMember.Global
        public static void Publish<T>(T? msg = null) where T : class
        {
            try
            {
                List<Action<T>> actionsCopy;

                lock (_actions)
                {
                    if (_actions.ContainsKey(typeof(T)))
                    {
                        actionsCopy = _actions[typeof(T)].OfType<Action<T>>().ToList();
                    }
                    else
                    {
                        Log.Logger.Error("Not Defined ClientHandler. {Msg}", msg);
                        return;
                    }
                }

                if (!actionsCopy.Any()) return;
                
                foreach (var a in actionsCopy)
                {
                    try
                    {
                        a(msg!);
                    }
                    catch (Exception ex)
                    {
                        Log.Logger.Error("Exception: {Ex}", ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Exception: {Ex}", ex.Message);
            }
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            OnChannelActive(context);
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            try
            {
                var buffer = message as IByteBuffer;

                var id = buffer!.ReadInt();

                var bytes = new byte[buffer!.ReadableBytes];
                buffer.ReadBytes(bytes);

                OnReceive(context, id, bytes);
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Exception: {Ex}", ex.Message);
            }
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception ex)
        {
            Log.Logger.Error("Exception: {Ex}", ex.Message);
            context.CloseAsync();
        }

        public override void ChannelUnregistered(IChannelHandlerContext context)
        {
            base.ChannelUnregistered(context);
            OnChannelUnregistered(context);
        }


        protected abstract void OnChannelActive(IChannelHandlerContext context);

        protected abstract void OnChannelUnregistered(IChannelHandlerContext context);

        protected abstract void OnReceive(IChannelHandlerContext context, int id, byte[] bytes);
    }
}