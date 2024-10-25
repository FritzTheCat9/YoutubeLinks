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

public class UpdateLinkFeatureTests
{
    private readonly IAuthService _authService = Substitute.For<IAuthService>();
    private readonly IClock _clock = Substitute.For<IClock>();
    private readonly IStringLocalizer<ApiValidationMessage> _localizer = Substitute.For<IStringLocalizer<ApiValidationMessage>>();
    private readonly IPlaylistRepository _playlistRepository = Substitute.For<IPlaylistRepository>();
    private readonly IYoutubeService _youtubeService = Substitute.For<IYoutubeService>();

    [Fact]
    public async Task UpdateLinkHandler_ThrowsNotFoundException_IfLinkIsNotFound()
    {
        var command = new UpdateLink.Command
        {
            Id = 1,
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            Downloaded = false
        };

        var linkRepository = Substitute.For<ILinkRepository>();
        var mediator = Substitute.For<IMediator>();

        linkRepository.Get(Arg.Any<int>()).Returns(Task.FromResult<Link>(null));

        mediator.Send(Arg.Any<UpdateLink.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new UpdateLinkFeature.Handler(_playlistRepository, linkRepository, _authService,
                    _youtubeService, _clock, _localizer);
                return handler.Handle(callInfo.Arg<UpdateLink.Command>(), CancellationToken.None);
            });

        var action = async () => await mediator.Send(command, CancellationToken.None);

