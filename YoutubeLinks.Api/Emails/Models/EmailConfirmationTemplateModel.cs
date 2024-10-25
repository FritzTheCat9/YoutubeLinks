namespace YoutubeLinks.Api.Emails.Models;

public class EmailConfirmationTemplateModel : BaseTemplateModel
{
    public EmailConfirmationTemplateModel()
    {
        Subject = "Email confirmation";
        TemplateFileName = "EmailConfirmationTemplate.cshtml";
    }

    public string UserName { get; init; }
    public string Link { get; init; }
}