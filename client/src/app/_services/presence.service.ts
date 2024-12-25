import { inject, Injectable, signal } from '@angular/core';
import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
} from '@microsoft/signalr';
import { environment } from '../../environments/environment';
import { ToastrService } from 'ngx-toastr';
import { User } from '../_models/User';

@Injectable({
  providedIn: 'root',
})
export class PresenceService {
  hubUrl = environment.hubsUrl;
  private hubConnection?: HubConnection;
  private toaster = inject(ToastrService);
  onlineUsers = signal<string[]>([]);

  createConnection(user: User) {
    //sending a tocken to signalR hub
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token,
      })
      // reconnect if connection lost
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch((err) => console.log(err));

    //receiving a message from the server
    this.hubConnection.on('UserIsOnline', (username) => {
      this.toaster.info(username + ' has connected');
    });

    this.hubConnection.on('UserIsOffline', (username) => {
      this.toaster.warning(username + ' has disconnected');
    });

    this.hubConnection.on('GetOnlineUsers', (usernames) => {
      this.onlineUsers.set(usernames);
    });
  }

  stopHubConnection() {
    if (this.hubConnection?.state === HubConnectionState.Connecting) {
      this.hubConnection.stop().catch((err) => console.log(err));
    }
  }
}
