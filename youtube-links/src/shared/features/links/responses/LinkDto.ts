export interface LinkDto {
	id: number;
	created: Date; // Date
	modified: Date; // Date
	url: string;
	videoId: string;
	title: string;
	downloaded: boolean;
	playlistId: number;
}
