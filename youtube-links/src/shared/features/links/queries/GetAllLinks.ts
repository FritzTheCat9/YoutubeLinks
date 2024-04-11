export namespace GetAllLinks {
	export interface Query {
		playlistId: number;
		downloaded: boolean;
	}

	export interface LinkInfoDto {
		id: number;
		url: string;
		videoId: string;
		title: string;
	}
}
