using static YoutubeLinks.Blazor.Layout.MainLayout;

namespace YoutubeLinks.E2E;

[TestFixture]
public class HomePageTests : PageTestBase
{
    [Test]
    public async Task HamburgerMenu()
    {
        await NavigateToPage();
        await ClickElement(MainLayoutConst.ToggleNavMenuButton);
        await ClickElement(MainLayoutConst.ToggleNavMenuButton);
    }

    [Test]
    public async Task SwitchTheme()
    {
        await NavigateToPage();
        await ClickElement(MainLayoutConst.ChangeThemeButton);
        await ClickElement(MainLayoutConst.ChangeThemeButton);
        await ClickElement(MainLayoutConst.ChangeThemeButton);
    }

    [Test]
    public async Task CheckProjectOnGithub()
    {
        const string projectUrl = "https://github.com/FritzTheCat9/YoutubeLinks";

        await NavigateToPage();

        var popup = await Page.RunAndWaitForPopupAsync(async () =>
        {
            await ClickElement(MainLayoutConst.RedirectToProjectGithubPageButton);
        });

        await Expect(popup).ToHaveURLAsync(projectUrl);
    }
}