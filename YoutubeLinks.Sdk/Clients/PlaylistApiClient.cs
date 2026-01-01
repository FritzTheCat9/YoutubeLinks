using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Playlists.Commands;
using YoutubeLinks.Shared.Features.Playlists.Queries;
using YoutubeLinks.Shared.Features.Playlists.Responses;

namespace YoutubeLinks.Sdk.Clients;

public interface IPlaylistApiClient
{
    Task<PagedList<PlaylistDto>> GetAllUserPlaylists(GetAllUserPlaylists.Query query);
    Task<PagedList<PlaylistDto>> GetAllPublicPlaylists(GetAllPublicPlaylists.Query query);
    Task<PlaylistDto> GetPlaylist(int id);
    Task<int> CreatePlaylist(CreatePlaylist.Command command);
    Task UpdatePlaylist(UpdatePlaylist.Command command);
    Task DeletePlaylist(int id);
    Task<HttpResponseMessage> ExportPlaylist(ExportPlaylist.Command command);
    Task ImportPlaylist(ImportPlaylist.Command command);
    Task ResetLinksDownloadedFlag(ResetLinksDownloadedFlag.Command command);
}

public class PlaylistApiClient(IApiClient apiClient) : IPlaylistApiClient
{
    private const string _url = "api/playlists";

    public async Task<PagedList<PlaylistDto>> GetAllUserPlaylists(GetAllUserPlaylists.Query query)
    {
        return await apiClient.Post<GetAllUserPlaylists.Query, PagedList<PlaylistDto>>($"{_url}/all", query);
    }

    public async Task<PagedList<PlaylistDto>> GetAllPublicPlaylists(GetAllPublicPlaylists.Query query)
    {
        return await apiClient.Post<GetAllPublicPlaylists.Query, PagedList<PlaylistDto>>($"{_url}/allPublic", query);
    }

    public async Task<PlaylistDto> GetPlaylist(int id)
    {
        return await apiClient.Get<PlaylistDto>($"{_url}/{id}");
    }

    public async Task<int> CreatePlaylist(CreatePlaylist.Command command)
    {
        return await apiClient.Post<CreatePlaylist.Command, int>(_url, command);
    }

    public async Task UpdatePlaylist(UpdatePlaylist.Command command)
    {
        await apiClient.Put($"{_url}/{command.Id}", command);
    }

    public async Task DeletePlaylist(int id)
    {
        await apiClient.Delete($"{_url}/{id}");
    }

    public async Task<HttpResponseMessage> ExportPlaylist(ExportPlaylist.Command command)
    {
        return await apiClient.PostReturnHttpResponseMessage($"{_url}/export", command);
    }

    public async Task ImportPlaylist(ImportPlaylist.Command command)
    {
        await apiClient.Post($"{_url}/import", command);
    }

    public async Task ResetLinksDownloadedFlag(ResetLinksDownloadedFlag.Command command)
    {
        await apiClient.Post($"{_url}/resetDownloadedFlag", command);
    }
}