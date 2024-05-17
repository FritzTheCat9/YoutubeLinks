using Microsoft.Playwright.NUnit;
using static YoutubeLinks.Blazor.Pages.Users.Auth;
using static YoutubeLinks.Blazor.Pages.Users.LoginDialog;

namespace YoutubeLinks.E2E
{
    public class PageTestBase : PageTest
    {
        protected const string BaseUrl = "http://localhost:7000";

        protected class AdminData
        {
            public const string UserName = "Admin";
            public const string Email = "ytlinksapp@gmail.com";
            public const string Password = "AQAAAAIAAYagAAAAECWFTp9uY78qPzaRu0d3uaJNo3WOlRpwCuCyDLH+yg/TowsjzlMGxMurTnvyZaYSxA==";
        }

        protected class UserData
        {
            public const string UserName = "User";
            public const string Email = "ytlinksapp1@gmail.com";
            public const string Password = "AQAAAAIAAYagAAAAECWFTp9uY78qPzaRu0d3uaJNo3WOlRpwCuCyDLH+yg/TowsjzlMGxMurTnvyZaYSxA==";
        }

        protected async Task FillInput(string testId, string text)
            => await Page.GetByTestId(testId).FillAsync(text);

        protected async Task ClickButton(string testId)
            => await Page.GetByTestId(testId).ClickAsync();

        protected async Task NavigateToPage(string url)
            => await Page.GotoAsync($"{BaseUrl}{url}");

        protected async Task CheckText(string testId, string text)
        {
            var element = Page.GetByTestId(testId);
            await Expect(element).ToHaveTextAsync(text);
        }

        protected async Task LoginAsUser()
        {
            await NavigateToPage("/");
            await ClickButton(AuthComponentConst.LoginButton);

            await FillInput(LoginDialogConst.EmailInput, UserData.Email);
            await FillInput(LoginDialogConst.PasswordInput, UserData.Password);
            await ClickButton(LoginDialogConst.LoginButton);

            //await CheckText(AuthComponentConst.UserNameText, UserData.UserName);
        }

        protected async Task LoginAsAdmin()
        {
            await NavigateToPage("/");
            await ClickButton(AuthComponentConst.LoginButton);

            await FillInput(LoginDialogConst.EmailInput, AdminData.Email);
            await FillInput(LoginDialogConst.PasswordInput, AdminData.Password);
            await ClickButton(LoginDialogConst.LoginButton);

            //await CheckText(AuthComponentConst.UserNameText, AdminData.UserName);
        }
    }
}
