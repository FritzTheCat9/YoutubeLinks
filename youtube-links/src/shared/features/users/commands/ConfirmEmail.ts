import { ValidationConsts } from '@/shared/localization/ValidationConsts';

export namespace ConfirmEmail {
  export interface Command {
    email: string;
    token: string;
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
    token: {
      notEmpty: (v: string) => !!v || 'Token should not be empty.',
    },
  };
}
