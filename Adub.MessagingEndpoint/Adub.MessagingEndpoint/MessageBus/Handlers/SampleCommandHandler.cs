using Adub.MessagingContracts.Commands;
using Adub.MessagingContracts.Events;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Adub.MessagingEndpoint.MessageBus.Handlers
{
    public class SampleCommandHandler : IConsumer<ISampleCommand>
    {
        public Task Consume(ConsumeContext<ISampleCommand> context)
        {
            Console.WriteLine($"SAMPLE COMMAND HANDLER RECEIVED: {context.Message.CommandMessage}");
            context.Publish<ISampleEvent>(new ISampleEvent()
            {
                EventMessage = $"Published from Sample Command Handler {DateTime.Now.ToLongTimeString()}"
            });
            return Task.CompletedTask;
        }
    }
}
