import { ExceptionType } from "./ExceptionType";

export interface ErrorResponse {
    type: ExceptionType;
    message: string;
}

export interface ValidationErrorResponse extends ErrorResponse {
    errors: { [key: string]: string[]; };
}

export interface ServerErrorResponse extends ErrorResponse {

}

export interface UnauthorizedErrorResponse extends ErrorResponse {

}

export interface ForbiddenErrorResponse extends ErrorResponse {

}

export interface NotFoundErrorResponse extends ErrorResponse {

}