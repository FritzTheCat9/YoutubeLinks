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
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();

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

        _userRepository.AsQueryable().Returns(list.AsQueryable());

        var handler = new GetAllUsersFeature.Handler(_userRepository);
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<PagedList<UserDto>>(result);
        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Items);
    }
}