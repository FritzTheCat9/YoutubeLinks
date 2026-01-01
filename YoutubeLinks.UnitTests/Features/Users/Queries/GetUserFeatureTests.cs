using NSubstitute;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Users.Queries;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Users.Helpers;
using YoutubeLinks.Shared.Features.Users.Queries;
using YoutubeLinks.Shared.Features.Users.Responses;

namespace YoutubeLinks.UnitTests.Features.Users.Queries;

public class GetUserFeatureTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();

    [Fact]
    public async Task GetUserHandler_ThrowsNotFoundException_IfUserIsNotFound()
    {
        var query = new GetUser.Query
        {
            Id = 1
        };

        _userRepository.Get(Arg.Any<int>()).Returns(Task.FromResult<User>(null));

        var handler = new GetUserFeature.Handler(_userRepository);

        await Assert.ThrowsAsync<MyNotFoundException>(() => handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task GetUserHandler_ReturnsUserDto()
    {
        var query = new GetUser.Query
        {
            Id = 1
        };

        var user = User.Create("testuser@gmail.com", "TestUser", ThemeColor.Light, true, true);

        _userRepository.Get(Arg.Any<int>()).Returns(user);

        var handler = new GetUserFeature.Handler(_userRepository);
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<UserDto>(result);
    }
}