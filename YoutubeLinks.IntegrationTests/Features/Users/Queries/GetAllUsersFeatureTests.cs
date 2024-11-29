using FluentAssertions;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Users.Queries;

namespace YoutubeLinks.IntegrationTests.Features.Users.Queries;

public class GetAllUsersFeatureTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllUsers_ShouldReturnPaginatedUsers()
    {
        var users = new List<User>()
        {
            new()
            {
                Email = "testuser1@gmail.com",
                UserName = "TestUser1",
                Password = PasswordService.Hash("Asd123!"),
                EmailConfirmed = true,
            },
            new()
            {
                Email = "testuser2@gmail.com",
                UserName = "TestUser2",
                Password = PasswordService.Hash("Asd123!"),
                EmailConfirmed = true,
            },
            new()
            {
                Email = "testuser3@gmail.com",
                UserName = "TestUser3",
                Password = PasswordService.Hash("Asd123!"),
                EmailConfirmed = true,
            },
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