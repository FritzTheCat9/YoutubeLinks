import { YoutubeFileType, isValidYoutubeFileTypeValue } from '../helpers/YoutubeFileType';

export namespace DownloadLink {
  export interface Command {
    id: number;
    youtubeFileType: YoutubeFileType;
  }

  export const Validation = {
    youtubeFileType: {
      isInEnum: (v: string) =>
        isValidYoutubeFileTypeValue(v) ||
        `YoutubeFileType has a range of values which does not include: ${v}.`,
    },
  };
}
