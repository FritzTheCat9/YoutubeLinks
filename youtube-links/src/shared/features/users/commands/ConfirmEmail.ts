export namespace ConfirmEmail {

    export interface Command {
        email: string;
        token: string;
    }
}