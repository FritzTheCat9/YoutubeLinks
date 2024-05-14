namespace YoutubeLinks.Api.Emails.Models
{
    public class ResetPasswordSuccessTemplateModel : BaseTemplateModel
    {
        public string UserName { get; set; }

        public ResetPasswordSuccessTemplateModel()
        {
            Subject = "Reset password";
            TemplateFileName = "ResetPasswordSuccessTemplate.cshtml";
        }
    }
}
