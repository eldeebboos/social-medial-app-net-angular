import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import { of, tap } from 'rxjs';
import { Photo } from '../_models/photo';
import { PageinatedResult } from '../_models/pageination';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import {
  setPageinatedResponse,
  setPaginationHeaders,
} from './PaginatedHelpers';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  private http = inject(HttpClient);
  private accountService = inject(AccountService);
  url = environment.apiUrl;
  pageinatedResult = signal<PageinatedResult<Member[]> | null>(null);
  memberCache = new Map();
  user = this.accountService.currentUser();
  userParams = signal<UserParams>(new UserParams(this.user));

  resetUserParams() {
    this.userParams.set(new UserParams(this.user));
  }

  getMembers() {
    const response = this.memberCache.get(
      Object.values(this.userParams).join('-')
    );

    //get the data from cache
    if (response) return setPageinatedResponse(response, this.pageinatedResult);
    let params = setPaginationHeaders(
      this.userParams().pageNumber,
      this.userParams().pageSize
    );
    params = params.append('minAge', this.userParams().minAge);
    params = params.append('maxAge', this.userParams().maxAge);
    params = params.append('gender', this.userParams().gender);
    params = params.append('orderBy', this.userParams().orderBy);

    return this.http
      .get<Member[]>(this.url + 'users', { observe: 'response', params })
      .subscribe({
        next: (response) => {
          setPageinatedResponse(response, this.pageinatedResult);
          this.memberCache.set(
            Object.values(this.userParams()).join('-'),
            response
          );
        },
      });
  }

  getMember(username: string) {
    //get from cache
    const member: Member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.body), [])
      .find((m: Member) => m.userName === username);

    if (member) return of(member);

    return this.http.get<Member>(this.url + 'users/' + username);
  }
  getMemberById(id: number) {
    return this.http.get<Member>(this.url + 'users/' + id);
  }

  updateMember(member: Member) {
    return this.http.put(this.url + 'users', member);
    // .pipe(
    //   tap(() => {
    //     this.members.update((members) =>
    //       members.map((m) => (m.userName === member.userName ? member : m))
    //     );
    //   })
    // );
  }

  setMainPhoto(photo: Photo) {
    return this.http.put(this.url + 'users/set-main-photo/' + photo.id, {});
    // .pipe(
    //   tap(() => {
    //     this.members.update((members) => {
    //       return members.map((m) => {
    //         if (m.photos.includes(photo)) {
    //           m.photoUrl = photo.url;
    //         }
    //         return m;
    //       });
    //     });
    //   })
    // );
  }

  deletePhoto(photo: Photo) {
    return this.http.delete(this.url + 'users/delete-photo/' + photo.id);
    // .pipe(
    // tap(() => {
    //   this.members.update((members) => {
    //     return members.map((m) => {
    //       if (m.photos.includes(photo)) {
    //         m.photos = m.photos.filter((p) => p.id !== photo.id);
    //       }
    //       return m;
    //     });
    //   });
    // })
    // );
  }
}
