import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import { of, tap } from 'rxjs';
import { Photo } from '../_models/photo';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  private http = inject(HttpClient);
  url = environment.apiUrl;
  members = signal<Member[]>([]);

  getMembers() {
    return this.http.get<Member[]>(this.url + 'users').subscribe({
      next: (members) => {
        this.members.set(members);
      },
    });
  }
  getMember(username: string) {
    const member = this.members().find((m) => m.userName === username);
    if (member) return of(member);
    return this.http.get<Member>(this.url + 'users/' + username);
  }
  getMemberById(id: number) {
    return this.http.get<Member>(this.url + 'users/' + id);
  }

  updateMember(member: Member) {
    return this.http.put(this.url + 'users', member).pipe(
      tap(() => {
        this.members.update((members) =>
          members.map((m) => (m.userName === member.userName ? member : m))
        );
      })
    );
  }

  setMainPhoto(photo: Photo) {
    return this.http
      .put(this.url + 'users/set-main-photo/' + photo.id, {})
      .pipe(
        tap(() => {
          this.members.update((members) => {
            return members.map((m) => {
              if (m.photos.includes(photo)) {
                m.photoUrl = photo.url;
              }
              return m;
            });
          });
        })
      );
  }

  deletePhoto(photo: Photo) {
    return this.http.delete(this.url + 'users/delete-photo/' + photo.id).pipe(
      tap(() => {
        this.members.update((members) => {
          return members.map((m) => {
            if (m.photos.includes(photo)) {
              m.photos = m.photos.filter((p) => p.id !== photo.id);
            }
            return m;
          });
        });
      })
    );
  }
}
