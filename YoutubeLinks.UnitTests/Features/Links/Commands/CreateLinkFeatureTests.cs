using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Localization;
using NSubstitute;
using YoutubeLinks.Api;
using YoutubeLinks.Api.Abstractions;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Links.Commands;
using YoutubeLinks.Api.Services;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Commands;

namespace YoutubeLinks.UnitTests.Features.Links.Commands;

public class CreateLinkFeatureTests
{
    private readonly IAuthService _authService = Substitute.For<IAuthService>();
    private readonly IClock _clock = Substitute.For<IClock>();
    private readonly ILinkRepository _linkRepository = Substitute.For<ILinkRepository>();
    private readonly IStringLocalizer<ApiValidationMessage> _localizer = Substitute.For<IStringLocalizer<ApiValidationMessage>>();
    private readonly IPlaylistRepository _playlistRepository = Substitute.For<IPlaylistRepository>();
    private readonly IYoutubeService _youtubeService = Substitute.For<IYoutubeService>();

    [Fact]
    public async Task CreateLinkHandler_ThrowsValidationException_IfVideoIdIsNull()
    {
        var command = new CreateLink.Command
        {
            Url = string.Empty,
            PlaylistId = 1
        };

        var mediator = Substitute.For<IMediator>();

        mediator.Send(Arg.Any<CreateLink.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new CreateLinkFeature.Handler(_linkRepository, _playlistRepository, _authService,
                    _youtubeService, _clock, _localizer);
                return handler.Handle(callInfo.Arg<CreateLink.Command>(), CancellationToken.None);
            });

        var action = async () => await mediator.Send(command, CancellationToken.None);

        await Assert.ThrowsAsync<MyValidationException>(action);
        await _linkRepository.DidNotReceive().Create(Arg.Any<Link>());
    }

    [Fact]
    public async Task CreateLinkHandler_ThrowsNotFoundException_IfPlaylistIsNotFound()
    {
        var command = new CreateLink.Command
        {
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            PlaylistId = 1
        };

        var playlistRepository = Substitute.For<IPlaylistRepository>();
        var mediator = Substitute.For<IMediator>();

        playlistRepository.Get(Arg.Any<int>()).Returns(Task.FromResult<Playlist>(null));

        mediator.Send(Arg.Any<CreateLink.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new CreateLinkFeature.Handler(_linkRepository, playlistRepository, _authService,
                    _youtubeService, _clock, _localizer);
                return handler.Handle(callInfo.Arg<CreateLink.Command>(), CancellationToken.None);
            });

        var action = async () => await mediator.Send(command, CancellationToken.None);

        await Assert.ThrowsAsync<MyNotFoundException>(action);
        await _linkRepository.DidNotReceive().Create(Arg.Any<Link>());
    }

    [Fact]
    public async Task CreateLinkHandler_ThrowsForbiddenException_IfPlaylistIsNotOwnedByLoggedInUser()
    {
        var command = new CreateLink.Command
        {
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            PlaylistId = 1
        };

        var playlistRepository = Substitute.For<IPlaylistRepository>();
        var authService = Substitute.For<IAuthService>();
        var mediator = Substitute.For<IMediator>();

        playlistRepository.Get(Arg.Any<int>()).Returns(new Playlist());
        authService.IsLoggedInUser(Arg.Any<int>()).Returns(false);

        mediator.Send(Arg.Any<CreateLink.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new CreateLinkFeature.Handler(_linkRepository, playlistRepository, authService,
                    _youtubeService, _clock, _localizer);
                return handler.Handle(callInfo.Arg<CreateLink.Command>(), CancellationToken.None);
            });

        var action = async () => await mediator.Send(command, CancellationToken.None);

        await Assert.ThrowsAsync<MyForbiddenException>(action);
        await _linkRepository.DidNotReceive().Create(Arg.Any<Link>());
    }

    [Fact]
    public async Task CreateLinkHandler_ThrowsValidationException_IfLinkUrlIsNotUniqueInPlaylist()
    {
        var command = new CreateLink.Command
        {
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            PlaylistId = 1
        };

        var playlistRepository = Substitute.For<IPlaylistRepository>();
        var authService = Substitute.For<IAuthService>();
        var mediator = Substitute.For<IMediator>();

        playlistRepository.Get(Arg.Any<int>()).Returns(new Playlist());
        authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);
        playlistRepository.LinkUrlExists(Arg.Any<string>(), Arg.Any<int>()).Returns(true);

        mediator.Send(Arg.Any<CreateLink.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new CreateLinkFeature.Handler(_linkRepository, playlistRepository, authService,
                    _youtubeService, _clock, _localizer);
                return handler.Handle(callInfo.Arg<CreateLink.Command>(), CancellationToken.None);
            });

        var action = async () => await mediator.Send(command, CancellationToken.None);

        await Assert.ThrowsAsync<MyValidationException>(action);
        await _linkRepository.DidNotReceive().Create(Arg.Any<Link>());
    }

    [Fact]
    public async Task CreateLinkHandler_ReturnsLinkId_ForValidLink()
    {
        var command = new CreateLink.Command
        {
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            PlaylistId = 1
        };

        var playlistRepository = Substitute.For<IPlaylistRepository>();
        var authService = Substitute.For<IAuthService>();
        var linkRepository = Substitute.For<ILinkRepository>();
        var mediator = Substitute.For<IMediator>();

        playlistRepository.Get(Arg.Any<int>()).Returns(new Playlist());
        authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);
        playlistRepository.LinkUrlExists(Arg.Any<string>(), Arg.Any<int>()).Returns(false);
        linkRepository.Create(Arg.Any<Link>()).Returns(1);

        mediator.Send(Arg.Any<CreateLink.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new CreateLinkFeature.Handler(linkRepository, playlistRepository, authService,
                    _youtubeService, _clock, _localizer);
                return handler.Handle(callInfo.Arg<CreateLink.Command>(), CancellationToken.None);
            });

        var result = await mediator.Send(command, CancellationToken.None);

        result.Should().Be(1);
        await linkRepository.Received().Create(Arg.Any<Link>());
    }
}