        await Assert.ThrowsAsync<MyNotFoundException>(action);
        await linkRepository.DidNotReceive().Update(Arg.Any<Link>());
    }

    [Fact]
    public async Task UpdateLinkHandler_ThrowsForbiddenException_IfUserIsNotLoggedIn()
    {
        var command = new UpdateLink.Command
        {
            Id = 1,
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            Downloaded = false
        };

        var linkRepository = Substitute.For<ILinkRepository>();
        var authService = Substitute.For<IAuthService>();
        var mediator = Substitute.For<IMediator>();

        linkRepository.Get(Arg.Any<int>()).Returns(new Link
        {
            Playlist = new Playlist
            {
                UserId = 1
            }
        });
        authService.IsLoggedInUser(Arg.Any<int>()).Returns(false);

        mediator.Send(Arg.Any<UpdateLink.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new UpdateLinkFeature.Handler(_playlistRepository, linkRepository, authService,
                    _youtubeService, _clock, _localizer);
                return handler.Handle(callInfo.Arg<UpdateLink.Command>(), CancellationToken.None);
            });

        var action = async () => await mediator.Send(command, CancellationToken.None);

        await Assert.ThrowsAsync<MyForbiddenException>(action);
        await linkRepository.DidNotReceive().Update(Arg.Any<Link>());
    }

    [Fact]
    public async Task UpdateLinkHandler_ThrowsValidationException_IfVideoIdIsNull()
    {
        var command = new UpdateLink.Command
        {
            Id = 1,
            Url = string.Empty,
            Downloaded = false
        };

        var linkRepository = Substitute.For<ILinkRepository>();
        var authService = Substitute.For<IAuthService>();
        var mediator = Substitute.For<IMediator>();

        linkRepository.Get(Arg.Any<int>()).Returns(new Link
        {
            Playlist = new Playlist
            {
                UserId = 1
            }
        });
        authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);

        mediator.Send(Arg.Any<UpdateLink.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new UpdateLinkFeature.Handler(_playlistRepository, linkRepository, authService,
                    _youtubeService, _clock, _localizer);
                return handler.Handle(callInfo.Arg<UpdateLink.Command>(), CancellationToken.None);
            });

        var action = async () => await mediator.Send(command, CancellationToken.None);

        await Assert.ThrowsAsync<MyValidationException>(action);
        await linkRepository.DidNotReceive().Update(Arg.Any<Link>());
    }


    [Fact]
    public async Task UpdateLinkHandler_ThrowsValidationException_IfLinkUrlExistsInOtherLinks()
    {
        var command = new UpdateLink.Command
        {
            Id = 1,
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            Downloaded = false
        };

        var linkRepository = Substitute.For<ILinkRepository>();
        var authService = Substitute.For<IAuthService>();
        var playlistRepository = Substitute.For<IPlaylistRepository>();
        var mediator = Substitute.For<IMediator>();

        linkRepository.Get(Arg.Any<int>()).Returns(new Link
        {
            Playlist = new Playlist
            {
                UserId = 1
            }
        });
        authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);
        playlistRepository.LinkUrlExistsInOtherLinksThan(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(true);

        mediator.Send(Arg.Any<UpdateLink.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new UpdateLinkFeature.Handler(playlistRepository, linkRepository, authService,
                    _youtubeService, _clock, _localizer);
                return handler.Handle(callInfo.Arg<UpdateLink.Command>(), CancellationToken.None);
            });

        var action = async () => await mediator.Send(command, CancellationToken.None);

        await Assert.ThrowsAsync<MyValidationException>(action);
        await linkRepository.DidNotReceive().Update(Arg.Any<Link>());
    }

    [Fact]
    public async Task UpdateLinkHandler_GetVideoTitle_IfTitleIsEmpty()
    {
        var command = new UpdateLink.Command
        {
            Id = 1,
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            Downloaded = false
        };
        var link = new Link
        {
            Playlist = new Playlist
            {
                UserId = 1
            },
            Title = string.Empty,
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ"
        };
        const string newTitle = "Rick Astley - Never Gonna Give You Up";

        var linkRepository = Substitute.For<ILinkRepository>();
        var authService = Substitute.For<IAuthService>();
        var playlistRepository = Substitute.For<IPlaylistRepository>();
        var youtubeService = Substitute.For<IYoutubeService>();
        var mediator = Substitute.For<IMediator>();

        linkRepository.Get(Arg.Any<int>()).Returns(link);
        authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);
        playlistRepository.LinkUrlExistsInOtherLinksThan(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(false);
        youtubeService.GetVideoTitle(Arg.Any<string>()).Returns(newTitle);

        mediator.Send(Arg.Any<UpdateLink.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new UpdateLinkFeature.Handler(playlistRepository, linkRepository, authService,
                    youtubeService, _clock, _localizer);
                return handler.Handle(callInfo.Arg<UpdateLink.Command>(), CancellationToken.None);
            });

        var result = await mediator.Send(command, CancellationToken.None);

        link.Title.Should().Be(newTitle);
        result.Should().Be(Unit.Value);
        await linkRepository.Received().Update(Arg.Any<Link>());
    }

    [Fact]
    public async Task UpdateLinkHandler_GetVideoTitle_IfUrlChanged()
    {
        var command = new UpdateLink.Command
        {
            Id = 1,
            Url = "https://www.youtube.com/watch?v=b7k0a5hYnSI",
            Downloaded = false
        };
        var link = new Link
        {
            Playlist = new Playlist
            {
                UserId = 1
            },
            Title = "Rick Astley - Never Gonna Give You Up",
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ"
        };
        const string newTitle = "Natasha Bedingfield - Unwritten";

        var linkRepository = Substitute.For<ILinkRepository>();
        var authService = Substitute.For<IAuthService>();
        var playlistRepository = Substitute.For<IPlaylistRepository>();
        var youtubeService = Substitute.For<IYoutubeService>();
        var mediator = Substitute.For<IMediator>();

        linkRepository.Get(Arg.Any<int>()).Returns(link);
        authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);
        playlistRepository.LinkUrlExistsInOtherLinksThan(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(false);
        youtubeService.GetVideoTitle(Arg.Any<string>()).Returns(newTitle);

        mediator.Send(Arg.Any<UpdateLink.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new UpdateLinkFeature.Handler(playlistRepository, linkRepository, authService,
                    youtubeService, _clock, _localizer);
                return handler.Handle(callInfo.Arg<UpdateLink.Command>(), CancellationToken.None);
            });

        var result = await mediator.Send(command, CancellationToken.None);

        link.Title.Should().Be(newTitle);
        result.Should().Be(Unit.Value);
        await linkRepository.Received().Update(Arg.Any<Link>());
    }

    [Fact]
    public async Task UpdateLinkHandler_DontCallGetVideoTitle_IfTitleIsNotEmptyAndUrlDidNotChanged()
    {
        var command = new UpdateLink.Command
        {
            Id = 1,
            Title = "Rick Astley - Never Gonna Give You Up",
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            Downloaded = false
        };
        var link = new Link
        {
            Playlist = new Playlist
            {
                UserId = 1
            },
            Title = "Rick Astley - Never Gonna Give You Up",
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ"
        };
        const string oldTitle = "Rick Astley - Never Gonna Give You Up";

        var linkRepository = Substitute.For<ILinkRepository>();
        var authService = Substitute.For<IAuthService>();
        var playlistRepository = Substitute.For<IPlaylistRepository>();
        var mediator = Substitute.For<IMediator>();

        linkRepository.Get(Arg.Any<int>()).Returns(link);
        authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);
        playlistRepository.LinkUrlExistsInOtherLinksThan(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(false);

        mediator.Send(Arg.Any<UpdateLink.Command>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new UpdateLinkFeature.Handler(playlistRepository, linkRepository, authService,
                    _youtubeService, _clock, _localizer);
                return handler.Handle(callInfo.Arg<UpdateLink.Command>(), CancellationToken.None);
            });

        var result = await mediator.Send(command, CancellationToken.None);

        link.Title.Should().Be(oldTitle);
        result.Should().Be(Unit.Value);
        await _youtubeService.DidNotReceive().GetVideoTitle(Arg.Any<string>());
        await linkRepository.Received().Update(Arg.Any<Link>());
    }
}