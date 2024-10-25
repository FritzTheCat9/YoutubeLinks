namespace YoutubeLinks.Api.Emails.Models
{
    public class ForgotPasswordTemplateModel : BaseTemplateModel
    {
        public string UserName { get; init; }
        public string Link { get; init; }

        public ForgotPasswordTemplateModel()
        {
            Subject = "Forgot password";
            TemplateFileName = "ForgotPasswordEmailTemplate.cshtml";
        }
    }
}
