using YoutubeLinks.Shared.Extensions;

namespace YoutubeLinks.Api.Emails
{
    public static class EmailExtensions
    {
        private const string _sectionName = "Email";

        public static IServiceCollection AddEmails(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<EmailOptions>(configuration.GetRequiredSection(_sectionName));
            var emailOptions = configuration.GetOptions<EmailOptions>(_sectionName);

            services.AddFluentEmail(emailOptions.Email)
                    .AddRazorRenderer()
                    .AddSmtpSender(emailOptions.SMTPHost,
                                   emailOptions.Port,
                                   emailOptions.Email,
                                   emailOptions.Password);

            services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}
