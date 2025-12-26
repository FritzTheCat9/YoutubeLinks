using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Users.Helpers;
using YoutubeLinks.Shared.Features.Users.Queries;

namespace YoutubeLinks.IntegrationTests.Features.Users.Queries;

public class GetAllUsersFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllUsers_ShouldReturnPaginatedUsers()
    {
        var user1 = User.Create("testuser1@gmail.com", "TestUser1", ThemeColor.Light, false, true);
        user1.SetPassword("Asd123!", PasswordService);
        var user2 = User.Create("testuser2@gmail.com", "TestUser2", ThemeColor.Light, false, true);
        user2.SetPassword("Asd123!", PasswordService);
        var user3 = User.Create("testuser3@gmail.com", "TestUser3", ThemeColor.Light, false, true);
        user3.SetPassword("Asd123!", PasswordService);

        var users = new List<User>
        {
            user1,
            user2,
            user3
        };

        await Context.Users.AddRangeAsync(users);
        await Context.SaveChangesAsync();

        await Logout();

        var command = new GetAllUsers.Query
        {
            SearchTerm = "",
            Page = 1,
            PageSize = 2,
            SortColumn = "email",
            SortOrder = SortOrder.Ascending,
        };

        var paginatedUsers = await UserApiClient.GetAllUsers(command);

        Assert.NotNull(paginatedUsers);
        Assert.Equal(2, paginatedUsers.Items.Count);
        var sorted = paginatedUsers.Items.OrderBy(x => x.Email).ToList();
        Assert.Equal(sorted, paginatedUsers.Items);
        Assert.Equal(command.Page, paginatedUsers.Page);
        Assert.Equal(command.PageSize, paginatedUsers.PageSize);
        Assert.Equal(5, paginatedUsers.TotalCount);
        Assert.Equal(3, paginatedUsers.PagesCount);
        Assert.True(paginatedUsers.HasNextPage);
        Assert.False(paginatedUsers.HasPreviousPage);

        foreach (var returnedUser in paginatedUsers.Items)
        {
            var matchingUser = users.FirstOrDefault(x => x.Id == returnedUser.Id);

            Assert.NotNull(matchingUser);
            Assert.Equal(returnedUser.Id, matchingUser.Id);
            Assert.Equal(returnedUser.Email, matchingUser.Email);
            Assert.Equal(returnedUser.UserName, matchingUser.UserName);
        }
    }
}