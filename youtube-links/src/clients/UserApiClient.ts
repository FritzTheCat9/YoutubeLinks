import type { PagedList } from "@/shared/abstractions/PagedList";
import type { ConfirmEmail } from "@/shared/features/users/commands/ConfirmEmail";
import type { Login } from "@/shared/features/users/commands/Login";
import type { Register } from "@/shared/features/users/commands/Register";
import type { UpdateUserTheme } from "@/shared/features/users/commands/UpdateUserTheme";
import type { GetAllUsers } from "@/shared/features/users/queries/GetAllUsers";
import type { JwtDto } from "@/shared/features/users/responses/JwtDto";
import type { UserDto } from "@/shared/features/users/responses/UserDto";
import { apiClient, type IApiClient } from "./ApiClient";

export interface IUserApiClient {
    ConfirmEmail(command: ConfirmEmail.Command): Promise<boolean>;
    Login(command: Login.Command): Promise<JwtDto>;
    Register(command: Register.Command): Promise<void>;
    UpdateUserTheme(command: UpdateUserTheme.Command): Promise<void>;
    GetAllUsers(query: GetAllUsers.Query): Promise<PagedList<UserDto>>;
    GetUser(id: number): Promise<UserDto>;
}

export class UserApiClient implements IUserApiClient {
    private readonly apiClient: IApiClient;
    private readonly url: string = "api/users";

    constructor() {
        this.apiClient = apiClient;
    }

    async ConfirmEmail(command: ConfirmEmail.Command): Promise<boolean> {
        return await this.apiClient.post<ConfirmEmail.Command, boolean>(`${this.url}/confirmEmail`, command);
    }

    async Login(command: Login.Command): Promise<JwtDto> {
        return await this.apiClient.post<Login.Command, JwtDto>(`${this.url}/login`, command);
    }

    async Register(command: Register.Command): Promise<void> {
        return await this.apiClient.postWithoutResponse<Register.Command>(`${this.url}/register`, command);
    }

    async UpdateUserTheme(command: UpdateUserTheme.Command): Promise<void> {
        return await this.apiClient.put<UpdateUserTheme.Command>(`${this.url}/${command.id}/theme`, command);
    }

    async GetAllUsers(query: GetAllUsers.Query): Promise<PagedList<UserDto>> {
        return await this.apiClient.post<GetAllUsers.Query, PagedList<UserDto>>(`${this.url}/all`, query);
    }

    async GetUser(id: number): Promise<UserDto> {
        return await this.apiClient.getWithResponse<UserDto>(`${this.url}/${id}`);
    }
}

export const userApiClient = new UserApiClient();