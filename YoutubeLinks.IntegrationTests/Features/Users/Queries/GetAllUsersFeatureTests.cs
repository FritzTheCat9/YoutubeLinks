using FluentAssertions;
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

        paginatedUsers.Should().NotBeNull();
        paginatedUsers.Items.Should().HaveCount(2);
        paginatedUsers.Items.Should().BeInAscendingOrder(x => x.Email);
        paginatedUsers.Page.Should().Be(command.Page);
        paginatedUsers.PageSize.Should().Be(command.PageSize);
        paginatedUsers.TotalCount.Should().Be(5);
        paginatedUsers.PagesCount.Should().Be(3);
        paginatedUsers.HasNextPage.Should().Be(true);
        paginatedUsers.HasPreviousPage.Should().Be(false);

        foreach (var returnedUser in paginatedUsers.Items)
        {
            var matchingUser = users.FirstOrDefault(x => x.Id == returnedUser.Id);

            matchingUser.Should().NotBeNull();
            matchingUser.Should().Match<User>(x =>
                x.Id == returnedUser.Id &&
                x.Email == returnedUser.Email &&
                x.UserName == returnedUser.UserName);
        }
    }
}