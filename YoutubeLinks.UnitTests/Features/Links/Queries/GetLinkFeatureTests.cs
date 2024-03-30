using FluentAssertions;
using MediatR;
using NSubstitute;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Links.Queries;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Queries;
using YoutubeLinks.Shared.Features.Links.Responses;

namespace YoutubeLinks.UnitTests.Features.Links.Queries
{
    public class GetLinkFeatureTests
    {
        private readonly IAuthService _authService;

        public GetLinkFeatureTests()
        {
            _authService = Substitute.For<IAuthService>();
        }

        [Fact]
        public async Task GetLinkHandler_ThrowsNotFoundException_IfLinkIsNotFound()
        {
            var query = new GetLink.Query
            {
                Id = 1,
            };

            var linkRepository = Substitute.For<ILinkRepository>();
            var mediator = Substitute.For<IMediator>();

            linkRepository.Get(Arg.Any<int>()).Returns(Task.FromResult<Link>(null));

            mediator.Send(Arg.Any<GetLink.Query>(), CancellationToken.None)
                .Returns(callInfo =>
                {
                    var handler = new GetLinkFeature.Handler(linkRepository, _authService);
                    return handler.Handle(callInfo.Arg<GetLink.Query>(), CancellationToken.None);
                });

            var action = async () => await mediator.Send(query, CancellationToken.None);

            await Assert.ThrowsAsync<MyNotFoundException>(action);
        }

        [Fact]
        public async Task GetLinkHandler_ThrowsForbiddenException_IfPlaylistIsNotOwnedByLoggedInUserAndPlaylistIsNotPublic()
        {
            var query = new GetLink.Query
            {
                Id = 1,
            };

            var linkRepository = Substitute.For<ILinkRepository>();
            var authService = Substitute.For<IAuthService>();
            var mediator = Substitute.For<IMediator>();

            linkRepository.Get(Arg.Any<int>()).Returns(new Link
            {
                Playlist = new Playlist
                {
                    UserId = 1,
                    Public = false,
                }
            });
            authService.IsLoggedInUser(Arg.Any<int>()).Returns(false);

            mediator.Send(Arg.Any<GetLink.Query>(), CancellationToken.None)
                .Returns(callInfo =>
                {
                    var handler = new GetLinkFeature.Handler(linkRepository, authService);
                    return handler.Handle(callInfo.Arg<GetLink.Query>(), CancellationToken.None);
                });

            var action = async () => await mediator.Send(query, CancellationToken.None);

            await Assert.ThrowsAsync<MyForbiddenException>(action);
        }

        [Fact]
        public async Task GetLinkHandler_ReturnsLinkDto_IfPlaylistIsPublic()
        {
            var query = new GetLink.Query
            {
                Id = 1,
            };

            var linkRepository = Substitute.For<ILinkRepository>();
            var authService = Substitute.For<IAuthService>();
            var mediator = Substitute.For<IMediator>();

            linkRepository.Get(Arg.Any<int>()).Returns(new Link
            {
                Playlist = new Playlist
                {
                    UserId = 1,
                    Public = true,
                }
            });
            authService.IsLoggedInUser(Arg.Any<int>()).Returns(false);

            mediator.Send(Arg.Any<GetLink.Query>(), CancellationToken.None)
                .Returns(callInfo =>
                {
                    var handler = new GetLinkFeature.Handler(linkRepository, authService);
                    return handler.Handle(callInfo.Arg<GetLink.Query>(), CancellationToken.None);
                });

            var result = await mediator.Send(query, CancellationToken.None);

            result.Should().BeOfType<LinkDto>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetLinkHandler_ReturnsLinkDto_IfPlaylistIsOwnedByLoggedInUser()
        {
            var query = new GetLink.Query
            {
                Id = 1,
            };

            var linkRepository = Substitute.For<ILinkRepository>();
            var authService = Substitute.For<IAuthService>();
            var mediator = Substitute.For<IMediator>();

            linkRepository.Get(Arg.Any<int>()).Returns(new Link
            {
                Playlist = new Playlist
                {
                    UserId = 1,
                    Public = false,
                }
            });
            authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);

            mediator.Send(Arg.Any<GetLink.Query>(), CancellationToken.None)
                .Returns(callInfo =>
                {
                    var handler = new GetLinkFeature.Handler(linkRepository, authService);
                    return handler.Handle(callInfo.Arg<GetLink.Query>(), CancellationToken.None);
                });

            var result = await mediator.Send(query, CancellationToken.None);

            result.Should().BeOfType<LinkDto>();
            result.Should().NotBeNull();
        }
    }
}
