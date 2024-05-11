export interface JwtDto {
  accessToken: string;
  refreshToken: string;
}

export namespace Jwt {
  export const Dto: string = 'JwtDto';
  export const AuthnticationType: string = 'Jwt';
}
