import type { SortOrder } from "@/shared/abstractions/SortOrder";

export namespace GetAllUsers {

    export interface Query {
        page: number;
        pageSize: number;
        sortColumn: string;
        sortOrder: SortOrder;
        searchTerm: string;
    }
}