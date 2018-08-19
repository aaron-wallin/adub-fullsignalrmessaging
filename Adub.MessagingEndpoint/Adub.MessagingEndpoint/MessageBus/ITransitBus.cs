using MassTransit;
using System.Threading.Tasks;

namespace Adub.MessagingEndpoint.MessageBus
{
    public interface ITransitBus
    {
        IBusControl ServiceBus { get; set; }

        Task Publish<T>(T message) where T : class;
        ITransitBus Start();
    }
}