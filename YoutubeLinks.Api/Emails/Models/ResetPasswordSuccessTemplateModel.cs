namespace YoutubeLinks.Api.Emails.Models;

public class ResetPasswordSuccessTemplateModel : BaseTemplateModel
{
    public ResetPasswordSuccessTemplateModel()
    {
        Subject = "Reset password";
        TemplateFileName = "ResetPasswordSuccessTemplate.cshtml";
    }

    public string UserName { get; init; }
}