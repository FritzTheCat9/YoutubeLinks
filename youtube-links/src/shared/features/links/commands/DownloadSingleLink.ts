import { ValidationConsts } from '@/shared/localization/ValidationConsts';
import { YoutubeFileType, isValidYoutubeFileTypeValue } from '../helpers/YoutubeFileType';

export namespace DownloadSingleLink {
  export interface Command {
    url: string;
    youtubeFileType: YoutubeFileType;
  }

  export const Validation = {
    url: {
      notEmpty: (v: string) => !!v || 'Youtube video url should not be empty.',
      matchesYoutubeVideoUrlRegex: (v: string) =>
        ValidationConsts.matchesYoutubeVideoUrlRegex(v) ||
        'This is not a valid link to the YouTube video.',
    },
    youtubeFileType: {
      isInEnum: (v: string) =>
        isValidYoutubeFileTypeValue(v) ||
        `YoutubeFileType has a range of values which does not include: ${v}.`,
    },
  };
}
