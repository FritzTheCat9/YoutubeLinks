using Blazored.LocalStorage;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Users.Responses;

namespace YoutubeLinks.Blazor.Auth;

public class JwtProvider(ILocalStorageService localStorageService) : IJwtProvider
{
    public async Task<JwtDto> GetJwtDto()
    {
        return await localStorageService.GetItemAsync<JwtDto>(Jwt.Dto) ?? null;
    }

    public async Task SetJwtDto(JwtDto token)
    {
        await localStorageService.SetItemAsync(Jwt.Dto, token);
    }

    public async Task RemoveJwtDto()
    {
        await localStorageService.RemoveItemAsync(Jwt.Dto);
    }
}