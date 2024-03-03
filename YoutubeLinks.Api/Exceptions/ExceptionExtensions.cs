namespace YoutubeLinks.Api.Exceptions
{
    public static class ExceptionExtensions
    {
        public static IServiceCollection AddExceptionMiddleware(this IServiceCollection services)
        {
            services.AddSingleton<ExceptionMiddleware>();

            return services;
        }

        public static WebApplication UseExceptionMiddleware(this WebApplication app)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            return app;
        }
    }
}
