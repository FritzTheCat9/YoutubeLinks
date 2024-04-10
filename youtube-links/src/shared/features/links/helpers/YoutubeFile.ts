import { YoutubeFileType } from "./YoutubeFileType";

export interface YoutubeFile {
    fileBytes: string;  //byte[]
    contentType: string;
    fileName: string;
    youtubeFileType: YoutubeFileType;
}