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
        Task<HttpResponseMessage> ExportPlaylist(ExportPlaylist.Command command);
        Task ImportPlaylistFromJson(ImportPlaylist.Command command);
        Task ResetLinksDownloadedFlag(ResetLinksDownloadedFlag.Command command);
    }

    public class PlaylistApiClient : IPlaylistApiClient
    {
        private readonly IApiClient _apiClient;
        private const string Url = "api/playlists";

        public PlaylistApiClient(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<PagedList<PlaylistDto>> GetAllUserPlaylists(GetAllUserPlaylists.Query query)
            => await _apiClient.Post<GetAllUserPlaylists.Query, PagedList<PlaylistDto>>($"{Url}/all", query);

        public async Task<PagedList<PlaylistDto>> GetAllPublicPlaylists(GetAllPublicPlaylists.Query query)
            => await _apiClient.Post<GetAllPublicPlaylists.Query, PagedList<PlaylistDto>>($"{Url}/allPublic", query);

        public async Task<PlaylistDto> GetPlaylist(int id)
            => await _apiClient.Get<PlaylistDto>($"{Url}/{id}");

        public async Task CreatePlaylist(CreatePlaylist.Command command)
            => await _apiClient.Post(Url, command);

        public async Task UpdatePlaylist(UpdatePlaylist.Command command)
            => await _apiClient.Put($"{Url}/{command.Id}", command);

        public async Task DeletePlaylist(int id)
            => await _apiClient.Delete($"{Url}/{id}");

        public async Task<HttpResponseMessage> ExportPlaylist(ExportPlaylist.Command command)
            => await _apiClient.PostReturnHttpResponseMessage($"{Url}/export", command);

        public async Task ImportPlaylistFromJson(ImportPlaylist.Command command)
            => await _apiClient.Post($"{Url}/import", command);

        public async Task ResetLinksDownloadedFlag(ResetLinksDownloadedFlag.Command command)
            => await _apiClient.Post($"{Url}/resetDownloadedFlag", command);
    }
}
