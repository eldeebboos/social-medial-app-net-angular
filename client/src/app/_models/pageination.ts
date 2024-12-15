export interface Pageination {
  currentPage: number;
  itemsPerPage: number;
  totalItems: number;
  totalPages: number;
}

export interface PageinatedResult<T> {
  items?: T;
  pageination?: Pageination;
}
