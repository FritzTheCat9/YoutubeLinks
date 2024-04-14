import type { SortOrder } from '@/shared/abstractions/SortOrder';

export namespace GetAllPaginatedLinks {
  export interface Query {
    page: number;
    pageSize: number;
    sortColumn: string;
    sortOrder: SortOrder;
    searchTerm: string;
    playlistId: number;
  }
}
