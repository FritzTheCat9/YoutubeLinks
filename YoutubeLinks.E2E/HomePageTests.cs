using static YoutubeLinks.Blazor.Layout.MainLayout;

namespace YoutubeLinks.E2E
{
    [TestFixture]
    public class HomePageTests : PageTestBase
    {
        [Test]
        public async Task HamburgerMenu()
        {

        }

        [Test]
        public async Task SwitchTheme()
        {

        }

        [Test]
        public async Task CheckProjectOnGithub()
        {
            var projectUrl = "https://github.com/FritzTheCat9/YoutubeLinks";

            await NavigateToPage();

            var popup = await Page.RunAndWaitForPopupAsync(async () =>
            {
                await ClickElement(MainLayoutConst.RedirectToProjectGithubPageButton);
            });

            await Expect(popup).ToHaveURLAsync(projectUrl);
        }
    }
}