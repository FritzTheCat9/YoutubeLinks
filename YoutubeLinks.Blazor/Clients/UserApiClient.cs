using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Users.Commands;
using YoutubeLinks.Shared.Features.Users.Queries;
using YoutubeLinks.Shared.Features.Users.Responses;

namespace YoutubeLinks.Blazor.Clients
{
    public interface IUserApiClient
    {
        Task<bool> ConfirmEmail(ConfirmEmail.Command command);
        Task ForgotPassword(ForgotPassword.Command command);
        Task<JwtDto> Login(Login.Command command);
        Task<JwtDto> RefreshToken(RefreshToken.Command command);
        Task Register(Register.Command command);
        Task ResendConfirmationEmail(ResendConfirmationEmail.Command command);
        Task UpdateUserTheme(UpdateUserTheme.Command command);
        Task<PagedList<UserDto>> GetAllUsers(GetAllUsers.Query query);
        Task<UserDto> GetUser(int id);
    }

    public class UserApiClient : IUserApiClient
    {
        private readonly IApiClient _apiClient;
        private readonly string _url = "api/users";

        public UserApiClient(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<bool> ConfirmEmail(ConfirmEmail.Command command)
            => await _apiClient.Post<ConfirmEmail.Command, bool>($"{_url}/confirmEmail", command);

        public async Task ForgotPassword(ForgotPassword.Command command)
            => await _apiClient.Post($"{_url}/forgotPassword", command);

        public async Task<JwtDto> Login(Login.Command command)
            => await _apiClient.Post<Login.Command, JwtDto>($"{_url}/login", command);

        public async Task<JwtDto> RefreshToken(RefreshToken.Command command)
            => await _apiClient.Post<RefreshToken.Command, JwtDto>($"{_url}/refresh-token", command);

        public async Task Register(Register.Command command)
            => await _apiClient.Post($"{_url}/register", command);

        public async Task ResendConfirmationEmail(ResendConfirmationEmail.Command command)
            => await _apiClient.Post($"{_url}/resendConfirmationEmail", command);

        public async Task UpdateUserTheme(UpdateUserTheme.Command command)
            => await _apiClient.Put($"{_url}/{command.Id}/theme", command);

        public async Task<PagedList<UserDto>> GetAllUsers(GetAllUsers.Query query)
            => await _apiClient.Post<GetAllUsers.Query, PagedList<UserDto>>($"{_url}/all", query);

        public async Task<UserDto> GetUser(int id)
            => await _apiClient.Get<UserDto>($"{_url}/{id}");
    }
}
