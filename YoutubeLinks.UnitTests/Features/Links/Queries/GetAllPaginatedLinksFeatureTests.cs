using FluentAssertions;
using MediatR;
using NSubstitute;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Links.Queries;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Queries;
using YoutubeLinks.Shared.Features.Links.Responses;

namespace YoutubeLinks.UnitTests.Features.Links.Queries
{
    public class GetAllPaginatedLinksFeatureTests
    {
        private readonly ILinkRepository _linkRepository;
        private readonly IAuthService _authService;

        public GetAllPaginatedLinksFeatureTests()
        {
            _linkRepository = Substitute.For<ILinkRepository>();
            _authService = Substitute.For<IAuthService>();
        }

        [Fact]
        public async Task GetAllPaginatedLinksHandler_ThrowsNotFoundException_IfPlaylistIsNotFound()
        {
            var query = new GetAllPaginatedLinks.Query
            {
                Page = 1,
                PageSize = 10,
                SortColumn = "title",
                SortOrder = SortOrder.Ascending,
                SearchTerm = "",
                PlaylistId = 1,
            };

            var playlistRepository = Substitute.For<IPlaylistRepository>();
            var mediator = Substitute.For<IMediator>();

            playlistRepository.Get(Arg.Any<int>()).Returns(Task.FromResult<Playlist>(null));

            mediator.Send(Arg.Any<GetAllPaginatedLinks.Query>(), CancellationToken.None)
                .Returns(callInfo =>
                {
                    var handler = new GetAllPaginatedLinksFeature.Handler(_linkRepository, playlistRepository, _authService);
                    return handler.Handle(callInfo.Arg<GetAllPaginatedLinks.Query>(), CancellationToken.None);
                });

            var action = async () => await mediator.Send(query, CancellationToken.None);

            await Assert.ThrowsAsync<MyNotFoundException>(action);
        }

        [Fact]
        public async Task GetAllPaginatedLinksHandler_ReturnsLinksPagedList()
        {
            var query = new GetAllPaginatedLinks.Query
            {
                Page = 1,
                PageSize = 10,
                SortColumn = "title",
                SortOrder = SortOrder.Ascending,
                SearchTerm = "",
                PlaylistId = 1,
            };

            var list = new List<Link>()
            {
                new()
                {
                    Id = 1,
                }
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
            linkRepository.AsQueryable(Arg.Any<int>(), Arg.Any<bool>()).Returns(list.AsQueryable());

            mediator.Send(Arg.Any<GetAllPaginatedLinks.Query>(), CancellationToken.None)
                .Returns(callInfo =>
                {
                    var handler = new GetAllPaginatedLinksFeature.Handler(linkRepository, playlistRepository, authService);
                    return handler.Handle(callInfo.Arg<GetAllPaginatedLinks.Query>(), CancellationToken.None);
                });

            var result = await mediator.Send(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Should().BeOfType<PagedList<LinkDto>>();
            result.TotalCount.Should().Be(1);
            result.Items.Count.Should().Be(1);
        }
    }
}
