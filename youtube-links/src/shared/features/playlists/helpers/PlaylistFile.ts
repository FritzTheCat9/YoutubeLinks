import { PlaylistFileType } from './PlaylistFileType';

export interface PlaylistFile {
	fileBytes: string; // byte[]
	contentType: string;
	fileName: string;
	playlistFileType: PlaylistFileType;
}

export interface PlaylistJSONModel {
	linksCount: number;
	linkModels: LinkJSONModel[];
}

export interface LinkJSONModel {
	title: string;
	url: string;
	videoId: string;
}
