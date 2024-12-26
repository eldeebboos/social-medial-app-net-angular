import { inject, Injectable, signal } from '@angular/core';
import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
} from '@microsoft/signalr';
import { environment } from '../../environments/environment';
import { ToastrService } from 'ngx-toastr';
import { User } from '../_models/User';
import { take } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class PresenceService {
  hubUrl = environment.hubsUrl;
  private hubConnection?: HubConnection;
  private toaster = inject(ToastrService);
  private router = inject(Router);
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
      this.onlineUsers.update((users) => [...users, username]);
    });

    this.hubConnection.on('UserIsOffline', (username) => {
      this.onlineUsers.update((users) => users.filter((u) => u !== username));
    });

    this.hubConnection.on('GetOnlineUsers', (usernames) => {
      this.onlineUsers.set(usernames);
    });

    this.hubConnection.on('NewMessageReceived', ({ username, knownAs }) => {
      this.toaster
        .info(knownAs + ' has sent a new message! Click me to see it')
        .onTap.pipe(take(1))
        .subscribe(() =>
          this.router.navigateByUrl('/members/' + username + '?tab=Messages')
        );
    });
  }

  stopHubConnection() {
    if (this.hubConnection?.state === HubConnectionState.Connecting) {
      this.hubConnection.stop().catch((err) => console.log(err));
    }
  }
}
