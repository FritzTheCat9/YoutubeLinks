import { ValidationConsts } from '@/shared/localization/ValidationConsts';

export namespace CreatePlaylist {
  export interface Command {
    name: string;
    public: boolean;
  }

  export const Validation = {
    name: {
      notEmpty: (v: string) => !!v || 'Name should not be empty.',
      maximumLength: (v: string) =>
        v.length <= ValidationConsts.MaximumStringLength ||
        `The length of name must be ${ValidationConsts.MaximumStringLength} characters or fewer. You entered ${v.length} characters.`,
    },
  };
}
