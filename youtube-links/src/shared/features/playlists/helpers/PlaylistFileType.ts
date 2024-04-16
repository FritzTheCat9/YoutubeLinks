export enum PlaylistFileType {
  JSON,
  TXT,
}

export function isValidPlaylistFileTypeValue(value: any): value is PlaylistFileType {
  return Object.values(PlaylistFileType).includes(value);
}
