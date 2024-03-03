using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using YoutubeLinks.Blazor;
using YoutubeLinks.Blazor.Auth;
using YoutubeLinks.Blazor.Clients;
using YoutubeLinks.Blazor.Exceptions;
using YoutubeLinks.Blazor.Extensions;
using YoutubeLinks.Blazor.Localization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddApiClients(builder.Configuration)
                .AddFluentValidation()
                .AddExceptions()
                .AddAuth()
                .AddMyLocalization();

builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();

var host = builder.Build();

await host.UseMyLocalization();

await host.RunAsync();
