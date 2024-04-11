import type { PlaylistFileType } from '../helpers/PlaylistFileType';

export namespace ExportPlaylist {
	export interface Command {
		id: number;
		playlistFileType: PlaylistFileType;
	}
}
