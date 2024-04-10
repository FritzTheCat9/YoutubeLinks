import { YoutubeFileType } from "./YoutubeFileType";

export namespace YoutubeHelpers {

    export const VideoIdRegex: string = "(?:\?|&)v=([^&]+)";
    export const VideoPathBase: string = "https://www.youtube.com/watch?v=";
    export const MaximumTitleLength: number = 255;

    export function youtubeFileTypeToString(youtubeFileType: YoutubeFileType): string {
        switch (youtubeFileType) {
            case YoutubeFileType.MP4:
                return "mp4";
            default:
                return "mp3";
        }
    }

    export function haveValidCharactersInTitle(title: string): boolean {
        const invalidChars: string = getInvalidPathAndFileNameCharacters();
        return !containsInvalidChars(title, invalidChars);
    }

    function getInvalidPathAndFileNameCharacters(): string {
        return [...getInvalidFileNameChars(), ...getInvalidPathChars()].join("");
    }

    function getInvalidFileNameChars(): string[] {
        return ['<', '>', ':', '"', '/', '\\', '|', '?', '*'];
    }

    function getInvalidPathChars(): string[] {
        return ['<', '>', ':', '"', '|', '?', '*'];
    }

    function containsInvalidChars(input: string, invalidChars: string): boolean {
        return input.split("").some(char => invalidChars.includes(char));
    }
}