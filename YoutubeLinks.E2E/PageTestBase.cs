using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using static YoutubeLinks.Blazor.Pages.Users.Auth;
using static YoutubeLinks.Blazor.Pages.Users.LoginDialog;

namespace YoutubeLinks.E2E
{
    public class PageTestBase : PageTest
    {
        protected const string BaseUrl = "http://localhost:7000";
        protected const string ApiBaseUrl = "http://localhost:5000/api";

        protected class AdminData
        {
            public const string UserName = "Admin";
            public const string Email = "ytlinksapp@gmail.com";
            public const string Password = "Asd123!";
        }

        protected class UserData
        {
            public const string UserName = "User";
            public const string Email = "ytlinksapp1@gmail.com";
            public const string Password = "Asd123!";
        }

        public override BrowserNewContextOptions ContextOptions()
        {
            return new BrowserNewContextOptions
            {
                ViewportSize = new()
                {
                    Width = 1920,
                    Height = 1080
                },
            };
        }

        protected async Task FillInput(string testId, string text)
            => await Page.GetByTestId(testId).FillAsync(text);

        protected async Task ClickElement(string testId)
            => await Page.GetByTestId(testId).First.ClickAsync();

        protected async Task ClickEnter(string testId)
            => await Page.GetByTestId(testId).PressAsync("Enter");

        protected async Task NavigateToPage(string url = "")
            => await Page.GotoAsync($"{BaseUrl}/{url}");

        protected async Task CheckText(string testId, string text)
        {
            var element = Page.GetByTestId(testId);
            await Expect(element).ToHaveTextAsync(text);
        }

        protected Task<IResponse> WaitForApiResponse(string url)
            => Page.WaitForResponseAsync(response => response.Url == $"{ApiBaseUrl}/{url}");

        protected async Task ApiResponseOkAfterButtonClick(string testId, string url)
        {
            var responseTask = WaitForApiResponse(url);
            await ClickElement(testId);
            var response = await responseTask;
            Assert.That(response.Ok);
        }

        protected async Task ApiResponseOkAfterEnterClick(string testId, string url)
        {
            var responseTask = WaitForApiResponse(url);
            await ClickEnter(testId);
            var response = await responseTask;
            Assert.That(response.Ok);
        }

        protected async Task LoginAsUser()
        {
            await NavigateToPage();
            await ClickElement(AuthComponentConst.LoginButton);

            await FillInput(LoginDialogConst.EmailInput, UserData.Email);
            await FillInput(LoginDialogConst.PasswordInput, UserData.Password);

            await ApiResponseOkAfterButtonClick(LoginDialogConst.LoginButton, "users/login");

            await CheckText(AuthComponentConst.UserNameText, UserData.UserName);
        }

        protected async Task LoginAsAdmin()
        {
            await NavigateToPage();
            await ClickElement(AuthComponentConst.LoginButton);

            await FillInput(LoginDialogConst.EmailInput, AdminData.Email);
            await FillInput(LoginDialogConst.PasswordInput, AdminData.Password);

            await ApiResponseOkAfterButtonClick(LoginDialogConst.LoginButton, "users/login");

            await CheckText(AuthComponentConst.UserNameText, AdminData.UserName);
        }

        protected ILocator GetLocatorByTestId(string testId) 
            => Page.GetByTestId(testId).First;
    }
}
