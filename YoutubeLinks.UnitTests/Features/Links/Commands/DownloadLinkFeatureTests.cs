using FluentAssertions;
using MediatR;
using NSubstitute;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Links.Commands;
using YoutubeLinks.Api.Services;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Shared.Features.Links.Helpers;

namespace YoutubeLinks.UnitTests.Features.Links.Commands
{
    public class DownloadLinkFeatureTests
    {
        private readonly IAuthService _authService;
        private readonly IYoutubeService _youtubeService;

        public DownloadLinkFeatureTests()
        {
            _authService = Substitute.For<IAuthService>();
            _youtubeService = Substitute.For<IYoutubeService>();
        }

        [Fact]
        public async Task DownloadLinkHandler_ThrowsNotFoundException_IfLinkIsNotFound()
        {
            var command = new DownloadLink.Command
            {
                Id = 1,
                YoutubeFileType = YoutubeFileType.MP3,
            };

            var linkRepository = Substitute.For<ILinkRepository>();
            var mediator = Substitute.For<IMediator>();

            linkRepository.Get(Arg.Any<int>()).Returns(Task.FromResult<Link>(null));

            mediator.Send(Arg.Any<DownloadLink.Command>(), CancellationToken.None)
                .Returns(callInfo =>
                {
                    var handler = new DownloadLinkFeature.Handler(_authService, linkRepository, _youtubeService);
                    return handler.Handle(callInfo.Arg<DownloadLink.Command>(), CancellationToken.None);
                });

            var action = async () => await mediator.Send(command, CancellationToken.None);

            await Assert.ThrowsAsync<MyNotFoundException>(action);
        }

        [Fact]
        public async Task DownloadLinkHandler_ThrowsForbiddenException_IfPlaylistIsNotOwnedByLoggedInUserOrPlaylistIsNotPublic()
        {
            var command = new DownloadLink.Command
            {
                Id = 1,
                YoutubeFileType = YoutubeFileType.MP3,
            };

            var linkRepository = Substitute.For<ILinkRepository>();
            var authService = Substitute.For<IAuthService>();
            var mediator = Substitute.For<IMediator>();

            linkRepository.Get(Arg.Any<int>()).Returns(new Link()
            {
                Playlist = new Playlist()
                {
                    UserId = 1,
                    Public = false,
                }
            });
            authService.IsLoggedInUser(Arg.Any<int>()).Returns(false);

            mediator.Send(Arg.Any<DownloadLink.Command>(), CancellationToken.None)
                .Returns(callInfo =>
                {
                    var handler = new DownloadLinkFeature.Handler(authService, linkRepository, _youtubeService);
                    return handler.Handle(callInfo.Arg<DownloadLink.Command>(), CancellationToken.None);
                });

            var action = async () => await mediator.Send(command, CancellationToken.None);

            await Assert.ThrowsAsync<MyForbiddenException>(action);
        }

        [Fact]
        public async Task DownloadLinkHandler_UserPlaylist_ReturnsYoutubeFile()
        {
            var command = new DownloadLink.Command
            {
                Id = 1,
                YoutubeFileType = YoutubeFileType.MP3,
            };
            var youtubeFile = new YoutubeFile();

            var linkRepository = Substitute.For<ILinkRepository>();
            var authService = Substitute.For<IAuthService>();
            var youtubeService = Substitute.For<IYoutubeService>();
            var mediator = Substitute.For<IMediator>();

            linkRepository.Get(Arg.Any<int>()).Returns(new Link()
            {
                Playlist = new Playlist()
                {
                    UserId = 1,
                    Public = false,
                }
            });
            authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);
            youtubeService.GetMP3File(Arg.Any<string>()).Returns(youtubeFile);

            mediator.Send(Arg.Any<DownloadLink.Command>(), CancellationToken.None)
                .Returns(callInfo =>
                {
                    var handler = new DownloadLinkFeature.Handler(authService, linkRepository, youtubeService);
                    return handler.Handle(callInfo.Arg<DownloadLink.Command>(), CancellationToken.None);
                });

            var result = await mediator.Send(command, CancellationToken.None);

            result.Should().Be(youtubeFile);
            await youtubeService.Received().GetMP3File(Arg.Any<string>());
            await youtubeService.DidNotReceive().GetMP4File(Arg.Any<string>());
        }

        [Fact]
        public async Task DownloadLinkHandler_PublicPlaylist_ReturnsYoutubeFile()
        {
            var command = new DownloadLink.Command
            {
                Id = 1,
                YoutubeFileType = YoutubeFileType.MP4,
            };
            var youtubeFile = new YoutubeFile();

            var linkRepository = Substitute.For<ILinkRepository>();
            var authService = Substitute.For<IAuthService>();
            var youtubeService = Substitute.For<IYoutubeService>();
            var mediator = Substitute.For<IMediator>();

            linkRepository.Get(Arg.Any<int>()).Returns(new Link()
            {
                Playlist = new Playlist()
                {
                    UserId = 1,
                    Public = true,
                }
            });
            authService.IsLoggedInUser(Arg.Any<int>()).Returns(false);
            youtubeService.GetMP4File(Arg.Any<string>()).Returns(youtubeFile);

            mediator.Send(Arg.Any<DownloadLink.Command>(), CancellationToken.None)
                .Returns(callInfo =>
                {
                    var handler = new DownloadLinkFeature.Handler(authService, linkRepository, youtubeService);
                    return handler.Handle(callInfo.Arg<DownloadLink.Command>(), CancellationToken.None);
                });

            var result = await mediator.Send(command, CancellationToken.None);

            result.Should().Be(youtubeFile);
            await youtubeService.Received().GetMP4File(Arg.Any<string>());
            await youtubeService.DidNotReceive().GetMP3File(Arg.Any<string>());
        }
    }
}
