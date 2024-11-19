using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using YoutubeLinks.Blazor;
using YoutubeLinks.Blazor.Auth;
using YoutubeLinks.Shared.Clients;
using YoutubeLinks.Blazor.Exceptions;
using YoutubeLinks.Blazor.Extensions;
using YoutubeLinks.Blazor.Localization;
using YoutubeLinks.Blazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddApiClients(builder.Configuration)
    .AddFluentValidation()
    .AddExceptions()
    .AddAuth()
    .AddMyLocalization()
    .AddServices();

builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();

builder.Logging.AddFilter("Microsoft.AspNetCore.Authorization.*", LogLevel.None);

var host = builder.Build();

await host.UseMyLocalization();

await host.RunAsync();