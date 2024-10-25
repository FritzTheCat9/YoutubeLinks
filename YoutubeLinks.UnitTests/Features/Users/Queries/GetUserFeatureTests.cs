using FluentAssertions;
using MediatR;
using NSubstitute;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Users.Queries;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Users.Queries;
using YoutubeLinks.Shared.Features.Users.Responses;

namespace YoutubeLinks.UnitTests.Features.Users.Queries;

public class GetUserFeatureTests
{
    [Fact]
    public async Task GetUserHandler_ThrowsNotFoundException_IfUserIsNotFound()
    {
        var query = new GetUser.Query
        {
            Id = 1
        };

        var userRepository = Substitute.For<IUserRepository>();
        var mediator = Substitute.For<IMediator>();

        userRepository.Get(Arg.Any<int>()).Returns(Task.FromResult<User>(null));

        mediator.Send(Arg.Any<GetUser.Query>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new GetUserFeature.Handler(userRepository);
                return handler.Handle(callInfo.Arg<GetUser.Query>(), CancellationToken.None);
            });

        var action = async () => await mediator.Send(query, CancellationToken.None);

        await Assert.ThrowsAsync<MyNotFoundException>(action);
    }

    [Fact]
    public async Task GetUserHandler_ReturnsUserDto()
    {
        var query = new GetUser.Query
        {
            Id = 1
        };

        var userRepository = Substitute.For<IUserRepository>();
        var mediator = Substitute.For<IMediator>();

        userRepository.Get(Arg.Any<int>()).Returns(new User());

        mediator.Send(Arg.Any<GetUser.Query>(), CancellationToken.None)
            .Returns(callInfo =>
            {
                var handler = new GetUserFeature.Handler(userRepository);
                return handler.Handle(callInfo.Arg<GetUser.Query>(), CancellationToken.None);
            });

        var result = await mediator.Send(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeOfType<UserDto>();
    }
}