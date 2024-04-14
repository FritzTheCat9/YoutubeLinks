import type { ThemeColor } from '../helpers/ThemeColor';

export namespace Register {
  export interface Command {
    email: string;
    userName: string;
    password: string;
    repeatPassword: string;
    themeColor: ThemeColor;
  }
}
