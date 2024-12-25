import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { User } from '../_models/User';
import { map } from 'rxjs';
import { environment } from '../../environments/environment';
import { LikesService } from './likes.service';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private http = inject(HttpClient);
  private likeService = inject(LikesService);
  private presenceService = inject(PresenceService);
  URL = environment.apiUrl + 'account';
  currentUser = signal<User | null>(null);
  roles = computed(() => {
    const user = this.currentUser();
    if (user && user.token) {
      //decode the token
      const role = JSON.parse(atob(user.token.split('.')[1])).role;
      return Array.isArray(role) ? role : [role];
    }
    return null;
  });

  login(model: any) {
    return this.http.post<User>(`${this.URL}/login`, model).pipe(
      map((user) => {
        if (user) {
          this.setCurrentUser(user);
        }
        return user;
      })
    );
  }

  register(model: any) {
    return this.http.post<User>(`${this.URL}/register`, model).pipe(
      map((user) => {
        if (user) {
          this.setCurrentUser(user);
        }
        return user;
      })
    );
  }

  setCurrentUser(user: User) {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUser.set(user);
    this.likeService.getLikeIds();

    this.presenceService.createConnection(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUser.set(null);
    this.presenceService.stopHubConnection();
  }
}
