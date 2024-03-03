namespace YoutubeLinks.Blazor.Exceptions
{
    public static class ExceptionsExtensions
    {
        public static IServiceCollection AddExceptions(this IServiceCollection services)
        {
            services.AddScoped<IExceptionHandler, ExceptionHandler>();
            services.AddSingleton<ValidationErrors>();

            return services;
        }
    }
}
