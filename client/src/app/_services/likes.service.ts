import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Member } from '../_models/member';
import { PageinatedResult } from '../_models/pageination';
import {
  setPageinatedResponse,
  setPaginationHeaders,
} from './PaginatedHelpers';

@Injectable({
  providedIn: 'root',
})
export class LikesService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  likeIds = signal<number[]>([]);
  pageinatedResult = signal<PageinatedResult<Member[]> | null>(null);

  toogleLike(targetId: number) {
    return this.http.post(this.baseUrl + 'likes/' + targetId, {});
  }

  getLikes(predicate: string, pageNumber: number, pageSize: number) {
    let params = setPaginationHeaders(pageNumber, pageSize);
    params = params.append('perdicate', predicate);

    return this.http
      .get<Member[]>(this.baseUrl + 'likes', {
        observe: 'response',
        params,
      })
      .subscribe((response) =>
        setPageinatedResponse(response, this.pageinatedResult)
      );
  }
  getLikeIds() {
    this.http.get<number[]>(this.baseUrl + 'likes/list').subscribe({
      next: (ids) => {
        this.likeIds.set(ids);
      },
    });
  }
}
