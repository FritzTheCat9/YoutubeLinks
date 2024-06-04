using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using static YoutubeLinks.Blazor.Pages.Links.CreateLinkForm;
using static YoutubeLinks.Blazor.Pages.Links.LinksPage;
using static YoutubeLinks.Blazor.Pages.Links.UpdateLinkDialog;
using static YoutubeLinks.Blazor.Pages.Playlists.CreatePlaylistDialog;
using static YoutubeLinks.Blazor.Pages.Playlists.PlaylistsPage;
using static YoutubeLinks.Blazor.Pages.Playlists.UpdatePlaylistDialog;
using static YoutubeLinks.Blazor.Pages.Users.Auth;
using static YoutubeLinks.Blazor.Pages.Users.LoginDialog;
using static YoutubeLinks.Blazor.Pages.Users.UsersPage;
using static YoutubeLinks.Blazor.Shared.DeleteDialog;

namespace YoutubeLinks.E2E
{
    public class PageTestBase : PageTest
    {
        protected const string BaseUrl = "http://localhost:7000";

        public override BrowserNewContextOptions ContextOptions()
        {
            return new BrowserNewContextOptions
            {
                //ViewportSize = new()
                //{
                //    Width = 1920,
                //    Height = 1080
                //},
                Permissions = ["clipboard-read", "clipboard-write"]
            };
        }

        protected ILocator GetLocatorByTestId(string testId)
            => Page.GetByTestId(testId).First;

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

        protected static async Task SaveDownloadedFile(IDownload download)
        {
            var testProjectDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..");
            var tempFolderPath = Path.Combine(testProjectDirectoryPath, "Temp");
            var savePath = Path.Combine(tempFolderPath, download.SuggestedFilename);

            if (!Directory.Exists(tempFolderPath))
                Directory.CreateDirectory(tempFolderPath);

            await download.SaveAsAsync(savePath);
        }

        #region Api

        protected const string ApiBaseUrl = "http://localhost:5000/api";

        protected Task<IResponse> WaitForApiResponse(string url)
            => Page.WaitForResponseAsync(response => response.Url.Contains($"{ApiBaseUrl}/{url}"));

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

        #endregion

        #region Auth

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

        protected async Task Logout()
        {
            await ClickElement(AuthComponentConst.LogoutButton);

            await Expect(GetLocatorByTestId(AuthComponentConst.UserNameText)).ToBeHiddenAsync();
            await Expect(GetLocatorByTestId(AuthComponentConst.LogoutButton)).ToBeHiddenAsync();

            await Expect(GetLocatorByTestId(AuthComponentConst.LoginButton)).ToBeVisibleAsync();
            await Expect(GetLocatorByTestId(AuthComponentConst.RegisterButton)).ToBeVisibleAsync();
        }

        #endregion

        #region Playlists

        protected async Task NavigateToAdminPlaylists()
        {
            await LoginAsAdmin();

            await NavigateToPage("users");
            await FillInput(UsersPageConst.SearchInput, AdminData.UserName);
            await ClickEnter(UsersPageConst.SearchInput);
            await Expect(GetLocatorByTestId(UsersPageConst.UserNameTableRowData)).ToHaveTextAsync(AdminData.UserName);

            await ClickElement(UsersPageConst.NavigateToUserPlaylistsButton);
        }

        protected async Task GoBackToPlaylistsPageFromLinksPage()
        {
            await ClickElement("Playlists");
        }

        protected async Task SearchPlaylist(string name)
        {
            await FillInput(PlaylistsPageConst.SearchInput, name);
            await ClickEnter(PlaylistsPageConst.SearchInput);
        }

        protected async Task CreateTestPlaylist(string playlistName, bool playlistIsPublic)
        {
            await ClickElement(PlaylistsPageConst.CreatePlaylistButton);
            await FillInput(CreatePlaylistDialogConst.NameInput, playlistName);
            await ClickElement(CreatePlaylistDialogConst.PublicCheckbox);
            await ClickElement(CreatePlaylistDialogConst.CreatePlaylistButton);
            await CheckPlaylistsTableRowIsValid(playlistName, playlistIsPublic);
        }

        protected async Task UpdateTestPlaylist(string playlistName, string playlistNameUpdated, bool playlistIsPublicUpdated)
        {
            await SearchPlaylist(playlistName);
            await ClickElement(PlaylistsPageConst.UpdatePlaylistButton);
            await FillInput(UpdatePlaylistDialogConst.NameInput, playlistNameUpdated);
            await ClickElement(UpdatePlaylistDialogConst.PublicCheckbox);
            await ClickElement(UpdatePlaylistDialogConst.UpdatePlaylistButton);
            await CheckPlaylistsTableRowIsValid(playlistNameUpdated, playlistIsPublicUpdated);
        }

        protected async Task CheckPlaylistsTableRowIsValid(string name, bool isPublic)
        {
            await SearchPlaylist(name);
            await Expect(GetLocatorByTestId(PlaylistsPageConst.NameTableRowData)).ToHaveTextAsync(name);
        }

        protected async Task DeleteTestPlaylist(string playlistName)
        {
            await SearchPlaylist(playlistName);
            await ClickElement(PlaylistsPageConst.DeletePlaylistButton);
            await ClickElement(DeleteDialogConst.DeleteButton);
            await CheckPlaylistsTableRowIsHidden(playlistName);
        }

        protected async Task CheckPlaylistsTableRowIsHidden(string name)
        {
            await SearchPlaylist(name);
            await Expect(GetLocatorByTestId(PlaylistsPageConst.NameTableRowData)).ToBeHiddenAsync();
        }

        #endregion

        #region Links

        protected async Task SearchLink(string name)
        {
            await FillInput(LinksPageConst.SearchInput, name);
            await ClickEnter(LinksPageConst.SearchInput);
        }

        protected async Task CreateLink(string title, string url)
        {
            await FillInput(CreateLinkFormConst.UrlInput, url);
            await ApiResponseOkAfterButtonClick(CreateLinkFormConst.CreateButton, "links");
            await CheckLinksTableRowIsValid(title);
        }

        protected async Task UpdateLink(string title, string updatedUrl, string updatedTitle, bool clickDownloadedCheckbox)
        {
            await SearchLink(title);
            await ClickElement(LinksPageConst.UpdateLinkButton);
            await FillInput(UpdateLinkDialogConst.UrlInput, updatedUrl);
            await FillInput(UpdateLinkDialogConst.TitleInput, updatedTitle);
            if (clickDownloadedCheckbox)
                await ClickElement(UpdateLinkDialogConst.DownloadedCheckbox);
            await ApiResponseOkAfterButtonClick(UpdateLinkDialogConst.UpdateButton, "links");
            await CheckLinksTableRowIsValid(updatedTitle);
        }

        protected async Task DeleteLink(string title)
        {
            await SearchLink(title);
            await ClickElement(LinksPageConst.DeleteLinkButton);
            await ApiResponseOkAfterButtonClick(DeleteDialogConst.DeleteButton, "links");
            await CheckLinksTableRowIsHidden(title);
        }

        protected async Task CheckLinksTableRowIsValid(string title)
        {
            await SearchLink(title);
            await Expect(GetLocatorByTestId(LinksPageConst.TitleTableRowData)).ToHaveTextAsync(title);
        }

        protected async Task CheckLinksTableRowIsHidden(string title)
        {
            await SearchLink(title);
            await Expect(GetLocatorByTestId(LinksPageConst.TitleTableRowData)).ToBeHiddenAsync();
        }

        #endregion
    }
}
