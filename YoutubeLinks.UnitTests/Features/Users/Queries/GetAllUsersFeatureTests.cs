using NSubstitute;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Users.Queries;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Users.Helpers;
using YoutubeLinks.Shared.Features.Users.Queries;
using YoutubeLinks.Shared.Features.Users.Responses;

namespace YoutubeLinks.UnitTests.Features.Users.Queries;

public class GetAllUsersFeatureTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();

    [Fact]
    public async Task GetAllUsersHandler_ReturnsUsersPagedList()
    {
        var query = new GetAllUsers.Query
        {
            Page = 1,
            PageSize = 10,
            SortColumn = "",
            SortOrder = SortOrder.Ascending,
            SearchTerm = ""
        };

        var users = new List<User>
        {
            User.Create("test@test.com", "TestUser", ThemeColor.Light, true, true)
        };

        var pagedUsers = new PagedList<User>(
            items: users,
            page: query.Page,
            pageSize: query.PageSize,
            totalCount: users.Count
        );

        _userRepository.GetAllPaginated(query).Returns(pagedUsers);

        var handler = new GetAllUsersFeature.Handler(_userRepository);
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<PagedList<UserDto>>(result);
        Assert.Equal(pagedUsers.TotalCount, result.TotalCount);
        Assert.Single(result.Items);
        Assert.Equal("TestUser", result.Items.First().UserName);
    }
}
