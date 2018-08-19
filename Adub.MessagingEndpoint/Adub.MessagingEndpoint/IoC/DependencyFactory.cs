using Adub.MessagingEndpoint.Configuration;
using Adub.MessagingEndpoint.HubConnector;
using Adub.MessagingEndpoint.MessageBus;
using Lamar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Adub.MessagingEndpoint.IoC
{
    public sealed class DependencyFactory
    {
        private static DependencyFactory instance = null;
        private static readonly object padlock = new object();
        public IContainer Container { get; set; }

        DependencyFactory()
        {
            this.Container = new Container((config) => {
                config.For<IRabbitConnectionInfo>().Use(new RabbitConnectionInfo().Initialize("masstransit-poc")).Singleton();
                config.For<ITransitBus>().Use<TransitBus>().Singleton();
                config.For<ISignalRMessageSender>().Use<SignalRMessageSender>().Singleton();
            });
        }

        public static DependencyFactory Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new DependencyFactory();
                    }
                    return instance;
                }
            }
        }

        public T GetInstance<T>()
        {
            return Container.GetInstance<T>();
        }
    }
}
