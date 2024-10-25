using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Shared.Features.Links.Queries;
using YoutubeLinks.Shared.Features.Links.Responses;
using static YoutubeLinks.Shared.Features.Links.Queries.GetAllLinks;

namespace YoutubeLinks.Blazor.Clients;

public interface ILinkApiClient
{
    Task<PagedList<LinkDto>> GetAllPaginatedLinks(GetAllPaginatedLinks.Query query);
    Task<IEnumerable<LinkInfoDto>> GetAllLinks(Query query);
    Task<LinkDto> GetLink(int id);
    Task CreateLink(CreateLink.Command command);
    Task UpdateLink(UpdateLink.Command command);
    Task SetLinkDownloadedFlag(SetLinkDownloadedFlag.Command command);
    Task DeleteLink(int id);
    Task<HttpResponseMessage> DownloadLink(DownloadLink.Command command);
    Task<HttpResponseMessage> DownloadSingleLink(DownloadSingleLink.Command command);
}

public class LinkApiClient : ILinkApiClient
{
    private const string Url = "api/links";
    private readonly IApiClient _apiClient;

    public LinkApiClient(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<PagedList<LinkDto>> GetAllPaginatedLinks(GetAllPaginatedLinks.Query query)
        => await _apiClient.Post<GetAllPaginatedLinks.Query, PagedList<LinkDto>>($"{Url}/allPaginated", query);

    public async Task<IEnumerable<LinkInfoDto>> GetAllLinks(Query query)
        => await _apiClient.Post<Query, IEnumerable<LinkInfoDto>>($"{Url}/all", query);

    public async Task<LinkDto> GetLink(int id)
        => await _apiClient.Get<LinkDto>($"{Url}/{id}");

    public async Task CreateLink(CreateLink.Command command)
        => await _apiClient.Post(Url, command);

    public async Task UpdateLink(UpdateLink.Command command)
        => await _apiClient.Put($"{Url}/{command.Id}", command);

    public async Task SetLinkDownloadedFlag(SetLinkDownloadedFlag.Command command)
        => await _apiClient.Put($"{Url}/{command.Id}/downloaded", command);

    public async Task DeleteLink(int id)
        => await _apiClient.Delete($"{Url}/{id}");

    public async Task<HttpResponseMessage> DownloadLink(DownloadLink.Command command)
        => await _apiClient.PostReturnHttpResponseMessage($"{Url}/download", command);

    public async Task<HttpResponseMessage> DownloadSingleLink(DownloadSingleLink.Command command)
        => await _apiClient.PostReturnHttpResponseMessage($"{Url}/downloadSingle", command);
}