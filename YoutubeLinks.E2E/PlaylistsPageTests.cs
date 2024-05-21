using static YoutubeLinks.Blazor.Pages.Playlists.CreatePlaylistDialog;
using static YoutubeLinks.Blazor.Pages.Playlists.PlaylistsPage;
using static YoutubeLinks.Blazor.Pages.Playlists.UpdatePlaylistDialog;
using static YoutubeLinks.Blazor.Pages.Users.UsersPage;

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
        public async Task Filter()
        {

        }

        [Test]
        public async Task SortByName()
        {

        }

        private async Task CheckTableRowIsValid(string name, bool isPublic)
        {
            await FillInput(PlaylistsPageConst.SearchInput, name);
            await ClickEnter(PlaylistsPageConst.SearchInput);
            await Expect(GetLocatorByTestId(PlaylistsPageConst.NameTableRowData)).ToHaveTextAsync(name);
            // assert public/private icon == isPublic
        }

        [Test]
        public async Task CreatePlaylist()
        {
            var playlistName = "Test Playlist";
            var playlistIsPublic = false;

            await ClickElement(PlaylistsPageConst.CreatePlaylistButton);
            await FillInput(CreatePlaylistDialogConst.NameInput, playlistName);
            await ClickElement(CreatePlaylistDialogConst.PublicCheckbox);
            await ClickElement(CreatePlaylistDialogConst.CreatePlaylistButton);

            await CheckTableRowIsValid(playlistName, playlistIsPublic);
        }

        [Test]
        public async Task UpdatePlaylist()
        {
            var playlistName = "Test Playlist";

            await FillInput(PlaylistsPageConst.SearchInput, playlistName);
            await ClickEnter(PlaylistsPageConst.SearchInput);
            await Expect(GetLocatorByTestId(PlaylistsPageConst.NameTableRowData)).ToHaveTextAsync(playlistName);

            var playlistNameUpdated = "Test Playlist - Updated";
            var playlistIsPublic = true;

            await ClickElement(PlaylistsPageConst.UpdatePlaylistButton);
            await FillInput(UpdatePlaylistDialogConst.NameInput, playlistNameUpdated);
            await ClickElement(UpdatePlaylistDialogConst.PublicCheckbox);
            await ClickElement(UpdatePlaylistDialogConst.UpdatePlaylistButton);

            await CheckTableRowIsValid(playlistNameUpdated, playlistIsPublic);
        }

        [Test]
        public async Task DeletePlaylist()
        {

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

        }
    }
}
