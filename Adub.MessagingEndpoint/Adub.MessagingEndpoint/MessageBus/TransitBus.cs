﻿using Adub.MessagingEndpoint.Configuration;
using Adub.MessagingEndpoint.IoC;
using Adub.MessagingEndpoint.MessageBus.Handlers;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace Adub.MessagingEndpoint.MessageBus
{
    public class TransitBus : ITransitBus
    {
        private IBusControl _serviceBus;
        private readonly IRabbitConnectionInfo rabbitConnectionInfo;
        private bool started = false;
        public IBusControl ServiceBus { get { return _serviceBus; } set { } }
                
        public TransitBus(IRabbitConnectionInfo rabbitConnectionInfo)
        {
            this.rabbitConnectionInfo = rabbitConnectionInfo;
        }

        public ITransitBus Start()
        {
            if (started) return this;

            _serviceBus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var conn = $"rabbitmq://{rabbitConnectionInfo.Host}:{rabbitConnectionInfo.Port}/{rabbitConnectionInfo.VHost}";
                Console.WriteLine($"Rabbit Connection: {conn}");

                var host = sbc.Host(new Uri(conn), h =>
                {
                    h.Username(rabbitConnectionInfo.User);
                    h.Password(rabbitConnectionInfo.Password);
                });

                sbc.ReceiveEndpoint(host, "sample_queue", ep =>
                {
                    ep.Consumer(typeof(SampleMessageHandler), type => DependencyFactory.Instance.Container.GetInstance(type));
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
    }
}
