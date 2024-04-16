export enum ThemeColor {
  System,
  Light,
  Dark,
}

export function isValidThemeColorValue(value: any): value is ThemeColor {
  return Object.values(ThemeColor).includes(value);
}
