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

  // TODO: Add validators and form model with a file

  // export interface FormModel extends Command {
  //     file: BrowserFile;
  // }
}
