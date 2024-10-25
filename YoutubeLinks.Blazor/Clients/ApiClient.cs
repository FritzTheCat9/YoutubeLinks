using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Newtonsoft.Json;
using YoutubeLinks.Blazor.Auth;
using YoutubeLinks.Shared.Exceptions;

namespace YoutubeLinks.Blazor.Clients;

public interface IApiClient
{
    Task<HttpResponseMessage> Get(string url);
    Task<TResponse> Get<TResponse>(string url);
    Task Post<TRequest>(string url, TRequest tRequest);
    Task<TResponse> Post<TRequest, TResponse>(string url, TRequest tRequest);
    Task<HttpResponseMessage> PostReturnHttpResponseMessage<TRequest>(string url, TRequest tRequest);
    Task Put<TRequest>(string url, TRequest tRequest);
    Task Put(string url);
    Task Delete(string url);
}

public class ApiClient(
    HttpClient client,
    IJwtProvider jwtProvider) : IApiClient
{
    private const string AuthScheme = "Bearer";
    private const string LanguageHeader = "Accept-Language";
    private readonly string _baseUrl = client.BaseAddress?.ToString();

    public async Task<HttpResponseMessage> Get(string url)
    {
        await AddHeaderValues();
        var response = await client.GetAsync($"{_baseUrl}{url}");

        if (!response.IsSuccessStatusCode)
        {
            await HandleErrors(response);
        }

        return response;
    }

    public async Task<TResponse> Get<TResponse>(string url)
    {
        await AddHeaderValues();
        var response = await client.GetAsync($"{_baseUrl}{url}");

        if (!response.IsSuccessStatusCode)
        {
            await HandleErrors(response);
        }

        var tResponse = await response.Content.ReadFromJsonAsync<TResponse>();
        return tResponse;
    }

    public async Task Post<TRequest>(string url, TRequest tRequest)
    {
        await AddHeaderValues();
        var response = await client.PostAsJsonAsync($"{_baseUrl}{url}", tRequest);

        if (!response.IsSuccessStatusCode)
        {
            await HandleErrors(response);
        }
    }

    public async Task<TResponse> Post<TRequest, TResponse>(string url, TRequest tRequest)
    {
        await AddHeaderValues();
        var response = await client.PostAsJsonAsync($"{_baseUrl}{url}", tRequest);

        if (!response.IsSuccessStatusCode)
        {
            await HandleErrors(response);
        }

        var tResponse = await response.Content.ReadFromJsonAsync<TResponse>();
        return tResponse;
    }

    public async Task<HttpResponseMessage> PostReturnHttpResponseMessage<TRequest>(string url, TRequest tRequest)
    {
        await AddHeaderValues();

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}{url}");
        request.Content = JsonContent.Create(tRequest);

        request.SetBrowserResponseStreamingEnabled(true);

        var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        if (!response.IsSuccessStatusCode)
        {
            await HandleErrors(response);
        }

        return response;
    }

    public async Task Put<TRequest>(string url, TRequest tRequest)
    {
        await AddHeaderValues();
        var response = await client.PutAsJsonAsync($"{_baseUrl}{url}", tRequest);

        if (!response.IsSuccessStatusCode)
        {
            await HandleErrors(response);
        }
    }

    public async Task Put(string url)
    {
        await AddHeaderValues();
        var response = await client.PutAsync($"{_baseUrl}{url}", null);

        if (!response.IsSuccessStatusCode)
        {
            await HandleErrors(response);
        }
    }

    public async Task Delete(string url)
    {
        await AddHeaderValues();
        var response = await client.DeleteAsync($"{_baseUrl}{url}");

        if (!response.IsSuccessStatusCode)
        {
            await HandleErrors(response);
        }
    }

    private async Task AddHeaderValues()
    {
        var token = await jwtProvider.GetJwtDto();
        client.DefaultRequestHeaders.Authorization =
            token is not null ? new AuthenticationHeaderValue(AuthScheme, token.AccessToken) : null;

        var currentCultureName = CultureInfo.CurrentCulture.Name;
        client.DefaultRequestHeaders.Add(LanguageHeader, currentCultureName);
    }

    private static async Task HandleErrors(HttpResponseMessage response)
    {
        var error = await response.Content.ReadAsStringAsync();
        var tResponse = JsonConvert.DeserializeObject<ErrorResponse>(error);

        switch (tResponse.Type)
        {
            case ExceptionType.Validation:
                var validationErrorResponse = JsonConvert.DeserializeObject<ValidationErrorResponse>(error);
                throw new MyValidationException(validationErrorResponse.Errors);
            case ExceptionType.Unauthorized:
                JsonConvert.DeserializeObject<UnauthorizedErrorResponse>(error);
                throw new MyUnauthorizedException();
            case ExceptionType.Forbidden:
                JsonConvert.DeserializeObject<ForbiddenErrorResponse>(error);
                throw new MyForbiddenException();
            case ExceptionType.NotFound:
                JsonConvert.DeserializeObject<NotFoundErrorResponse>(error);
                throw new MyNotFoundException();
            case ExceptionType.Server:
            default:
                JsonConvert.DeserializeObject<ServerErrorResponse>(error);
                throw new MyServerException();
        }
    }
}