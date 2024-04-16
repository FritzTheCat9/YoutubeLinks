import { isValidThemeColorValue, type ThemeColor } from '../helpers/ThemeColor';

export namespace UpdateUserTheme {
  export interface Command {
    id: number;
    themeColor: ThemeColor;
  }

  export const Validation = {
    themeColor: {
      isInEnum: (v: string) =>
        isValidThemeColorValue(v) ||
        `ThemeColor has a range of values which does not include: ${v}.`,
    },
  };
}
