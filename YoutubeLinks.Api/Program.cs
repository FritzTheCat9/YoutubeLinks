using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Database;
using YoutubeLinks.Api.Emails;
using YoutubeLinks.Api.Exceptions;
using YoutubeLinks.Api.Extensions;
using YoutubeLinks.Api.Localization;
using YoutubeLinks.Api.Logging;
using YoutubeLinks.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwagger()
                .AddCORS(builder.Configuration)
                .AddMediatr()
                .AddDatabase(builder.Configuration)
                .AddAuth(builder.Configuration)
                .AddServices(builder.Environment)
                .AddEmails(builder.Configuration)
                .AddExceptionMiddleware()
                .AddMyLocalization()
                .AddLogging(builder.Configuration)
                .AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "youtubelinks.api");
        options.RoutePrefix = string.Empty;
    });
}

app.UseMyLocalization()
   .UseExceptionMiddleware()
   .AddEndpoints()
   .UseCORS()
   .UseAuth();

app.Run();
