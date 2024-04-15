import { ValidationConsts } from '@/shared/localization/ValidationConsts';

export namespace CreateLink {
  export interface Command {
    url: string;
    playlistId: number;
  }

  export const Validation = {
    url: {
      notEmpty: (v: string) => !!v || 'Youtube video url should not be empty.',
      matchesYoutubeVideoUrlRegex: (v: string) =>
        ValidationConsts.matchesYoutubeVideoUrlRegex(v) ||
        'This is not a valid link to the YouTube video.',
    },
  };
}
