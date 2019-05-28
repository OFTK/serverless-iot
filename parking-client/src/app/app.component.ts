import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import { map, tap } from 'rxjs/operators';

interface SignalRConnection {
  url: string;
  accessToken: string;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  private readonly negotiateConnectionUrl = 'http://localhost:7071/api/negotiate';

  private hub: HubConnection;

  private parkingSpots = 3;

  constructor(private readonly http: HttpClient) {
    this.http
      .post<SignalRConnection>(this.negotiateConnectionUrl, null)
      .pipe(
        map(connection => new HubConnectionBuilder().withUrl(connection.url, { accessTokenFactory: () => connection.accessToken }).build()),
        tap(hub =>
          hub.on('parking-state-change', data => {
            if (data.Action === 1) {
              if (this.parkingSpots > 0) {
                this.parkingSpots--;
              }
            } else {
              this.parkingSpots++;
            }
          })
        )
      )
      .subscribe(newHub => newHub.start());
  }
}
