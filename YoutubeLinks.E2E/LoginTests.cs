using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;
using MudBlazor;

namespace YoutubeLinks.E2E
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class LoginTests : PageTest
    {
        [Test]
        public async Task LoginDialogCanLogInUser()
        {
            await Page.GotoAsync("http://localhost:7000");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();

            await Page.GetByTestId("login-email-input").FillAsync("ytlinksapp@gmail.com");
            await Page.GetByTestId("login-password-input").FillAsync("Asd123!");
            await Page.GetByTestId("login-button").ClickAsync();

            // check if user is logged in
        }
    }
}