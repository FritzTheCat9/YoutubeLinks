using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Playlists.Commands;
using YoutubeLinks.Shared.Features.Playlists.Queries;
using YoutubeLinks.Shared.Features.Playlists.Responses;

namespace YoutubeLinks.Shared.Clients;

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

public class PlaylistApiClient(IApiClient apiClient) : IPlaylistApiClient
{
    private const string Url = "api/playlists";

    public async Task<PagedList<PlaylistDto>> GetAllUserPlaylists(GetAllUserPlaylists.Query query)
    {
        return await apiClient.Post<GetAllUserPlaylists.Query, PagedList<PlaylistDto>>($"{Url}/all", query);
    }

    public async Task<PagedList<PlaylistDto>> GetAllPublicPlaylists(GetAllPublicPlaylists.Query query)
    {
        return await apiClient.Post<GetAllPublicPlaylists.Query, PagedList<PlaylistDto>>($"{Url}/allPublic", query);
    }

    public async Task<PlaylistDto> GetPlaylist(int id)
    {
        return await apiClient.Get<PlaylistDto>($"{Url}/{id}");
    }

    public async Task CreatePlaylist(CreatePlaylist.Command command)
    {
        await apiClient.Post(Url, command);
    }

    public async Task UpdatePlaylist(UpdatePlaylist.Command command)
    {
        await apiClient.Put($"{Url}/{command.Id}", command);
    }

    public async Task DeletePlaylist(int id)
    {
        await apiClient.Delete($"{Url}/{id}");
    }

    public async Task<HttpResponseMessage> ExportPlaylist(ExportPlaylist.Command command)
    {
        return await apiClient.PostReturnHttpResponseMessage($"{Url}/export", command);
    }

    public async Task ImportPlaylistFromJson(ImportPlaylist.Command command)
    {
        await apiClient.Post($"{Url}/import", command);
    }

    public async Task ResetLinksDownloadedFlag(ResetLinksDownloadedFlag.Command command)
    {
        await apiClient.Post($"{Url}/resetDownloadedFlag", command);
    }
}