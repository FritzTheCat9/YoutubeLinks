import type { LinkJSONModel } from '../helpers/PlaylistFile';
import type { PlaylistFileType } from '../helpers/PlaylistFileType';

export namespace ImportPlaylist {
  export interface Command {
    name: string;
    public: boolean;
    exportedLinks: LinkJSONModel[];
    exportedLinkUrls: string[];
    playlistFileType: PlaylistFileType;
  }

  // export interface FormModel extends Command {
  //     file: BrowserFile;
  // }
}
