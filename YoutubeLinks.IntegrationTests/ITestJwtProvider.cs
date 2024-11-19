using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Users.Responses;

namespace YoutubeLinks.IntegrationTests;

public class TestJwtProvider : IJwtProvider
{
    private JwtDto _jwtDto = null!;

    public Task<JwtDto> GetJwtDto()
    {
        return Task.FromResult(_jwtDto);
    }

    public Task SetJwtDto(JwtDto token)
    {
        _jwtDto = token;
        return Task.CompletedTask;
    }

    public Task RemoveJwtDto()
    {
        _jwtDto = null!;
        return Task.CompletedTask;
    }
}