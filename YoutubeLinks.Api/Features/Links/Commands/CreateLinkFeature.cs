using MediatR;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Api.Abstractions;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Api.Localization;
using YoutubeLinks.Api.Services;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Shared.Features.Links.Helpers;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.Api.Features.Links.Commands
{
    public static class CreateLinkFeature
    {
        public static void Endpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/links", async (
                CreateLink.Command command,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var playlistId = await mediator.Send(command, cancellationToken);
                return Results.CreatedAtRoute("GetLink", new { id = playlistId });
            })
                .WithTags(Tags.Links)
                .RequireAuthorization(Policy.User);
        }

        public class Handler : IRequestHandler<CreateLink.Command, int>
        {
            private readonly ILinkRepository _linkRepository;
            private readonly IPlaylistRepository _playlistRepository;
            private readonly IAuthService _authService;
            private readonly IYoutubeService _youtubeService;
            private readonly IClock _clock;
            private readonly IStringLocalizer<ApiValidationMessage> _localizer;

            public Handler(
                ILinkRepository linkRepository,
                IPlaylistRepository playlistRepository,
                IAuthService authService,
                IYoutubeService youtubeService,
                IClock clock,
                IStringLocalizer<ApiValidationMessage> localizer)
            {
                _linkRepository = linkRepository;
                _playlistRepository = playlistRepository;
                _authService = authService;
                _youtubeService = youtubeService;
                _clock = clock;
                _localizer = localizer;
            }

            public async Task<int> Handle(
                CreateLink.Command command,
                CancellationToken cancellationToken)
            {
                var videoId = YoutubeHelpers.GetVideoId(command.Url);
                if (string.IsNullOrWhiteSpace(videoId))
                    throw new MyValidationException(nameof(CreateLink.Command.Url),
                                                    _localizer[nameof(ApiValidationMessageString.UrlIdNotValid)]);

                command.Url = $"{YoutubeHelpers.VideoPathBase}{videoId}";
                var videoTitle = await _youtubeService.GetVideoTitle(videoId);

                await ValidateCommand(command, _localizer);

                var link = new Link
                {
                    Id = 0,
                    Created = _clock.Current(),
                    Modified = _clock.Current(),
                    Url = command.Url,
                    VideoId = videoId,
                    Title = videoTitle,
                    Downloaded = false,
                    PlaylistId = command.PlaylistId,
                };

                return await _linkRepository.Create(link);
            }

            private async Task ValidateCommand(CreateLink.Command command, IStringLocalizer<ApiValidationMessage> localizer)
            {
                var playlist = await _playlistRepository.Get(command.PlaylistId) ?? throw new MyNotFoundException();

                var isUserPlaylist = _authService.IsLoggedInUser(playlist.UserId);
                if (!isUserPlaylist)
                    throw new MyForbiddenException();

                var urlExists = await _playlistRepository.LinkUrlExists(command.Url, playlist.Id);
                if (urlExists)
                    throw new MyValidationException(nameof(CreateLink.Command.Url),
                                                    localizer[nameof(ApiValidationMessageString.UrlMustBeUnique)]);
            }
        }
    }
}
