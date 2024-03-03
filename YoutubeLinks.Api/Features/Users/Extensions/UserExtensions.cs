using System.Linq.Expressions;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Features.Users.Commands;
using YoutubeLinks.Api.Features.Users.Queries;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Users.Queries;
using YoutubeLinks.Shared.Features.Users.Responses;

namespace YoutubeLinks.Api.Features.Users.Extensions
{
    public static class UserExtensions
    {
        public static IEndpointRouteBuilder AddUserEndpoints(this IEndpointRouteBuilder app)
        {
            ConfirmEmailFeature.Endpoint(app);
            LoginFeature.Endpoint(app);
            RegisterFeature.Endpoint(app);
            GetAllUsersFeature.Endpoint(app);
            GetUserFeature.Endpoint(app);

            return app;
        }

        public static UserDto ToDto(this User user)
        {
            return new()
            {
                Id = user.Id,
                Created = user.Created,
                Modified = user.Modified,
                UserName = user.UserName,
                Email = user.Email,
                IsAdmin = user.IsAdmin,
            };
        }

        public static IQueryable<User> FilterMyUsers(
            this IQueryable<User> users,
            GetAllUsers.Query query)
        {
            var searchTerm = query.SearchTerm.ToLower().Trim();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                users = users.Where(x =>
                    x.UserName.ToLower().Contains(searchTerm)
                    || x.Email.ToLower().Contains(searchTerm));
            }

            return users;
        }

        public static IQueryable<User> SortMyUsers(
            this IQueryable<User> users,
            GetAllUsers.Query query)
        {
            return query.SortOrder switch
            {
                SortOrder.Ascending => users.OrderBy(GetUsersSortProperty(query)),
                SortOrder.Descending => users.OrderByDescending(GetUsersSortProperty(query)),
                SortOrder.None => users.OrderBy(x => x.UserName),
                _ => users.OrderBy(x => x.UserName),
            };
        }

        private static Expression<Func<User, object>> GetUsersSortProperty(GetAllUsers.Query query)
        {
            return query.SortColumn.ToLowerInvariant() switch
            {
                "username" => user => user.UserName,
                "email" => user => user.Email,
                _ => user => user.UserName,
            };
        }
    }
}
