import type { ThemeColor } from "../helpers/ThemeColor";

export interface UserDto {
    id: number;
    created: Date;      //DateTime
    modified: Date;     //DateTime
    userName: string;
    email: string;
    isAdmin: boolean;
    themeColor: ThemeColor;
}