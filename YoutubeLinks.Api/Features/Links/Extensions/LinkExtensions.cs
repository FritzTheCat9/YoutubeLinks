using System.Linq.Expressions;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Features.Links.Commands;
using YoutubeLinks.Api.Features.Links.Queries;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Links.Queries;
using YoutubeLinks.Shared.Features.Links.Responses;
namespace YoutubeLinks.Api.Features.Links.Extensions
{
    public static class LinkExtensions
    {
        public static IEndpointRouteBuilder AddLinksEndpoints(this IEndpointRouteBuilder app)
        {
            CreateLinkFeature.Endpoint(app);
            DeleteLinkFeature.Endpoint(app);
            UpdateLinkFeature.Endpoint(app);
            GetAllLinksFeature.Endpoint(app);
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
                Title = link.Title,
                Downloaded = link.Downloaded,
                PlaylistId = link.PlaylistId,
            };
        }

        public static IQueryable<Link> FilterLinks(
            this IQueryable<Link> links,
            GetAllLinks.Query query)
        {
            var searchTerm = query.SearchTerm.ToLower().Trim();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                links = links.Where(x =>
                    x.Title.ToLower().Contains(searchTerm));

            return links;
        }

        public static IQueryable<Link> SortLinks(
            this IQueryable<Link> links,
            GetAllLinks.Query query)
        {
            return query.SortOrder switch
            {
                SortOrder.Ascending => links.OrderBy(GetLinkSortProperty(query)),
                SortOrder.Descending => links.OrderByDescending(GetLinkSortProperty(query)),
                SortOrder.None => links.OrderBy(x => x.Title),
                _ => links.OrderBy(x => x.Title),
            };
        }

        private static Expression<Func<Link, object>> GetLinkSortProperty(GetAllLinks.Query query)
        {
            return query.SortColumn.ToLowerInvariant() switch
            {
                "title" => link => link.Title,
                _ => link => link.Title,
            };
        }
    }
}
