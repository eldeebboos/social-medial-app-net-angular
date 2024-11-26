import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  private http = inject(HttpClient);
  url = environment.apiUrl;

  getMembers() {
    return this.http.get<Member[]>(this.url + 'users');
  }
  getMember(username: string) {
    return this.http.get<Member>(this.url + 'users/' + username);
  }
  getMemberById(id: number) {
    return this.http.get<Member>(this.url + 'users/' + id);
  }
}
