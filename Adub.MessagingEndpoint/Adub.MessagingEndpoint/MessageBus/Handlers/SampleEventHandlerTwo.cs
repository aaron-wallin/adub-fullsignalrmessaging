using Adub.MessagingContracts.Events;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Adub.MessagingEndpoint.MessageBus.Handlers
{
    public class SampleEventHandlerTwo : IConsumer<ISampleEvent>
    {
        public Task Consume(ConsumeContext<ISampleEvent> context)
        {
            Console.WriteLine($"SAMPLE EVENT HANDLER TWO RECEIVED: {context.Message.EventMessage}");
            return Task.CompletedTask;
        }
    }
}
