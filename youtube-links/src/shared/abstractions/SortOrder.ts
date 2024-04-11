export enum SortOrder {
	None = 0,
	Ascending = 1,
	Descending = 2,
}

export const sortingDirectionToEnum = (direction: string) => {
	switch (direction) {
		case 'asc':
			return SortOrder.Ascending;
		case 'desc':
			return SortOrder.Descending;
		default:
			return SortOrder.None;
	}
};
