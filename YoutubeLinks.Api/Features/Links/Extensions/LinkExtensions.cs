using System.Linq.Expressions;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Features.Links.Commands;
using YoutubeLinks.Api.Features.Links.Queries;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Links.Queries;
using YoutubeLinks.Shared.Features.Links.Responses;
using static YoutubeLinks.Shared.Features.Links.Queries.GetAllLinks;

namespace YoutubeLinks.Api.Features.Links.Extensions
{
    public static class LinkExtensions
    {
        public static IEndpointRouteBuilder AddLinksEndpoints(this IEndpointRouteBuilder app)
        {
            CreateLinkFeature.Endpoint(app);
            DeleteLinkFeature.Endpoint(app);
            DownloadLinkFeature.Endpoint(app);
            DownloadSingleLinkFeature.Endpoint(app);
            SetLinkDownloadedFlagFeature.Endpoint(app);
            UpdateLinkFeature.Endpoint(app);
            GetAllLinksFeature.Endpoint(app);
            GetAllPaginatedLinksFeature.Endpoint(app);
            GetLinkFeature.Endpoint(app);

            return app;
        }

        public static LinkDto ToDto(this Link link)
        {
            return new()
            {
                Id = link.Id,
                Created = link.Created,
                Modified = link.Modified,
                Url = link.Url,
                VideoId = link.VideoId,
                Title = link.Title,
                Downloaded = link.Downloaded,
                PlaylistId = link.PlaylistId,
            };
        }

        public static LinkInfoDto ToLinkInfoDto(this Link link)
        {
            return new()
            {
                Id = link.Id,
                Url = link.Url,
                VideoId = link.VideoId,
                Title = link.Title,
            };
        }

        /* GetAllPaginatedLinks.Query */

        public static IQueryable<Link> FilterLinks(
            this IQueryable<Link> links,
            GetAllPaginatedLinks.Query query)
        {
            var searchTerm = query.SearchTerm?.ToLower()?.Trim();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                links = links.Where(x =>
                    x.Title.ToLower().Contains(searchTerm));

            return links;
        }

        public static IQueryable<Link> SortLinks(
            this IQueryable<Link> links,
            GetAllPaginatedLinks.Query query)
        {
            return query.SortOrder switch
            {
                SortOrder.Ascending => links.OrderBy(GetLinkSortProperty(query)),
                SortOrder.Descending => links.OrderByDescending(GetLinkSortProperty(query)),
                SortOrder.None => links.OrderBy(x => x.Title),
                _ => links.OrderBy(x => x.Title),
            };
        }

        private static Expression<Func<Link, object>> GetLinkSortProperty(GetAllPaginatedLinks.Query query)
        {
            return query.SortColumn.ToLowerInvariant() switch
            {
                "title" => link => link.Title,
                "modified" => link => link.Modified,
                _ => link => link.Title,
            };
        }

        /* GetAllLinks.Query */

        public static IQueryable<Link> FilterDownloaded(
            this IQueryable<Link> links,
            Query query)
        {
            links = links.Where(x => x.Downloaded == query.Downloaded);
            return links;
        }

        public static IQueryable<Link> SortLinks(
            this IQueryable<Link> links)
        {
            links = links.OrderBy(link => link.Title);
            return links;
        }

        public static List<LinkInfoDto> ToLinkInfoDtos(
            this IQueryable<Link> links)
        {
            var dtos = links.Select(x => x.ToLinkInfoDto())
                            .ToList();
            return dtos;
        }
    }
}
