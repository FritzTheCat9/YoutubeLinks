namespace YoutubeLinks.Api.Emails.Models;

public class EmailConfirmationSuccessfulTemplateModel : BaseTemplateModel
{
    public EmailConfirmationSuccessfulTemplateModel()
    {
        Subject = "Email confirmation successful";
        TemplateFileName = "EmailConfirmationSuccessfulTemplate.cshtml";
    }

    public string UserName { get; init; }
}