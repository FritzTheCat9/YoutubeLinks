import { ValidationConsts } from '@/shared/localization/ValidationConsts';
import { isValidThemeColorValue, type ThemeColor } from '../helpers/ThemeColor';

export namespace Register {
  export interface Command {
    email: string;
    userName: string;
    password: string;
    repeatPassword: string;
    themeColor: ThemeColor;
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
    userName: {
      notEmpty: (v: string) => !!v || 'User name should not be empty.',
      minimumLength: (v: string) =>
        v.length >= ValidationConsts.MinimumStringLength ||
        `The length of user name must be at least ${ValidationConsts.MinimumStringLength} characters. You entered ${v.length} characters.`,
      maximumLength: (v: string) =>
        v.length <= ValidationConsts.MaximumStringLength ||
        `The length of user name must be ${ValidationConsts.MaximumStringLength} characters or fewer. You entered ${v.length} characters.`,
      matchesUserNameRegex: (v: string) =>
        ValidationConsts.matchesUserNameRegex(v) ||
        'User name can contain only: a-z, A-Z, 0-9 and _ characters.',
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
    repeatPassword: {
      notEmpty: (v: string) => !!v || 'Password should not be empty.',
      minimumLength: (v: string) =>
        v.length >= ValidationConsts.MinimumStringLength ||
        `The length of password must be at least ${ValidationConsts.MinimumStringLength} characters. You entered ${v.length} characters.`,
      maximumLength: (v: string) =>
        v.length <= ValidationConsts.MaximumStringLength ||
        `The length of password must be ${ValidationConsts.MaximumStringLength} characters or fewer. You entered ${v.length} characters.`,
      equalPassword: (v: string, password: string) =>
        v == password || 'The passwords entered must match.',
    },
    themeColor: {
      isInEnum: (v: string) =>
        isValidThemeColorValue(v) ||
        `ThemeColor has a range of values which does not include: ${v}.`,
    },
  };
}
