using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Users.Commands;
using YoutubeLinks.Shared.Features.Users.Queries;
using YoutubeLinks.Shared.Features.Users.Responses;

namespace YoutubeLinks.Shared.Clients;

public interface IUserApiClient
{
    Task<bool> ConfirmEmail(ConfirmEmail.Command command);
    Task ForgotPassword(ForgotPassword.Command command);
    Task<JwtDto> Login(Login.Command command);
    Task<JwtDto> RefreshToken(RefreshToken.Command command);
    Task<int> Register(Register.Command command);
    Task ResendConfirmationEmail(ResendConfirmationEmail.Command command);
    Task<bool> ResetPassword(ResetPassword.Command command);
    Task UpdateUserTheme(UpdateUserTheme.Command command);
    Task<PagedList<UserDto>> GetAllUsers(GetAllUsers.Query query);
    Task<UserDto> GetUser(int id);
}

public class UserApiClient(IApiClient apiClient) : IUserApiClient
{
    private const string Url = "api/users";

    public async Task<bool> ConfirmEmail(ConfirmEmail.Command command)
    {
        return await apiClient.Post<ConfirmEmail.Command, bool>($"{Url}/confirmEmail", command);
    }

    public async Task ForgotPassword(ForgotPassword.Command command)
    {
        await apiClient.Post($"{Url}/forgotPassword", command);
    }

    public async Task<JwtDto> Login(Login.Command command)
    {
        return await apiClient.Post<Login.Command, JwtDto>($"{Url}/login", command);
    }

    public async Task<JwtDto> RefreshToken(RefreshToken.Command command)
    {
        return await apiClient.Post<RefreshToken.Command, JwtDto>($"{Url}/refresh-token", command);
    }

    public async Task<int> Register(Register.Command command)
    {
        return await apiClient.Post<Register.Command, int>($"{Url}/register", command);
    }

    public async Task ResendConfirmationEmail(ResendConfirmationEmail.Command command)
    {
        await apiClient.Post($"{Url}/resendConfirmationEmail", command);
    }

    public async Task<bool> ResetPassword(ResetPassword.Command command)
    {
        return await apiClient.Post<ResetPassword.Command, bool>($"{Url}/resetPassword", command);
    }

    public async Task UpdateUserTheme(UpdateUserTheme.Command command)
    {
        await apiClient.Put($"{Url}/{command.Id}/theme", command);
    }

    public async Task<PagedList<UserDto>> GetAllUsers(GetAllUsers.Query query)
    {
        return await apiClient.Post<GetAllUsers.Query, PagedList<UserDto>>($"{Url}/all", query);
    }

    public async Task<UserDto> GetUser(int id)
    {
        return await apiClient.Get<UserDto>($"{Url}/{id}");
    }
}