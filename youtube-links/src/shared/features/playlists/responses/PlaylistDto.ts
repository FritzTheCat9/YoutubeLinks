export interface PlaylistDto {
	id: number;
	created: Date; // DateTime
	modified: Date; // DateTime
	name: string;
	public: boolean;
	userId: number;
}
