namespace YoutubeLinks.Api.Emails.Models
{
    public class EmailConfirmationSuccessfulTemplateModel : BaseTemplateModel
    {
        public string UserName { get; set; }

        public EmailConfirmationSuccessfulTemplateModel()
        {
            Subject = "Email confirmation successful";
            TemplateFileName = "EmailConfirmationSuccessfulTemplate.cshtml";
        }
    }
}
