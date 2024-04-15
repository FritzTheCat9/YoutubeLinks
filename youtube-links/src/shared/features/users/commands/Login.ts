import { ValidationConsts } from '@/shared/localization/ValidationConsts';

export namespace Login {
  export interface Command {
    email: string;
    password: string;
  }

  export const Validation = {
    email: {
      notEmpty: (v: string) => !!v || 'Email should not be empty.',
      maximumLength: (v: string) =>
        v.length <= ValidationConsts.MaximumStringLength ||
        `The length of email must be ${ValidationConsts.MaximumStringLength} characters or fewer. You entered ${v.length} characters.`,
      isEmailAddress: (v: string) =>
        ValidationConsts.isValidEmail(v) || 'Email address is not valid.',
    },

    password: {
      notEmpty: (v: string) => !!v || 'Password should not be empty.',
      minimumLength: (v: string) =>
        v.length >= ValidationConsts.MinimumStringLength ||
        `The length of password must be at least ${ValidationConsts.MinimumStringLength} characters. You entered ${v.length} characters.`,
      maximumLength: (v: string) =>
        v.length <= ValidationConsts.MaximumStringLength ||
        `The length of password must be ${ValidationConsts.MaximumStringLength} characters or fewer. You entered ${v.length} characters.`,
    },
  };
}
