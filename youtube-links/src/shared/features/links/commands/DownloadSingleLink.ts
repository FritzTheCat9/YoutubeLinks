import { YoutubeFileType } from '../helpers/YoutubeFileType';

export namespace DownloadSingleLink {
  export interface Command {
    url: string;
    youtubeFileType: YoutubeFileType;
  }
}
