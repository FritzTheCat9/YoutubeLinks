import { Jwt, type JwtDto } from "@/shared/features/users/responses/JwtDto";

class JwtProvider {
    getJwtDto(): JwtDto | undefined {
        const tokenString = localStorage.getItem(Jwt.Dto);
        if (tokenString) {
            return JSON.parse(tokenString) as JwtDto;
        }
        return undefined;
    }

    setJwtDto(token: JwtDto | undefined): void {
        if(token != undefined)
            localStorage.setItem(Jwt.Dto, JSON.stringify(token));
    }

    removeJwtDto(): void {
        localStorage.removeItem(Jwt.Dto);
    }
}

export const jwtProvider = new JwtProvider();