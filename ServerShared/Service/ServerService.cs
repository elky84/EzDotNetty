using LightInject;
using ServerShared.NetworkHandler;
using System;

namespace ServerShared.Service
{
    public static class ServerService
    {
        private static ServiceContainer Container;

        public class SingletoneLifetime : ILifetime
        {
            object instances;

            public object GetInstance(Func<object> instanceFactory, Scope currentScope)
            {
                if (instances == null)
                {
                    instances = instanceFactory();
                }
                return instances;
            }
        }

        public static bool Register()
        {
            Container = new ServiceContainer();
            Container.EnableAnnotatedPropertyInjection();

            // 이렇게 등록되어있는 Service 끼리만 Inject Annotation이 먹힌다. 
            // Service로 사용할 인스턴스라면, new 대신 GetInstance로 생성하거나, Inject를 사용해야 함에 주의하라.
            Container.Register<SessionService>(new SingletoneLifetime());
            Container.Register<RoomService>(new SingletoneLifetime());

            Container.Register<PacketDispatcher>(new SingletoneLifetime());
            Container.Register<ServerDispatcher>(new SingletoneLifetime());

            Container.Register<EzDotNetty.Handler.Server.NetworkHandler>(new SingletoneLifetime());

            PostConstruct();
            return true;
        }

        public static void AddSingletone<T>() where T : class
        {
            Container.Register<T>(new SingletoneLifetime());
        }

        public static void Register(Type serviceType, Type implementingType)
        {
            Container.Register(serviceType, implementingType);
        }

        public static void Register(Type serviceType, Type implementingType, ILifetime lifeTime)
        {
            Container.Register(serviceType, implementingType, lifeTime);
        }

        public static void RegisterSingletone(Type serviceType, Type implementingType)
        {
            Container.Register(serviceType, implementingType, new SingletoneLifetime());
        }


        public static void PostConstruct()
        {
        }

        public static TService GetInstance<TService>()
        {
            return Container.GetInstance<TService>();
        }
    }
}
