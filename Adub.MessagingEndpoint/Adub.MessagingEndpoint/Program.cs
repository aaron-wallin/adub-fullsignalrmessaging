using Adub.MessagingContracts;
using Adub.MessagingContracts.Commands;
using Adub.MessagingEndpoint.IoC;
using Adub.MessagingEndpoint.MessageBus;
using System;
using System.Threading;

namespace Adub.MessagingEndpoint
{
    class Program
    {
        static void Main(string[] args)
        {
            var bus = DependencyFactory.Instance.GetInstance<ITransitBus>().Start();

            do
            {
                bus.Send<ISampleCommand>(new ISampleCommand() 
                {
                    CommandMessage = $"Hi {DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}"
                });
                Thread.Sleep(10000);
            } while (true);
        }
    }
}
