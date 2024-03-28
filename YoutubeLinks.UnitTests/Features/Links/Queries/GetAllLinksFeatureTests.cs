using FluentAssertions;
using MediatR;
using NSubstitute;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Links.Queries;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Queries;

namespace YoutubeLinks.UnitTests.Features.Links.Queries
{
    public class GetAllLinksFeatureTests
    {
        private readonly ILinkRepository _linkRepository;
        private readonly IAuthService _authService;

        public GetAllLinksFeatureTests()
        {
            _linkRepository = Substitute.For<ILinkRepository>();
            _authService = Substitute.For<IAuthService>();
        }

        [Fact]
        public async Task GetAllLinksHandler_ThrowsNotFoundException_IfPlaylistIsNotFound()
        {
            var query = new GetAllLinks.Query
            {
                PlaylistId = 1,
                Downloaded = false,
            };

            var playlistRepository = Substitute.For<IPlaylistRepository>();
            var mediator = Substitute.For<IMediator>();

            playlistRepository.Get(Arg.Any<int>()).Returns(Task.FromResult<Playlist>(null));

            mediator.Send(Arg.Any<GetAllLinks.Query>(), CancellationToken.None)
                .Returns(callInfo =>
                {
                    var handler = new GetAllLinksFeature.Handler(_linkRepository, playlistRepository, _authService);
                    return handler.Handle(callInfo.Arg<GetAllLinks.Query>(), CancellationToken.None);
                });

            var action = async () => await mediator.Send(query, CancellationToken.None);

            await Assert.ThrowsAsync<MyNotFoundException>(action);
        }

        [Fact]
        public async Task GetAllLinksHandler_ReturnsLinkInfoDtos_IfPlaylistIsOwnedByUser()
        {
            var query = new GetAllLinks.Query
            {
                PlaylistId = 1,
                Downloaded = false,
            };

            var links = new List<Link>()
            {
                new()
                {
                     Id = 1,
                },
            };

            var playlistRepository = Substitute.For<IPlaylistRepository>();
            var authService = Substitute.For<IAuthService>();
            var linkRepository = Substitute.For<ILinkRepository>();
            var mediator = Substitute.For<IMediator>();

            playlistRepository.Get(Arg.Any<int>()).Returns(new Playlist
            {
                UserId = 1,
            });
            authService.IsLoggedInUser(Arg.Any<int>()).Returns(true);
            linkRepository.AsQueryable(Arg.Any<int>(), Arg.Any<bool>()).Returns(links.AsQueryable());

            mediator.Send(Arg.Any<GetAllLinks.Query>(), CancellationToken.None)
                .Returns(callInfo =>
                {
                    var handler = new GetAllLinksFeature.Handler(linkRepository, playlistRepository, authService);
                    return handler.Handle(callInfo.Arg<GetAllLinks.Query>(), CancellationToken.None);
                });

            var result = await mediator.Send(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Should().BeOfType<List<GetAllLinks.LinkInfoDto>>();
            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetAllLinksHandler_ReturnsLinkInfoDtos_IfPlaylistIsNotOwnedByUser()
        {
            var query = new GetAllLinks.Query
            {
                PlaylistId = 1,
                Downloaded = false,
            };

            var links = new List<Link>()
            {
                new()
                {
                     Id = 1,
                },
            };

            var playlistRepository = Substitute.For<IPlaylistRepository>();
            var authService = Substitute.For<IAuthService>();
            var linkRepository = Substitute.For<ILinkRepository>();
            var mediator = Substitute.For<IMediator>();

            playlistRepository.Get(Arg.Any<int>()).Returns(new Playlist
            {
                UserId = 1,
            });
            authService.IsLoggedInUser(Arg.Any<int>()).Returns(false);
            linkRepository.AsQueryable(Arg.Any<int>(), Arg.Any<bool>()).Returns(links.AsQueryable());

            mediator.Send(Arg.Any<GetAllLinks.Query>(), CancellationToken.None)
                .Returns(callInfo =>
                {
                    var handler = new GetAllLinksFeature.Handler(linkRepository, playlistRepository, authService);
                    return handler.Handle(callInfo.Arg<GetAllLinks.Query>(), CancellationToken.None);
                });

            var result = await mediator.Send(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Should().BeOfType<List<GetAllLinks.LinkInfoDto>>();
            result.Should().HaveCount(1);
        }
    }
}
