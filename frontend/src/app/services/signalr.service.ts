import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class SignalRService {
  private hubConnection?: signalR.HubConnection;

  startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(environment.apiUrl.replace('/api','') + '/orderHub')
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch(err => console.error('SignalR start error', err));
  }

  on(event: string, callback: (...args: any[]) => void) {
    this.hubConnection?.on(event, callback);
  }
}
