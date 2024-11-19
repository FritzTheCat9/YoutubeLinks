using YoutubeLinks.Shared.Features.Users.Responses;

namespace YoutubeLinks.Shared.Abstractions;

public interface IJwtProvider
{
    Task<JwtDto> GetJwtDto();
    Task SetJwtDto(JwtDto token);
    Task RemoveJwtDto();
}