using YoutubeLinks.Api.Data.Database;
using YoutubeLinks.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCORS(builder.Configuration)
                .AddDatabase(builder.Configuration);

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

app.UseCORS();

app.Run();
