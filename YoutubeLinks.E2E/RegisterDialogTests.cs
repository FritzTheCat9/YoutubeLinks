using static YoutubeLinks.Blazor.Pages.Users.Auth;
using static YoutubeLinks.Blazor.Pages.Users.LoginDialog;
using static YoutubeLinks.Blazor.Pages.Users.RegisterDialog;

namespace YoutubeLinks.E2E;

[TestFixture]
public class RegisterDialogTests : PageTestBase
{
    [Test]
    public async Task Register()
    {
        await NavigateToPage();
        await ClickElement(AuthComponentConst.RegisterButton);

        await FillInput(RegisterDialogConst.EmailInput, RegisterUserData.Email);
        await FillInput(RegisterDialogConst.UserNameInput, RegisterUserData.UserName);
        await FillInput(RegisterDialogConst.PasswordInput, RegisterUserData.Password);
        await FillInput(RegisterDialogConst.RepeatPasswordInput, RegisterUserData.Password);

        await ApiResponseOkAfterButtonClick(RegisterDialogConst.RegisterButton, "users/register");
    }

    /// <summary>
    ///     To login, we should confirm registered user email
    /// </summary>
    [Test]
    public async Task LoginAsRegisteredUser()
    {
        await NavigateToPage();
        await ClickElement(AuthComponentConst.LoginButton);

        await FillInput(LoginDialogConst.EmailInput, RegisterUserData.Email);
        await FillInput(LoginDialogConst.PasswordInput, RegisterUserData.Password);

        await ApiResponseOkAfterButtonClick(LoginDialogConst.LoginButton, "users/login");

        await CheckText(AuthComponentConst.UserNameText, RegisterUserData.UserName);
    }
}