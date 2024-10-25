using System.Reflection;
using FluentValidation;
using YoutubeLinks.Api.Behaviors;
using YoutubeLinks.Shared.Features.Users.Commands;

namespace YoutubeLinks.Api.Extensions;

public static class MediatRExtensions
{
    public static IServiceCollection AddMediatr(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(UnitOfWorkBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(Register.Validator).Assembly);
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        ValidatorOptions.Global.LanguageManager.Enabled = false;

        return services;
    }
}