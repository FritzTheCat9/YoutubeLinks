namespace YoutubeLinks.Api.Emails.Models
{
    public class ResetPasswordSuccessTemplateModel : BaseTemplateModel
    {
        public string UserName { get; init; }

        public ResetPasswordSuccessTemplateModel()
        {
            Subject = "Reset password";
            TemplateFileName = "ResetPasswordSuccessTemplate.cshtml";
        }
    }
}
