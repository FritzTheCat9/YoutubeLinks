using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Shared.Features.Links.Queries;
using YoutubeLinks.Shared.Features.Links.Responses;
using static YoutubeLinks.Shared.Features.Links.Queries.GetAllLinks;

namespace YoutubeLinks.Blazor.Clients
{
    public interface ILinkApiClient
    {
        Task<PagedList<LinkDto>> GetAllPaginatedLinks(GetAllPaginatedLinks.Query query);
        Task<IEnumerable<LinkInfoDto>> GetAllLinks(Query query);
        Task<LinkDto> GetLink(int id);
        Task CreateLink(CreateLink.Command command);
        Task UpdateLink(UpdateLink.Command command);
        Task DeleteLink(int id);
        Task<HttpResponseMessage> DownloadLink(DownloadLink.DownloadLinkCommand command);
    }

    public class LinkApiClient : ILinkApiClient
    {
        private readonly IApiClient _apiClient;
        private readonly string _url = "api/links";

        public LinkApiClient(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<PagedList<LinkDto>> GetAllPaginatedLinks(GetAllPaginatedLinks.Query query)
            => await _apiClient.Post<GetAllPaginatedLinks.Query, PagedList<LinkDto>>($"{_url}/allPaginated", query);

        public async Task<IEnumerable<LinkInfoDto>> GetAllLinks(Query query)
            => await _apiClient.Post<Query, IEnumerable<LinkInfoDto>>($"{_url}/all", query);

        public async Task<LinkDto> GetLink(int id)
            => await _apiClient.Get<LinkDto>($"{_url}/{id}");

        public async Task CreateLink(CreateLink.Command command)
            => await _apiClient.Post(_url, command);

        public async Task UpdateLink(UpdateLink.Command command)
            => await _apiClient.Put($"{_url}/{command.Id}", command);

        public async Task DeleteLink(int id)
            => await _apiClient.Delete($"{_url}/{id}");

        public async Task<HttpResponseMessage> DownloadLink(DownloadLink.DownloadLinkCommand command)
            => await _apiClient.PostReturnHttpResponseMessage($"{_url}/download", command);
    }
}
