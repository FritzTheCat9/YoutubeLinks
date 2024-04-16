import { isValidPlaylistFileTypeValue, type PlaylistFileType } from '../helpers/PlaylistFileType';

export namespace ExportPlaylist {
  export interface Command {
    id: number;
    playlistFileType: PlaylistFileType;
  }

  export const Validation = {
    playlistFileType: {
      isInEnum: (v: string) =>
        isValidPlaylistFileTypeValue(v) ||
        `PlaylistFileType has a range of values which does not include: ${v}.`,
    },
  };
}
