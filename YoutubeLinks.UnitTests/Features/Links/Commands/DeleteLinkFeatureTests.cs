using NSubstitute;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Features.Links.Commands;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Api.Auth;
using FluentAssertions;
using MediatR;

namespace YoutubeLinks.UnitTests.Features.Links.Commands
{
    public class DeleteLinkFeatureTests
    {
        private readonly IAuthService _authService;
        private readonly ILinkRepository _linkRepository;

        public DeleteLinkFeatureTests()
        {
            _authService = Substitute.For<IAuthService>();
            _linkRepository = Substitute.For<ILinkRepository>();
        }

        [Fact]
        public async Task Should_ThrowNotFoundException_WhenLinkIsNotFound()
        {
            var command = new DeleteLink.Command
            {
                Id = 1,
            };

            var linkRepository = Substitute.For<ILinkRepository>();

            linkRepository.Get(Arg.Any<int>()).Returns(Task.FromResult<Link>(null));

            var handler = new DeleteLinkFeature.Handler(_authService, linkRepository);
            var action = async () => await handler.Handle(command, CancellationToken.None);

            await Assert.ThrowsAsync<MyNotFoundException>(action);
            await linkRepository.DidNotReceive().Delete(Arg.Any<Link>());
        }

        [Fact]
        public async Task Should_ThrowForbiddenException_WhenPlaylistIsNotOwnedByLoggedInUser()
        {
            var command = new DeleteLink.Command
            {
                Id = 1,
            };

            var linkRepository = Substitute.For<ILinkRepository>();
            var authService = Substitute.For<IAuthService>();

            linkRepository.Get(Arg.Any<int>()).Returns(new Link()
            {
                Playlist = new Playlist()
                {
                    UserId = 1,
                }
            });
            authService.IsLoggedInUser(Arg.Any<int>()).Returns(false);

            var handler = new DeleteLinkFeature.Handler(authService, linkRepository);
            var action = async () => await handler.Handle(command, CancellationToken.None);

            await Assert.ThrowsAsync<MyForbiddenException>(action);
            await linkRepository.DidNotReceive().Delete(Arg.Any<Link>());
        }

        [Fact]
        public async Task Should_DeleteLinkSuccessfully_WhenLinkIsValid()
        {
            var command = new DeleteLink.Command
            {
                Id = 1,
            };

            var linkRepository = Substitute.For<ILinkRepository>();
            var authService = Substitute.For<IAuthService>();

            linkRepository.Get(Arg.Any<int>()).Returns(new Link()
            {
                Playlist = new Playlist()
                {
                    UserId = 1,
                }
            });
            authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);

            var handler = new DeleteLinkFeature.Handler(authService, linkRepository);
            var result = await handler.Handle(command, CancellationToken.None);

            result.Should().Be(Unit.Value);
            await linkRepository.Received().Delete(Arg.Any<Link>());
        }
    }
}
