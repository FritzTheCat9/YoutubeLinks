using static YoutubeLinks.Blazor.Pages.Users.UsersPage;

namespace YoutubeLinks.E2E
{
    [TestFixture]
    public class UsersPageTests : PageTestBase
    {
        [Test]
        public async Task Filter()
        {
            await NavigateToPage("users");

            await FillInput(UsersPageConst.SearchInput, AdminData.UserName);
            await ClickEnter(UsersPageConst.SearchInput);
            await Expect(Page.GetByTestId(UsersPageConst.UserNameTableRowData)).ToHaveTextAsync(AdminData.UserName);

            await FillInput(UsersPageConst.SearchInput, UserData.UserName);
            await ClickEnter(UsersPageConst.SearchInput);
            await Expect(GetLocatorByTestId(UsersPageConst.UserNameTableRowData)).ToHaveTextAsync(UserData.UserName);
        }

        [Test]
        public async Task SortByUserName()
        {
            await NavigateToPage("users");
            await ClickButton(UsersPageConst.UserNameTableSortLabel);
            await ClickButton(UsersPageConst.UserNameTableSortLabel);
            await ClickButton(UsersPageConst.UserNameTableSortLabel);
        }

        [Test]
        public async Task SortByEmail()
        {
            await NavigateToPage("users");
            await ClickButton(UsersPageConst.EmailTableSortLabel);
            await ClickButton(UsersPageConst.EmailTableSortLabel);
            await ClickButton(UsersPageConst.EmailTableSortLabel);
        }

        [Test]
        public async Task NavigateToPlaylistsView()
        {
            await NavigateToPage("users");
            await ClickButton(UsersPageConst.NavigateToUserPlaylistsButton);
        }
    }
}
