using Adub.MessagingContracts;
using Adub.MessagingEndpoint.Configuration;
using Adub.MessagingEndpoint.HubConnector;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace Adub.MessagingEndpoint.MessageBus.Handlers
{
    public class SampleMessageHandler : IConsumer<SampleMessage>
    {
        private readonly ISignalRMessageSender signalRMessageHandler;

        public SampleMessageHandler(IRabbitConnectionInfo rabbitConnectionInfo, ISignalRMessageSender signalRMessageHandler)
        {
            Console.WriteLine($"Testing dependency injection: {rabbitConnectionInfo.Host}");
            this.signalRMessageHandler = signalRMessageHandler;
        }

        public Task Consume(ConsumeContext<SampleMessage> context)
        {
            Console.WriteLine($"CONSUMING MESSAGE IN SAMPLEMESSAGEHANDLER: {context.Message.Text}");
            this.signalRMessageHandler.Send(context.Message.Text);
            return Task.CompletedTask;
        }        
    }
}
