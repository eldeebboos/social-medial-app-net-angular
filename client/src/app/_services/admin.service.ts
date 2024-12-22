import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { User } from '../_models/User';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;

  getUsersWithRoles() {
    return this.http.get<User[]>(this.baseUrl + 'admin/users-with-roles');
  }

  updateUserRoles(username: string, roles: string[]) {
    return this.http.post(
      `${this.baseUrl}admin/edit-roles/${username}?roles=${roles}`,
      {}
    );
  }
}
