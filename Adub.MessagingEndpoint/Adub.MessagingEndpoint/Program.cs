using Adub.MessagingContracts;
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
                bus.Publish(new SampleMessage { Text = $"Hi {DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}" });
                Thread.Sleep(30000);
            } while (true);
        }
    }
}
