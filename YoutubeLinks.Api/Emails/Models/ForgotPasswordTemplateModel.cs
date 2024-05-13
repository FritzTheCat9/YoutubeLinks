namespace YoutubeLinks.Api.Emails.Models
{
    public class ForgotPasswordTemplateModel : BaseTemplateModel
    {
        public string UserName { get; set; }
        public string Link { get; set; }

        public ForgotPasswordTemplateModel()
        {
            Subject = "Forgot password";
            TemplateFileName = "ForgotPasswordEmailTemplate.cshtml";
        }
    }
}
