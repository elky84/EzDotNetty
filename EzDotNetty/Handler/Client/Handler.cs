using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using EzDotNetty.Logging;
using System.Text;

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
                            Collection.Get(LoggerId.Message)!.Error("Callback already subscribed..");
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
                Collection.Get(LoggerId.Message)!.Error("Exception: " + ex);
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
                        Logging.Collection.Get(LoggerId.Message)!.Error(String.Format("remove subscription error for {0}", typeof(T).Name));

                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Collection.Get(LoggerId.Message)!.Error("Exception: " + ex);
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
                        Logging.Collection.Get(LoggerId.Message)!.Error(String.Format("remove subscription error for {0},{1}", type.Name, hash));
                        return;
                    }

                    var action = actions.FirstOrDefault(x => x.GetHashCode() == hash);
                    if (action == null)
                    {
                        Logging.Collection.Get(LoggerId.Message)!.Error(String.Format("remove subscription error for {0},{1}", type.Name, hash));
                        return;
                    }

                    actions.Remove(action);
                }
            }
            catch (Exception ex)
            {
                Logging.Collection.Get(LoggerId.Message)!.Error("Exception: " + ex);
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
                        Logging.Collection.Get(LoggerId.Message)!.Error($"Not Defined ClientHandler. {msg}");
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
                            Logging.Collection.Get(LoggerId.Message)!.Error("Exception: " + ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Collection.Get(LoggerId.Message)!.Error("Exception: " + ex);
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

                byte[] bytes = new byte[buffer!.ReadableBytes];
                buffer.ReadBytes(bytes);

                OnReceive(context, Encoding.UTF8.GetString(bytes, 0, bytes.Length));
            }
            catch (Exception e)
            {
                Logging.Collection.Get(LoggerId.Message)!.Error("Exception: " + e);
            }
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception e)
        {
            Logging.Collection.Get(LoggerId.Message)!.Error("Exception: " + e);
            context.CloseAsync();
        }

        public override void ChannelUnregistered(IChannelHandlerContext context)
        {
            base.ChannelUnregistered(context);
            OnChannelUnregistered(context);
        }



        public abstract void OnChannelActive(IChannelHandlerContext context);

        public abstract void OnChannelUnregistered(IChannelHandlerContext context);

        public abstract void OnReceive(IChannelHandlerContext context, string str);
    }
}