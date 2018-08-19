using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Adub.MessagingEndpoint.HubConnector
{
    public class SignalRMessageSender : ISignalRMessageSender
    {
        private HubConnection _hubConnection;
        private readonly string _hubUrl = "https://adub-signalr-hub.apps.pcf.nonprod.cudirect.com/notificationhub";
        private bool _isStarted = false;

        public SignalRMessageSender()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_hubUrl)
                .Build();        
        }

        private async Task TryStart()
        {
            if (_isStarted) await Task.CompletedTask;
            await _hubConnection.StartAsync();
            _isStarted = true;
        }

        public async Task Send(string message)
        {
            try
            {
                await this.TryStart();
                await _hubConnection.InvokeAsync("SendMessage", "System:MassTransit", message);
            }
            catch(Exception ex)
            {
                await Console.Error.WriteLineAsync($"Error occurred trying to send to SignalRHub {_hubUrl}");
                await Console.Error.WriteLineAsync(ex.ToString());
            }
            finally
            {
                //await _hubConnection.StopAsync();
            }
        }
    }
}
