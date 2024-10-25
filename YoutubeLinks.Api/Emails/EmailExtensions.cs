using YoutubeLinks.Shared.Extensions;

namespace YoutubeLinks.Api.Emails;

public static class EmailExtensions
{
    private const string SectionName = "Email";

    public static IServiceCollection AddEmails(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<EmailOptions>(configuration.GetRequiredSection(SectionName));
        var emailOptions = configuration.GetOptions<EmailOptions>(SectionName);

        services.AddFluentEmail(emailOptions.Email)
            .AddRazorRenderer()
            .AddSmtpSender(emailOptions.SmtpHost,
                emailOptions.Port,
                emailOptions.Email,
                emailOptions.Password);

        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}