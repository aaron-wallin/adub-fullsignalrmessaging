using Adub.MessagingContracts.Commands;
using Adub.MessagingEndpoint.Configuration;
using Adub.MessagingEndpoint.IoC;
using Adub.MessagingEndpoint.MessageBus.Handlers;
using MassTransit;
using MassTransit.RabbitMqTransport;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Adub.MessagingEndpoint.MessageBus
{
    public class TransitBus : ITransitBus
    {
        private IBusControl _serviceBus;
        private readonly IRabbitConnectionInfo rabbitConnectionInfo;
        private bool started = false;
        public IBusControl ServiceBus { get { return _serviceBus; } set { } }
        public string MessageQueueUri { get; private set; }
        private Dictionary<Type, string> _queueList;
                
        public TransitBus(IRabbitConnectionInfo rabbitConnectionInfo)
        {
            this.rabbitConnectionInfo = rabbitConnectionInfo;
            this.MessageQueueUri = $"rabbitmq://{rabbitConnectionInfo.Host}:{rabbitConnectionInfo.Port}/{rabbitConnectionInfo.VHost}";
            _queueList = new Dictionary<Type, string>();
            _queueList.Add(typeof(ISampleCommand), MessageQueues.SampleCommandQueue);
        }

        public ITransitBus Start()
        {
            if (started) return this;

            _serviceBus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {                
                Console.WriteLine($"Rabbit Connection: {MessageQueueUri}");

                var host = sbc.Host(new Uri(MessageQueueUri), h =>
                {
                    h.Username(rabbitConnectionInfo.User);
                    h.Password(rabbitConnectionInfo.Password);
                });

                sbc.ReceiveEndpoint(host, MessageQueues.SampleCommandQueue, ep =>
                {
                    ep.Consumer(typeof(SampleCommandHandler), type => DependencyFactory.Instance.Container.GetInstance(type));
                });                

                sbc.ReceiveEndpoint(host, MessageQueues.SampleEventQueue, ep =>
                {
                    ep.Consumer(typeof(SampleEventHandlerOne), type => DependencyFactory.Instance.Container.GetInstance(type));
                    //ep.Consumer(typeof(SampleEventHandlerTwo), type => DependencyFactory.Instance.Container.GetInstance(type));
                });

                sbc.ReceiveEndpoint(host, MessageQueues.SampleEventQueueTwo, ep =>
                {
                    //ep.Consumer(typeof(SampleEventHandlerOne), type => DependencyFactory.Instance.Container.GetInstance(type));
                    ep.Consumer(typeof(SampleEventHandlerTwo), type => DependencyFactory.Instance.Container.GetInstance(type));
                });
            });

            _serviceBus.Start();            

            started = true;

            return this;
        }        

        public async Task Publish<T>(T message) where T : class
        {
            await _serviceBus.Publish<T>(message);
        }

        public async Task Send<T>(object message) where T : class
        {
            var queue = _queueList[typeof(T)];
            var sendto = new Uri($"{MessageQueueUri}/{queue}");
            var ep = await _serviceBus.GetSendEndpoint(sendto);
            await ep.Send(message);
        }
    }
}
