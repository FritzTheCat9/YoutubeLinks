export namespace ValidationConsts {

    export const MaximumStringLength: number = 50;
    export const MinimumStringLength: number = 7;
    export const UserNameRegex: string = "^[a-zA-Z0-9_]+$";
    export const YoutubeVideoUrlRegex: string = "^(?:https?:\\/\\/)?(?:www\\.)?(?:youtube\\.com\\/(?:[^\\/\\n\\s]+\\/\\S+\\/|(?:v|e(?:mbed)?)\\/|\\S*?[?&]v=)|youtu\\.be\\/)([a-zA-Z0-9_-]{11})";
}