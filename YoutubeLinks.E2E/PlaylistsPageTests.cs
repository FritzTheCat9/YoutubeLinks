using Microsoft.Playwright;
using static YoutubeLinks.Blazor.Pages.Playlists.DownloadPlaylistPage;
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
            var playlistName = "Test Playlist";
            var playlistIsPublic = false;

            await CreateTestPlaylist(playlistName, playlistIsPublic);

            await SearchPlaylist(playlistName);
            await ClickElement(PlaylistsPageConst.NavigateToPlaylistLinksButton);

            var title = "Rick Astley - Never Gonna Give You Up (Official Music Video)";
            var url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";
            await CreateLink(title, url);

            await GoBackToPlaylistsPageFromLinksPage();
            await SearchPlaylist(playlistName);

            await ExportPlaylist(PlaylistsPageConst.ExportPlaylistToJsonButton);
            await ExportPlaylist(PlaylistsPageConst.ExportPlaylistToTxtButton);

            await DeleteTestPlaylist(playlistName);
        }

        private async Task ExportPlaylist(string downloadFileButton)
        {
            await Page.GetByTestId(PlaylistsPageConst.ExportPlaylistButton).ClickAsync();

            var downloadTask = Page.WaitForDownloadAsync(new PageWaitForDownloadOptions { Timeout = 60000 });
            await ApiResponseOkAfterButtonClick(downloadFileButton, "playlists/export");
            var download = await downloadTask;

            var testProjectDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..");
            var tempFolderPath = Path.Combine(testProjectDirectoryPath, "Temp");
            var savePath = Path.Combine(tempFolderPath, download.SuggestedFilename);

            if (!Directory.Exists(tempFolderPath))
                Directory.CreateDirectory(tempFolderPath);

            await download.SaveAsAsync(savePath);
        }

        [Test]
        public async Task ImportPlaylist()
        {

        }

        [Test]
        public async Task DownloadPlaylistMP3()
        {
            var playlistName = "Test Playlist";
            var playlistIsPublic = false;

            await CreateTestPlaylist(playlistName, playlistIsPublic);

            await SearchPlaylist(playlistName);
            await ClickElement(PlaylistsPageConst.NavigateToPlaylistLinksButton);

            var title = "Rick Astley - Never Gonna Give You Up (Official Music Video)";
            var url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";
            await CreateLink(title, url);

            await GoBackToPlaylistsPageFromLinksPage();
            await SearchPlaylist(playlistName);

            await ClickElement(PlaylistsPageConst.DownloadPlaylistButton);

            var downloadTask = Page.WaitForDownloadAsync(new PageWaitForDownloadOptions { Timeout = 60000 });
            await ApiResponseOkAfterButtonClick(DownloadPlaylistPageConst.DownloadButton, "links/download");
            var download = await downloadTask;

            var testProjectDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..");
            var tempFolderPath = Path.Combine(testProjectDirectoryPath, "Temp");
            var savePath = Path.Combine(tempFolderPath, download.SuggestedFilename);

            if (!Directory.Exists(tempFolderPath))
                Directory.CreateDirectory(tempFolderPath);

            await download.SaveAsAsync(savePath);

            await ClickElement("Playlists");

            await DeleteTestPlaylist(playlistName);
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
