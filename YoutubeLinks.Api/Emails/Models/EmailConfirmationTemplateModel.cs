namespace YoutubeLinks.Api.Emails.Models
{
    public class EmailConfirmationTemplateModel : BaseTemplateModel
    {
        public string UserName { get; init; }
        public string Link { get; init; }

        public EmailConfirmationTemplateModel()
        {
            Subject = "Email confirmation";
            TemplateFileName = "EmailConfirmationTemplate.cshtml";
        }
    }
}
