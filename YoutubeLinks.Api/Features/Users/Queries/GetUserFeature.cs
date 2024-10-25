using MediatR;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Users.Extensions;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Users.Queries;
using YoutubeLinks.Shared.Features.Users.Responses;

namespace YoutubeLinks.Api.Features.Users.Queries;

public static class GetUserFeature
{
    public static void Endpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users/{id:int}", async (
                int id,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetUser.Query { Id = id };
                return Results.Ok(await mediator.Send(query, cancellationToken));
            })
            .WithName("GetUser")
            .WithTags(Tags.Users)
            .AllowAnonymous();
    }

    public class Handler(IUserRepository userRepository) : IRequestHandler<GetUser.Query, UserDto>
    {
        public async Task<UserDto> Handle(
            GetUser.Query query,
            CancellationToken cancellationToken)
        {
            var user = await userRepository.Get(query.Id) ?? throw new MyNotFoundException();
            return user.ToDto();
        }
    }
}