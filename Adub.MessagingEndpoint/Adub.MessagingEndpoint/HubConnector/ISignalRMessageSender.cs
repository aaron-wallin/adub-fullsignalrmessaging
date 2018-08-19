using System.Threading.Tasks;

namespace Adub.MessagingEndpoint.HubConnector
{
    public interface ISignalRMessageSender
    {
        Task Send(string message);
    }
}