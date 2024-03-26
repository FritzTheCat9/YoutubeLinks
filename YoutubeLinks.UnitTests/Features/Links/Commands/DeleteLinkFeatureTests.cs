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

        public DeleteLinkFeatureTests()
        {
            _authService = Substitute.For<IAuthService>();
        }

        [Fact]
        public async Task DeleteLinkHandler_ThrowsNotFoundException_IfLinkIsNotFound()
        {
            var command = new DeleteLink.Command
            {
                Id = 1,
            };

            var linkRepository = Substitute.For<ILinkRepository>();
            var mediator = Substitute.For<IMediator>();

            linkRepository.Get(Arg.Any<int>()).Returns(Task.FromResult<Link>(null));

            mediator.Send(Arg.Any<DeleteLink.Command>(), CancellationToken.None)
                .Returns(callInfo =>
                {
                    var handler = new DeleteLinkFeature.Handler(_authService, linkRepository);
                    return handler.Handle(callInfo.Arg<DeleteLink.Command>(), CancellationToken.None);
                });

            var action = async () => await mediator.Send(command, CancellationToken.None);

            await Assert.ThrowsAsync<MyNotFoundException>(action);
            await linkRepository.DidNotReceive().Delete(Arg.Any<Link>());
        }

        [Fact]
        public async Task DeleteLinkHandler_ThrowsForbiddenException_IfPlaylistIsNotOwnedByLoggedInUser()
        {
            var command = new DeleteLink.Command
            {
                Id = 1,
            };

            var linkRepository = Substitute.For<ILinkRepository>();
            var authService = Substitute.For<IAuthService>();
            var mediator = Substitute.For<IMediator>();

            linkRepository.Get(Arg.Any<int>()).Returns(new Link()
            {
                Playlist = new Playlist()
                {
                    UserId = 1,
                }
            });
            authService.IsLoggedInUser(Arg.Any<int>()).Returns(false);

            mediator.Send(Arg.Any<DeleteLink.Command>(), CancellationToken.None)
               .Returns(callInfo =>
               {
                   var handler = new DeleteLinkFeature.Handler(authService, linkRepository);
                   return handler.Handle(callInfo.Arg<DeleteLink.Command>(), CancellationToken.None);
               });

            var action = async () => await mediator.Send(command, CancellationToken.None);

            await Assert.ThrowsAsync<MyForbiddenException>(action);
            await linkRepository.DidNotReceive().Delete(Arg.Any<Link>());
        }

        [Fact]
        public async Task DeleteLinkHandler_DeletesValidLink()
        {
            var command = new DeleteLink.Command
            {
                Id = 1,
            };

            var linkRepository = Substitute.For<ILinkRepository>();
            var authService = Substitute.For<IAuthService>();
            var mediator = Substitute.For<IMediator>();

            linkRepository.Get(Arg.Any<int>()).Returns(new Link()
            {
                Playlist = new Playlist()
                {
                    UserId = 1,
                }
            });
            authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);

            mediator.Send(Arg.Any<DeleteLink.Command>(), CancellationToken.None)
                .Returns(callInfo =>
                {
                    var handler = new DeleteLinkFeature.Handler(authService, linkRepository);
                    return handler.Handle(callInfo.Arg<DeleteLink.Command>(), CancellationToken.None);
                });

            var result = await mediator.Send(command, CancellationToken.None);

            result.Should().Be(Unit.Value);
            await linkRepository.Received().Delete(Arg.Any<Link>());
        }
    }
}
