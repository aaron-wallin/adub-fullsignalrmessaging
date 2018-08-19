import { Component, OnInit } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent implements OnInit {
  title = 'Adub-SignalR-WebClient';

  private _hubConnection: HubConnection;
  msgs: string[] = [];

  constructor() { }

  ngOnInit(): void {
    this._hubConnection = new HubConnectionBuilder()
      .withUrl('https://adub-signalr-hub.apps.pcf.nonprod.cudirect.com/notificationhub')
      .build();
    this._hubConnection
      .start()
      .then(() => console.log('Connection started!'))
      .catch(err => console.log('Error while establishing connection :('));

    this._hubConnection.on('ReceiveMessage', (type: string, payload: string) => {
      console.log(payload);
      this.msgs.push(payload);
    });
  }
}
