using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Serilog;

namespace EzDotNetty.Handler.Client
{
    public abstract class NetworkHandler : ChannelHandlerAdapter
    {
        private static readonly Dictionary<Type, List<object>> _actions = new();

        protected Random Random = new();

        public NetworkHandler() : base()
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
                Log.Logger.Error("Exception: " + ex);
                return 0;
            }
        }

        public static void Unsubscribe<T>(Action<T> callback)
        {
            try
            {
                lock (_actions)
                {
                    List<object>? actions;
                    if (_actions.TryGetValue(typeof(T), out actions) == false)
                    {
                        return;
                    }

                    if (actions.Contains(callback) == false)
                    {
                        return;
                    }

                    if (actions.Remove(callback) == false)
                    {
                        Log.Logger.Error(String.Format("remove subscription error for {0}", typeof(T).Name));

                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Exception: " + ex);
            }
        }

        public static void Unsubscribe(Type type, int hash)
        {
            try
            {
                lock (_actions)
                {
                    List<object>? actions;
                    if (_actions.TryGetValue(type, out actions) == false)
                    {
                        Log.Logger.Error(String.Format("remove subscription error for {0},{1}", type.Name, hash));
                        return;
                    }

                    var action = actions.FirstOrDefault(x => x.GetHashCode() == hash);
                    if (action == null)
                    {
                        Log.Logger.Error(String.Format("remove subscription error for {0},{1}", type.Name, hash));
                        return;
                    }

                    actions.Remove(action);
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Exception: " + ex);
            }
        }

        public void Publish<T>(T? msg = null) where T : class
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
                        Log.Logger.Error($"Not Defined ClientHandler. {msg}");
                        return;
                    }
                }

                if (actionsCopy.Any())
                {
                    foreach (var a in actionsCopy)
                    {
                        try
                        {
                            var copyA = a;
                            copyA(msg!);
                        }
                        catch (Exception ex)
                        {
                            // TODO: this entry should be removed
                            Log.Logger.Error("Exception: " + ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Exception: " + ex);
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

                byte[] bytes = new byte[buffer!.ReadableBytes];
                buffer.ReadBytes(bytes);

                OnReceive(context, id, bytes);
            }
            catch (Exception e)
            {
                Log.Logger.Error("Exception: " + e);
            }
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception e)
        {
            Log.Logger.Error("Exception: " + e);
            context.CloseAsync();
        }

        public override void ChannelUnregistered(IChannelHandlerContext context)
        {
            base.ChannelUnregistered(context);
            OnChannelUnregistered(context);
        }



        public abstract void OnChannelActive(IChannelHandlerContext context);

        public abstract void OnChannelUnregistered(IChannelHandlerContext context);

        public abstract void OnReceive(IChannelHandlerContext context, int id, byte[] bytes);
    }
}