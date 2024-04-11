export interface PagedList<T> {
	items: T[];
	page: number;
	pageSize: number;
	totalCount: number;
	pagesCount: number;
	hasPreviousPage: boolean;
	hasNextPage: boolean;
}
