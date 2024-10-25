namespace YoutubeLinks.Api.Emails.Models
{
    public class EmailConfirmationSuccessfulTemplateModel : BaseTemplateModel
    {
        public string UserName { get; init; }

        public EmailConfirmationSuccessfulTemplateModel()
        {
            Subject = "Email confirmation successful";
            TemplateFileName = "EmailConfirmationSuccessfulTemplate.cshtml";
        }
    }
}
