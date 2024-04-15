export interface LinkDto {
  id: number;
  created: Date;
  modified: Date;
  url: string;
  videoId: string;
  title: string;
  downloaded: boolean;
  playlistId: number;
}
