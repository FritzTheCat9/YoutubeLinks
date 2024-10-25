using FluentAssertions;
using MediatR;
using NSubstitute;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Users.Queries;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Users.Queries;
using YoutubeLinks.Shared.Features.Users.Responses;

namespace YoutubeLinks.UnitTests.Features.Users.Queries;

public class GetAllUsersFeatureTests
{
    [Fact]
    public async Task GetAllUsersHandler_ReturnsPlaylistsPagedList()
    {
        var query = new GetAllUsers.Query
        {
            Page = 1,
            PageSize = 10,
            SortColumn = "",
            SortOrder = SortOrder.Ascending,
            SearchTerm = ""
        };

        var list = new List<User>
        {
            new()
            {
                Id = 1
            }
        };

        var userRepository = Substitute.For<IUserRepository>();
        var mediator = Substitute.For<IMediator>();

        userRepository.AsQueryable().Returns(list.AsQueryable());

        mediator.Send(Arg.Any<GetAllUsers.Query>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new GetAllUsersFeature.Handler(userRepository);
                return handler.Handle(callInfo.Arg<GetAllUsers.Query>(), CancellationToken.None);
            });

        var result = await mediator.Send(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeOfType<PagedList<UserDto>>();
        result.TotalCount.Should().Be(1);
        result.Items.Count.Should().Be(1);
    }
}