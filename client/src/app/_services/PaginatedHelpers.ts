import { HttpParams, HttpResponse } from '@angular/common/http';
import { signal } from '@angular/core';
import { PageinatedResult } from '../_models/pageination';

export function setPageinatedResponse<T>(
  response: HttpResponse<T>,
  pageinatedResultSignal: ReturnType<typeof signal<PageinatedResult<T> | null>>
) {
  pageinatedResultSignal.set({
    items: response.body as T,
    pageination: JSON.parse(response.headers.get('Pageination')!),
  });
}

export function setPaginationHeaders(pageNumber: number, pageSize: number) {
  let params = new HttpParams();

  if (pageNumber && pageSize) {
    params = params.append('pageNumber', pageNumber);
    params = params.append('pageSize', pageSize);
  }
  return params;
}
