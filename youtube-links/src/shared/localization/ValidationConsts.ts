export namespace ValidationConsts {
  export const MaximumStringLength: number = 50;
  export const MinimumStringLength: number = 7;
  export const EmailRegex: RegExp = /\S+@\S+\.\S+/;
  export const UserNameRegex: RegExp = /^[a-zA-Z0-9_]+$/;
  export const YoutubeVideoUrlRegex: RegExp =
    /^(?:https?:\/\/)?(?:www\.)?(?:youtube\.com\/(?:[^\/\n\s]+\/\S+\/|(?:v|e(?:mbed)?)\/|\S*?[?&]v=)|youtu\.be\/)([a-zA-Z0-9_-]{11})$/;

  export function isValidEmail(email: string) {
    return EmailRegex.test(email);
  }

  export function matchesUserNameRegex(userName: string) {
    return UserNameRegex.test(userName);
  }

  export function matchesYoutubeVideoUrlRegex(url: string) {
    return YoutubeVideoUrlRegex.test(url);
  }
}
