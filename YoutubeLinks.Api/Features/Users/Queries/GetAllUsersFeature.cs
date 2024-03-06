using MediatR;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Extensions;
using YoutubeLinks.Api.Features.Users.Extensions;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Users.Queries;
using YoutubeLinks.Shared.Features.Users.Responses;

namespace YoutubeLinks.Api.Features.Users.Queries
{
    public static class GetAllUsersFeature
    {
        public static IEndpointRouteBuilder Endpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/users/all", async (
                GetAllUsers.Query query,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                return Results.Ok(await mediator.Send(query, cancellationToken));
            })
                .WithTags("Users")
                .AllowAnonymous();

            return app;
        }

        public class Handler : IRequestHandler<GetAllUsers.Query, PagedList<UserDto>>
        {
            private readonly IUserRepository _userRepository;

            public Handler(IUserRepository userRepository)
            {
                _userRepository = userRepository;
            }

            public async Task<PagedList<UserDto>> Handle(
                GetAllUsers.Query query,
                CancellationToken cancellationToken)
            {
                var usersQuery = _userRepository.AsQueryable();

                usersQuery = usersQuery.FilterMyUsers(query);
                usersQuery = usersQuery.SortMyUsers(query);

                var usersPagedList = PageListExtensions<UserDto>.Create(usersQuery.Select(x => x.ToDto()),
                                                                        query.Page,
                                                                        query.PageSize);

                return await Task.FromResult(usersPagedList);
            }
        }
    }
}
