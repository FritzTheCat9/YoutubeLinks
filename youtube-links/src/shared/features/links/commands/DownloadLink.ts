import { YoutubeFileType } from '../helpers/YoutubeFileType';

export namespace DownloadLink {
	export interface Command {
		url: string;
		youtubeFileType: YoutubeFileType;
	}
}
