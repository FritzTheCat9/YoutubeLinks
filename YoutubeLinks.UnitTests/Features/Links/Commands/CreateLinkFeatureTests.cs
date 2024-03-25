using FluentAssertions;
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

namespace YoutubeLinks.UnitTests.Features.Links.Commands
{
    public class CreateLinkFeatureTests
    {
        private readonly ILinkRepository _linkRepository;
        private readonly IPlaylistRepository _playlistRepository;
        private readonly IAuthService _authService;
        private readonly IYoutubeService _youtubeService;
        private readonly IClock _clock;
        private readonly IStringLocalizer<ApiValidationMessage> _localizer;

        public CreateLinkFeatureTests()
        {
            _linkRepository = Substitute.For<ILinkRepository>();
            _playlistRepository = Substitute.For<IPlaylistRepository>();
            _authService = Substitute.For<IAuthService>();
            _youtubeService = Substitute.For<IYoutubeService>();
            _clock = Substitute.For<IClock>();
            _localizer = Substitute.For<IStringLocalizer<ApiValidationMessage>>();
        }

        [Fact]
        public async Task Should_ThrowValidationException_WhenVideoIdIsNull()
        {
            var command = new CreateLink.Command
            {
                Url = string.Empty,
                PlaylistId = 1,
            };

            var handler = new CreateLinkFeature.Handler(_linkRepository, _playlistRepository, _authService, _youtubeService, _clock, _localizer);
            var action = async () => await handler.Handle(command, CancellationToken.None);

            await Assert.ThrowsAsync<MyValidationException>(action);
            await _linkRepository.DidNotReceive().Create(Arg.Any<Link>());
        }

        [Fact]
        public async Task Should_ThrowNotFoundException_WhenPlaylistIsNotFound()
        {
            var command = new CreateLink.Command
            {
                Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ&ab_channel=RickAstley",
                PlaylistId = 1,
            };

            var playlistRepository = Substitute.For<IPlaylistRepository>();

            playlistRepository.Get(Arg.Any<int>()).Returns(Task.FromResult<Playlist>(null));

            var handler = new CreateLinkFeature.Handler(_linkRepository, playlistRepository, _authService, _youtubeService, _clock, _localizer);
            var action = async () => await handler.Handle(command, CancellationToken.None);

            await Assert.ThrowsAsync<MyNotFoundException>(action);
            await _linkRepository.DidNotReceive().Create(Arg.Any<Link>());
        }

        [Fact]
        public async Task Should_ThrowForbiddenException_WhenPlaylistIsNotOwnedByLoggedInUser()
        {
            var command = new CreateLink.Command
            {
                Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ&ab_channel=RickAstley",
                PlaylistId = 1,
            };

            var playlistRepository = Substitute.For<IPlaylistRepository>();
            var authService = Substitute.For<IAuthService>();

            playlistRepository.Get(Arg.Any<int>()).Returns(new Playlist());
            authService.IsLoggedInUser(Arg.Any<int>()).Returns(false);

            var handler = new CreateLinkFeature.Handler(_linkRepository, playlistRepository, authService, _youtubeService, _clock, _localizer);
            var action = async () => await handler.Handle(command, CancellationToken.None);

            await Assert.ThrowsAsync<MyForbiddenException>(action);
            await _linkRepository.DidNotReceive().Create(Arg.Any<Link>());
        }

        [Fact]
        public async Task Should_ThrowValidationException_WhenLinkUrlIsNotUniqueInPlaylist()
        {
            var command = new CreateLink.Command
            {
                Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ&ab_channel=RickAstley",
                PlaylistId = 1,
            };

            var playlistRepository = Substitute.For<IPlaylistRepository>();
            var authService = Substitute.For<IAuthService>();

            playlistRepository.Get(Arg.Any<int>()).Returns(new Playlist());
            authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);
            playlistRepository.LinkUrlExists(Arg.Any<string>(), Arg.Any<int>()).Returns(true);

            var handler = new CreateLinkFeature.Handler(_linkRepository, playlistRepository, authService, _youtubeService, _clock, _localizer);
            var action = async () => await handler.Handle(command, CancellationToken.None);

            await Assert.ThrowsAsync<MyValidationException>(action);
            await _linkRepository.DidNotReceive().Create(Arg.Any<Link>());
        }

        [Fact]
        public async Task Should_ReturnCreatedLinkId_WhenLinkIsValid()
        {
            var command = new CreateLink.Command
            {
                Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ&ab_channel=RickAstley",
                PlaylistId = 1,
            };

            var playlistRepository = Substitute.For<IPlaylistRepository>();
            var authService = Substitute.For<IAuthService>();
            var linkRepository = Substitute.For<ILinkRepository>();

            playlistRepository.Get(Arg.Any<int>()).Returns(new Playlist());
            authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);
            playlistRepository.LinkUrlExists(Arg.Any<string>(), Arg.Any<int>()).Returns(false);
            linkRepository.Create(Arg.Any<Link>()).Returns(1);

            var handler = new CreateLinkFeature.Handler(linkRepository, playlistRepository, authService, _youtubeService, _clock, _localizer);
            var result = await handler.Handle(command, CancellationToken.None);

            result.Should().Be(1);
            await linkRepository.Received().Create(Arg.Any<Link>());
        }
    }
}
