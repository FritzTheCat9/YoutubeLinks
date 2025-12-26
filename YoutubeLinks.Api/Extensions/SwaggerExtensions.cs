using Microsoft.OpenApi;

namespace YoutubeLinks.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        const string bearer = "Bearer";

        services.AddSwaggerGen(setup =>
        {
            setup.AddSecurityDefinition(bearer, new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = bearer
            });

            setup.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(bearer, hostDocument: document)] = []
            });

            // Fix for swagger bug for endpoints with name containing '+': (https://github.com/swagger-api/swagger-ui/issues/7911)
            setup.CustomSchemaIds(s => s.FullName?.Replace("+", "."));
        });

        return services;
    }
}
