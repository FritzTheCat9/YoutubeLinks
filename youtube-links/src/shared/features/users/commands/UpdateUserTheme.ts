import type { ThemeColor } from '../helpers/ThemeColor';

export namespace UpdateUserTheme {
  export interface Command {
    id: number;
    themeColor: ThemeColor;
  }
}
