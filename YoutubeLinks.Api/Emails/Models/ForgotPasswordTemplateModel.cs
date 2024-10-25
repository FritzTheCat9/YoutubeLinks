namespace YoutubeLinks.Api.Emails.Models;

public class ForgotPasswordTemplateModel : BaseTemplateModel
{
    public ForgotPasswordTemplateModel()
    {
        Subject = "Forgot password";
        TemplateFileName = "ForgotPasswordEmailTemplate.cshtml";
    }

    public string UserName { get; init; }
    public string Link { get; init; }
}