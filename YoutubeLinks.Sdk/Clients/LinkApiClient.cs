using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Shared.Features.Links.Queries;
using YoutubeLinks.Shared.Features.Links.Responses;

namespace YoutubeLinks.Sdk.Clients;

public interface ILinkApiClient
{
    Task<PagedList<LinkDto>> GetAllPaginatedLinks(GetAllPaginatedLinks.Query query);
    Task<IEnumerable<GetAllLinks.LinkInfoDto>> GetAllLinks(GetAllLinks.Query query);
    Task<LinkDto> GetLink(int id);
    Task<int> CreateLink(CreateLink.Command command);
    Task UpdateLink(UpdateLink.Command command);
    Task SetLinkDownloadedFlag(SetLinkDownloadedFlag.Command command);
    Task DeleteLink(int id);
    Task<HttpResponseMessage> DownloadLink(DownloadLink.Command command);
    Task<HttpResponseMessage> DownloadSingleLink(DownloadSingleLink.Command command);
}

public class LinkApiClient(IApiClient apiClient) : ILinkApiClient
{
    private const string _url = "api/links";

    public async Task<PagedList<LinkDto>> GetAllPaginatedLinks(GetAllPaginatedLinks.Query query)
    {
        return await apiClient.Post<GetAllPaginatedLinks.Query, PagedList<LinkDto>>($"{_url}/allPaginated", query);
    }

    public async Task<IEnumerable<GetAllLinks.LinkInfoDto>> GetAllLinks(GetAllLinks.Query query)
    {
        return await apiClient.Post<GetAllLinks.Query, IEnumerable<GetAllLinks.LinkInfoDto>>($"{_url}/all", query);
    }

    public async Task<LinkDto> GetLink(int id)
    {
        return await apiClient.Get<LinkDto>($"{_url}/{id}");
    }

    public async Task<int> CreateLink(CreateLink.Command command)
    {
        return await apiClient.Post<CreateLink.Command, int>(_url, command);
    }

    public async Task UpdateLink(UpdateLink.Command command)
    {
        await apiClient.Put($"{_url}/{command.Id}", command);
    }

    public async Task SetLinkDownloadedFlag(SetLinkDownloadedFlag.Command command)
    {
        await apiClient.Put($"{_url}/{command.Id}/downloaded", command);
    }

    public async Task DeleteLink(int id)
    {
        await apiClient.Delete($"{_url}/{id}");
    }

    public async Task<HttpResponseMessage> DownloadLink(DownloadLink.Command command)
    {
        return await apiClient.PostReturnHttpResponseMessage($"{_url}/download", command);
    }

    public async Task<HttpResponseMessage> DownloadSingleLink(DownloadSingleLink.Command command)
    {
        return await apiClient.PostReturnHttpResponseMessage($"{_url}/downloadSingle", command);
    }
}