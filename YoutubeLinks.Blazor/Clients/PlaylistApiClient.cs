using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Playlists.Commands;
using YoutubeLinks.Shared.Features.Playlists.Queries;
using YoutubeLinks.Shared.Features.Playlists.Responses;

namespace YoutubeLinks.Blazor.Clients
{
    public interface IPlaylistApiClient
    {
        Task<PagedList<PlaylistDto>> GetAllUserPlaylists(GetAllUserPlaylists.Query query);
        Task<PagedList<PlaylistDto>> GetAllPublicPlaylists(GetAllPublicPlaylists.Query query);
        Task<PlaylistDto> GetPlaylist(int id);
        Task CreatePlaylist(CreatePlaylist.Command command);
        Task UpdatePlaylist(UpdatePlaylist.Command command);
        Task DeletePlaylist(int id);
        Task<HttpResponseMessage> ExportPlaylistToJson(ExportPlaylistToJson.Command command);
    }

    public class PlaylistApiClient : IPlaylistApiClient
    {
        private readonly IApiClient _apiClient;
        private readonly string _url = "api/playlists";

        public PlaylistApiClient(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<PagedList<PlaylistDto>> GetAllUserPlaylists(GetAllUserPlaylists.Query query)
            => await _apiClient.Post<GetAllUserPlaylists.Query, PagedList<PlaylistDto>>($"{_url}/all", query);

        public async Task<PagedList<PlaylistDto>> GetAllPublicPlaylists(GetAllPublicPlaylists.Query query)
            => await _apiClient.Post<GetAllPublicPlaylists.Query, PagedList<PlaylistDto>>($"{_url}/allPublic", query);

        public async Task<PlaylistDto> GetPlaylist(int id)
            => await _apiClient.Get<PlaylistDto>($"{_url}/{id}");

        public async Task CreatePlaylist(CreatePlaylist.Command command)
            => await _apiClient.Post(_url, command);

        public async Task UpdatePlaylist(UpdatePlaylist.Command command)
            => await _apiClient.Put($"{_url}/{command.Id}", command);

        public async Task DeletePlaylist(int id)
            => await _apiClient.Delete($"{_url}/{id}");

        public async Task<HttpResponseMessage> ExportPlaylistToJson(ExportPlaylistToJson.Command command)
            => await _apiClient.PostReturnHttpResponseMessage($"{_url}/export", command);
    }
}
