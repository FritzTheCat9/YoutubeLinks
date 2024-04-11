import { PlaylistFileType } from './PlaylistFileType';

export namespace PlaylistHelpers {
	export function playlistFileTypeToString(playlistFileType: PlaylistFileType): string {
		switch (playlistFileType) {
			case PlaylistFileType.TXT:
				return 'txt';
			default:
				return 'json';
		}
	}
}
