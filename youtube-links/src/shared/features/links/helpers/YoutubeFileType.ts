export enum YoutubeFileType {
  MP3,
  MP4,
}

export function isValidYoutubeFileTypeValue(value: any): value is YoutubeFileType {
  return Object.values(YoutubeFileType).includes(value);
}
