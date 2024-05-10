import { Jwt, type JwtDto } from "@/shared/features/users/responses/JwtDto";

class JwtProvider {
    getJwtDto(): JwtDto | null {
        const tokenString = localStorage.getItem(Jwt.Dto);
        if (tokenString) {
            return JSON.parse(tokenString) as JwtDto;
        }
        return null;
    }

    setJwtDto(token: JwtDto): void {
        localStorage.setItem(Jwt.Dto, JSON.stringify(token));
    }

    removeJwtDto(): void {
        localStorage.removeItem(Jwt.Dto);
    }
}

export const jwtProvider = new JwtProvider();