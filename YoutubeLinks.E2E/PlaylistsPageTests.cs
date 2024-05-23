using static YoutubeLinks.Blazor.Pages.Playlists.CreatePlaylistDialog;
using static YoutubeLinks.Blazor.Pages.Playlists.PlaylistsPage;
using static YoutubeLinks.Blazor.Pages.Playlists.UpdatePlaylistDialog;
using static YoutubeLinks.Blazor.Pages.Users.UsersPage;
using static YoutubeLinks.Blazor.Shared.DeleteDialog;

namespace YoutubeLinks.E2E
{
    [TestFixture]
    public class PlaylistsPageTests : PageTestBase
    {
        [SetUp]
        public async Task SetUp()
        {
            await LoginAsAdmin();

            await NavigateToPage("users");
            await FillInput(UsersPageConst.SearchInput, AdminData.UserName);
            await ClickEnter(UsersPageConst.SearchInput);
            await Expect(GetLocatorByTestId(UsersPageConst.UserNameTableRowData)).ToHaveTextAsync(AdminData.UserName);

            await ClickElement(UsersPageConst.NavigateToUserPlaylistsButton);
        }

        [Test]
        public async Task SortByName()
        {
            await ClickElement(PlaylistsPageConst.NameTableSortLabel);
            await ClickElement(PlaylistsPageConst.NameTableSortLabel);
            await ClickElement(PlaylistsPageConst.NameTableSortLabel);
        }

        [Test]
        public async Task CreateUpdateDeletePlaylist()
        {
            var playlistName = "Test Playlist";
            var playlistIsPublic = false;

            await CreateTestPlaylist(playlistName, playlistIsPublic);

            var playlistNameUpdated = "Test Playlist - Updated";
            var playlistIsPublicUpdated = true;

            await UpdateTestPlaylist(playlistName, playlistNameUpdated, playlistIsPublicUpdated);

            await DeleteTestPlaylist(playlistNameUpdated);
        }

        private async Task SearchPlaylist(string name)
        {
            await FillInput(PlaylistsPageConst.SearchInput, name);
            await ClickEnter(PlaylistsPageConst.SearchInput);
        }

        [Test]
        public async Task ExportPlaylist()
        {

        }

        [Test]
        public async Task ImportPlaylist()
        {

        }

        [Test]
        public async Task DownloadPlaylistMP3()
        {

        }

        [Test]
        public async Task DownloadPlaylistMP4()
        {

        }

        [Test]
        public async Task NavigateToPlaylistLinksView()
        {
            var playlistName = "Test Playlist";
            var playlistIsPublic = false;

            await CreateTestPlaylist(playlistName, playlistIsPublic);

            await SearchPlaylist(playlistName);
            await ClickElement(PlaylistsPageConst.NavigateToPlaylistLinksButton);
            await GoBackToPlaylistsPageFromLinksPage();

            await DeleteTestPlaylist(playlistName);
        }

        private async Task GoBackToPlaylistsPageFromLinksPage()
        {
            await ClickElement("Playlists");
        }

        private async Task CreateTestPlaylist(string playlistName, bool playlistIsPublic)
        {
            await ClickElement(PlaylistsPageConst.CreatePlaylistButton);
            await FillInput(CreatePlaylistDialogConst.NameInput, playlistName);
            await ClickElement(CreatePlaylistDialogConst.PublicCheckbox);
            await ClickElement(CreatePlaylistDialogConst.CreatePlaylistButton);
            await CheckTableRowIsValid(playlistName, playlistIsPublic);
        }

        private async Task UpdateTestPlaylist(string playlistName, string playlistNameUpdated, bool playlistIsPublicUpdated)
        {
            await SearchPlaylist(playlistName);
            await ClickElement(PlaylistsPageConst.UpdatePlaylistButton);
            await FillInput(UpdatePlaylistDialogConst.NameInput, playlistNameUpdated);
            await ClickElement(UpdatePlaylistDialogConst.PublicCheckbox);
            await ClickElement(UpdatePlaylistDialogConst.UpdatePlaylistButton);
            await CheckTableRowIsValid(playlistNameUpdated, playlistIsPublicUpdated);
        }

        private async Task CheckTableRowIsValid(string name, bool isPublic)
        {
            await SearchPlaylist(name);
            await Expect(GetLocatorByTestId(PlaylistsPageConst.NameTableRowData)).ToHaveTextAsync(name);
            // assert public/private icon == isPublic
        }

        private async Task DeleteTestPlaylist(string playlistName)
        {
            await SearchPlaylist(playlistName);
            await ClickElement(PlaylistsPageConst.DeletePlaylistButton);
            await ClickElement(DeleteDialogConst.DeleteButton);
            await CheckTableRowIsHidden(playlistName);
        }

        private async Task CheckTableRowIsHidden(string name)
        {
            await SearchPlaylist(name);
            await Expect(GetLocatorByTestId(PlaylistsPageConst.NameTableRowData)).ToBeHiddenAsync();
        }
    }
}
