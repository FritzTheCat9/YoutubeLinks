﻿using YoutubeLinks.Shared.Abstractions;
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
    private const string Url = "api/links";

    public async Task<PagedList<LinkDto>> GetAllPaginatedLinks(GetAllPaginatedLinks.Query query)
    {
        return await apiClient.Post<GetAllPaginatedLinks.Query, PagedList<LinkDto>>($"{Url}/allPaginated", query);
    }

    public async Task<IEnumerable<GetAllLinks.LinkInfoDto>> GetAllLinks(GetAllLinks.Query query)
    {
        return await apiClient.Post<GetAllLinks.Query, IEnumerable<GetAllLinks.LinkInfoDto>>($"{Url}/all", query);
    }

    public async Task<LinkDto> GetLink(int id)
    {
        return await apiClient.Get<LinkDto>($"{Url}/{id}");
    }

    public async Task<int> CreateLink(CreateLink.Command command)
    {
        return await apiClient.Post<CreateLink.Command, int>(Url, command);
    }

    public async Task UpdateLink(UpdateLink.Command command)
    {
        await apiClient.Put($"{Url}/{command.Id}", command);
    }

    public async Task SetLinkDownloadedFlag(SetLinkDownloadedFlag.Command command)
    {
        await apiClient.Put($"{Url}/{command.Id}/downloaded", command);
    }

    public async Task DeleteLink(int id)
    {
        await apiClient.Delete($"{Url}/{id}");
    }

    public async Task<HttpResponseMessage> DownloadLink(DownloadLink.Command command)
    {
        return await apiClient.PostReturnHttpResponseMessage($"{Url}/download", command);
    }

    public async Task<HttpResponseMessage> DownloadSingleLink(DownloadSingleLink.Command command)
    {
        return await apiClient.PostReturnHttpResponseMessage($"{Url}/downloadSingle", command);
    }
}