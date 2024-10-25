using FluentValidation;
using MediatR;
using YoutubeLinks.Shared.Features.Playlists.Responses;

namespace YoutubeLinks.Shared.Features.Playlists.Queries;

public static class GetPlaylist
{
    public class Query : IRequest<PlaylistDto>
    {
        public int Id { get; set; }
    }

    public class Validator : AbstractValidator<Query> { }
}