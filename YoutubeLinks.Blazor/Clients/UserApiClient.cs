using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Users.Commands;
using YoutubeLinks.Shared.Features.Users.Queries;
using YoutubeLinks.Shared.Features.Users.Responses;

namespace YoutubeLinks.Blazor.Clients;

public interface IUserApiClient
{
    Task<bool> ConfirmEmail(ConfirmEmail.Command command);
    Task ForgotPassword(ForgotPassword.Command command);
    Task<JwtDto> Login(Login.Command command);
    Task<JwtDto> RefreshToken(RefreshToken.Command command);
    Task Register(Register.Command command);
    Task ResendConfirmationEmail(ResendConfirmationEmail.Command command);
    Task<bool> ResetPassword(ResetPassword.Command command);
    Task UpdateUserTheme(UpdateUserTheme.Command command);
    Task<PagedList<UserDto>> GetAllUsers(GetAllUsers.Query query);
    Task<UserDto> GetUser(int id);
}

public class UserApiClient : IUserApiClient
{
    private const string Url = "api/users";
    private readonly IApiClient _apiClient;

    public UserApiClient(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<bool> ConfirmEmail(ConfirmEmail.Command command)
        => await _apiClient.Post<ConfirmEmail.Command, bool>($"{Url}/confirmEmail", command);

    public async Task ForgotPassword(ForgotPassword.Command command)
        => await _apiClient.Post($"{Url}/forgotPassword", command);

    public async Task<JwtDto> Login(Login.Command command)
        => await _apiClient.Post<Login.Command, JwtDto>($"{Url}/login", command);

    public async Task<JwtDto> RefreshToken(RefreshToken.Command command)
        => await _apiClient.Post<RefreshToken.Command, JwtDto>($"{Url}/refresh-token", command);

    public async Task Register(Register.Command command)
        => await _apiClient.Post($"{Url}/register", command);

    public async Task ResendConfirmationEmail(ResendConfirmationEmail.Command command)
        => await _apiClient.Post($"{Url}/resendConfirmationEmail", command);

    public async Task<bool> ResetPassword(ResetPassword.Command command)
        => await _apiClient.Post<ResetPassword.Command, bool>($"{Url}/resetPassword", command);

    public async Task UpdateUserTheme(UpdateUserTheme.Command command)
        => await _apiClient.Put($"{Url}/{command.Id}/theme", command);

    public async Task<PagedList<UserDto>> GetAllUsers(GetAllUsers.Query query)
        => await _apiClient.Post<GetAllUsers.Query, PagedList<UserDto>>($"{Url}/all", query);

    public async Task<UserDto> GetUser(int id)
        => await _apiClient.Get<UserDto>($"{Url}/{id}");
}