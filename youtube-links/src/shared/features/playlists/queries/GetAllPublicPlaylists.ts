import { SortOrder } from '@/shared/abstractions/SortOrder';

export namespace GetAllPublicPlaylists {
	export interface Query {
		page: number;
		pageSize: number;
		sortColumn: string;
		sortOrder: SortOrder;
		searchTerm: string;
	}
}
