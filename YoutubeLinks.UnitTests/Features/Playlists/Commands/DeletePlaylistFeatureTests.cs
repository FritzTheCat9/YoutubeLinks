using FluentAssertions;
using MediatR;
using NSubstitute;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Playlists.Commands;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Commands;

namespace YoutubeLinks.UnitTests.Features.Playlists.Commands
{
    public class DeletePlaylistFeatureTests
    {
        private readonly IAuthService _authService;

        public DeletePlaylistFeatureTests()
        {
            _authService = Substitute.For<IAuthService>();
        }

        [Fact]
        public async Task DeletePlaylistHandler_ThrowsNotFoundException_IfPlaylistIsNotFound()
        {
            var command = new DeletePlaylist.Command
            {
                Id = 1,
            };

            var playlistRepository = Substitute.For<IPlaylistRepository>();
            var mediator = Substitute.For<IMediator>();

            playlistRepository.Get(Arg.Any<int>()).Returns(Task.FromResult<Playlist>(null));

            mediator.Send(Arg.Any<DeletePlaylist.Command>(), CancellationToken.None)
                .Returns(callInfo =>
                {
                    var handler = new DeletePlaylistFeature.Handler(playlistRepository, _authService);
                    return handler.Handle(callInfo.Arg<DeletePlaylist.Command>(), CancellationToken.None);
                });

            var action = async () => await mediator.Send(command, CancellationToken.None);

            await Assert.ThrowsAsync<MyNotFoundException>(action);
            await playlistRepository.DidNotReceive().Delete(Arg.Any<Playlist>());
        }

        [Fact]
        public async Task DeletePlaylistHandler_ThrowsForbiddenException_IfPlaylistIsNotOwnedByLoggedInUser()
        {
            var command = new DeletePlaylist.Command
            {
                Id = 1,
            };

            var playlistRepository = Substitute.For<IPlaylistRepository>();
            var authService = Substitute.For<IAuthService>();
            var mediator = Substitute.For<IMediator>();

            playlistRepository.Get(Arg.Any<int>()).Returns(new Playlist
            {
                UserId = 1,
            });
            authService.IsLoggedInUser(Arg.Any<int>()).Returns(false);

            mediator.Send(Arg.Any<DeletePlaylist.Command>(), CancellationToken.None)
                .Returns(callInfo =>
                {
                    var handler = new DeletePlaylistFeature.Handler(playlistRepository, authService);
                    return handler.Handle(callInfo.Arg<DeletePlaylist.Command>(), CancellationToken.None);
                });

            var action = async () => await mediator.Send(command, CancellationToken.None);

            await Assert.ThrowsAsync<MyForbiddenException>(action);
            await playlistRepository.DidNotReceive().Delete(Arg.Any<Playlist>());
        }


        [Fact]
        public async Task DeletePlaylistHandler_DeletesPlaylist()
        {
            var command = new DeletePlaylist.Command
            {
                Id = 1,
            };

            var playlistRepository = Substitute.For<IPlaylistRepository>();
            var authService = Substitute.For<IAuthService>();
            var mediator = Substitute.For<IMediator>();

            playlistRepository.Get(Arg.Any<int>()).Returns(new Playlist
            {
                UserId = 1,
            });
            authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);

            mediator.Send(Arg.Any<DeletePlaylist.Command>(), CancellationToken.None)
                .Returns(callInfo =>
                {
                    var handler = new DeletePlaylistFeature.Handler(playlistRepository, authService);
                    return handler.Handle(callInfo.Arg<DeletePlaylist.Command>(), CancellationToken.None);
                });

            var result = await mediator.Send(command, CancellationToken.None);

            result.Should().Be(Unit.Value);
            await playlistRepository.Received().Delete(Arg.Any<Playlist>());
        }
    }
}
