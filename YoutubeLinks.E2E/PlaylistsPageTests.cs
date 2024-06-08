using Microsoft.Playwright;
using YoutubeLinks.Shared.Features.Playlists.Helpers;
using static YoutubeLinks.Blazor.Pages.Playlists.DownloadPlaylistPage;
using static YoutubeLinks.Blazor.Pages.Playlists.ImportPlaylistDialog;
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
            await ClickElement(PlaylistsPageConst.ExportPlaylistButton);

            var downloadTask = Page.WaitForDownloadAsync(new PageWaitForDownloadOptions { Timeout = 60000 });
            await ApiResponseOkAfterButtonClick(downloadFileButton, "playlists/export");
            var download = await downloadTask;

            await SaveDownloadedFile(download);
        }

        [Test]
        public async Task ImportPlaylist()
        {
            // Export Playlist
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

            // Import Playlist
            var importedPlaylistName = $"{playlistName} Imported";

            await ImportPlaylist(playlistName, PlaylistFileType.JSON, importedPlaylistName);
            await SearchPlaylist(importedPlaylistName);
            await DeleteTestPlaylist(importedPlaylistName);

            await ImportPlaylist(playlistName, PlaylistFileType.TXT, importedPlaylistName);
            await SearchPlaylist(importedPlaylistName);
            await DeleteTestPlaylist(importedPlaylistName);
        }

        private async Task ImportPlaylist(string playlistName, PlaylistFileType playlistFileType, string importedPlaylistName)
        {
            var playlistTypeString = PlaylistHelpers.PlaylistFileTypeToString(playlistFileType);

            await ClickElement(PlaylistsPageConst.ImportPlaylistButton);

            var fileChooser = await Page.RunAndWaitForFileChooserAsync(async () =>
            {
                await Page.GetByText("ADD .JSON OR .TXT FILE").ClickAsync();
            });
            var filePath = GetDownloadedFilePath($"{playlistName}.{playlistTypeString}");
            await fileChooser.SetFilesAsync(filePath);

            await FillInput(ImportPlaylistDialogConst.NameInput, importedPlaylistName);
            await ClickElement(ImportPlaylistDialogConst.ImportPlaylistButton);
        }

        [Test]
        public async Task DownloadPlaylistMP3()
        {
            await DownloadPlatlist();
        }

        [Test]
        public async Task DownloadPlaylistMP4()
        {
            await DownloadPlatlist(true);
        }

        private async Task DownloadPlatlist(bool mp4 = false)
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

            if (mp4)
            {
                await Page.ClickAsync("div.mud-select-input");
                var mudSelects = await Page.QuerySelectorAllAsync("div.mud-popover div.mud-list-item");
                if (mudSelects.Count >= 2)
                    await mudSelects[1].ClickAsync();
            }

            await ApiResponseOkAfterButtonClick(DownloadPlaylistPageConst.DownloadButton, "links/download");
            var download = await downloadTask;

            await SaveDownloadedFile(download);

            await ClickElement("Playlists");

            await DeleteTestPlaylist(playlistName);
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
