using static YoutubeLinks.Blazor.Pages.Playlists.PlaylistsPage;

namespace YoutubeLinks.E2E
{
    [TestFixture]
    public class PlaylistsPageTests : PageTestBase
    {
        [SetUp]
        public async Task SetUp()
        {
            await NavigateToAdminPlaylists();
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
    }
}
