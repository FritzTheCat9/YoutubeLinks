using FluentEmail.Core;
using Microsoft.Extensions.Options;

namespace YoutubeLinks.Api.Emails
{
    public interface IEmailService
    {
        Task SendEmail<T>(string to, T model) where T : BaseTemplateModel;
    }

    public class EmailService : IEmailService
    {
        private readonly EmailOptions _options;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFluentEmail _fluentEmail;
        private readonly ILogger<EmailService> _logger;
        private readonly string _templatesFolder;

        public EmailService(
            IOptions<EmailOptions> options,
            IWebHostEnvironment webHostEnvironment,
            IFluentEmail fluentEmail,
            ILogger<EmailService> logger)
        {
            _options = options.Value;
            _webHostEnvironment = webHostEnvironment;
            _fluentEmail = fluentEmail;
            _logger = logger;
            _templatesFolder = $"{_webHostEnvironment.ContentRootPath}\\Emails\\Templates";
        }

        public async Task SendEmail<T>(string to, T model) where T : BaseTemplateModel
        {
            if (!_options.SendEmails)
            {
                _logger.LogError("[Email Service] Sending emails is disabled in appsettings.json {SendEmails}", _options.SendEmails);

                return;
            }

            var email = _fluentEmail.SetFrom(_options.Email)
                                    .To(to)
                                    .Subject(model.Subject)
                                    .UsingTemplateFromFile($"{_templatesFolder}//{model.TemplateFileName}", model);

            await email.SendAsync();

            _logger.LogInformation("[Email Service] {Subject} email successfully sent to: {To}, ({TemplateFileName})", model.Subject, to, model.TemplateFileName);
        }
    }

    public class EmailData<TemplateModel>
    {
        public string To { get; set; }
        public TemplateModel Model { get; set; }
    }

    public class BaseTemplateModel
    {
        public string Subject { get; set; }
        public string TemplateFileName { get; set; }
    }
}
