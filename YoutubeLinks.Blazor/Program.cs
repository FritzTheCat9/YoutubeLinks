using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Serilog;
using YoutubeLinks.Blazor;
using YoutubeLinks.Blazor.Auth;
using YoutubeLinks.Blazor.Clients;
using YoutubeLinks.Blazor.Exceptions;
using YoutubeLinks.Blazor.Extensions;
using YoutubeLinks.Blazor.Localization;
using YoutubeLinks.Blazor.Logging;
using YoutubeLinks.Shared.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);



var sectionNameLog = "Log";
var sectionNameApi = "Api";
var logTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message}{NewLine}{Exception}";

builder.Services.Configure<LogOptions>(builder.Configuration.GetRequiredSection(sectionNameLog));
var logOptions = builder.Configuration.GetOptions<LogOptions>(sectionNameLog);

builder.Services.Configure<ApiOptions>(builder.Configuration.GetRequiredSection(sectionNameApi));
var apiOptions = builder.Configuration.GetOptions<ApiOptions>(sectionNameApi);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: logTemplate)
    .WriteTo.Http(apiOptions.Url + "/api/log", null)
    .WriteTo.BrowserConsole(outputTemplate: logTemplate)
    .WriteTo.File(logOptions.FilePath, rollingInterval: RollingInterval.Day, outputTemplate: logTemplate)
    .MinimumLevel.Information()
    .CreateLogger();

builder.Logging.AddSerilog();




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
