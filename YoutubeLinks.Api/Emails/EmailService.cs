using FluentEmail.Core;
using Microsoft.Extensions.Options;

namespace YoutubeLinks.Api.Emails;

public interface IEmailService
{
    Task SendEmail<T>(string to, T model) where T : BaseTemplateModel;
}

public class EmailService(
    IOptions<EmailOptions> options,
    IWebHostEnvironment webHostEnvironment,
    IFluentEmail fluentEmail,
    ILogger<EmailService> logger)
    : IEmailService
{
    private readonly EmailOptions _options = options.Value;
    private readonly string _templatesFolder = Path.Combine(Path.GetFullPath(webHostEnvironment.ContentRootPath), "Emails", "Templates");

    public async Task SendEmail<T>(string to, T model) where T : BaseTemplateModel
    {
        if (!_options.SendEmails)
        {
            logger.LogError("[Email Service] Sending emails is disabled in appsettings.json {SendEmails}",
                _options.SendEmails);

            return;
        }

        var email = fluentEmail.SetFrom(_options.Email)
            .To(to)
            .Subject(model.Subject)
            .UsingTemplateFromFile(Path.Combine(_templatesFolder, model.TemplateFileName), model);

        await email.SendAsync();

        logger.LogInformation("[Email Service] {Subject} email successfully sent to: {To}, ({TemplateFileName})",
            model.Subject, to, model.TemplateFileName);
    }
}

public class BaseTemplateModel
{
    public string Subject { get; protected init; }
    public string TemplateFileName { get; protected init; }
}