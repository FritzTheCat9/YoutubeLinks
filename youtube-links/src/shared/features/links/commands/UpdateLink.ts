import { ValidationConsts } from '@/shared/localization/ValidationConsts';
import { YoutubeHelpers } from '../helpers/YoutubeHelpers';

export namespace UpdateLink {
  export interface Command {
    id: number;
    url: string;
    title: string;
    downloaded: boolean;
  }

  export const Validation = {
    url: {
      notEmpty: (v: string) => !!v || 'Youtube video url should not be empty.',
      matchesYoutubeVideoUrlRegex: (v: string) =>
        ValidationConsts.matchesYoutubeVideoUrlRegex(v) ||
        'This is not a valid link to the YouTube video.',
    },
    title: {
      mustHaveValidCharactersInTitle: (v: string) =>
        !v.trim() ||
        YoutubeHelpers.haveValidCharactersInTitle(v) ||
        'Title contains invalid characters.',
      maximumLength: (v: string) =>
        v.length <= ValidationConsts.MaximumStringLength ||
        `The length of title must be ${ValidationConsts.MaximumStringLength} characters or fewer. You entered ${v.length} characters.`,
    },
  };
}
