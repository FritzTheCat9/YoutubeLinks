using Microsoft.Playwright;
using static YoutubeLinks.Blazor.Pages.Links.LinksPage;
using static YoutubeLinks.Blazor.Pages.Playlists.PlaylistsPage;

namespace YoutubeLinks.E2E
{
    [TestFixture]
    public class LinksPageTests : PageTestBase
    {
        [SetUp]
        public async Task SetUp()
        {
            await NavigateToAdminPlaylists();

            var playlistName = "Test Playlist";
            var playlistIsPublic = false;

            await CreateTestPlaylist(playlistName, playlistIsPublic);

            await SearchPlaylist(playlistName);
            await ClickElement(PlaylistsPageConst.NavigateToPlaylistLinksButton);
        }

        [TearDown]
        public async Task TearDown()
        {
            var playlistName = "Test Playlist";

            await GoBackToPlaylistsPageFromLinksPage();
            await DeleteTestPlaylist(playlistName);
        }

        [Test]
        public async Task SortByTitle()
        {
            await ClickElement(LinksPageConst.TitleTableSortLabel);
            await ClickElement(LinksPageConst.TitleTableSortLabel);
            await ClickElement(LinksPageConst.TitleTableSortLabel);
        }

        [Test]
        public async Task SortByModified()
        {
            await ClickElement(LinksPageConst.ModifiedTableSortLabel);
            await ClickElement(LinksPageConst.ModifiedTableSortLabel);
            await ClickElement(LinksPageConst.ModifiedTableSortLabel);
        }

        [Test]
        public async Task CreateUpdateDeleteLink()
        {
            var title = "Rick Astley - Never Gonna Give You Up (Official Music Video)";
            var url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";
            await CreateLink(title, url);

            var updatedTitle = "Rick Astley - Never Gonna Give You Up";
            await UpdateLink(title, url, updatedTitle, false);

            await DeleteLink(updatedTitle);
        }

        [Test]
        public async Task CopyLinkToClipboard()
        {

        }

        [Test]
        public async Task DownloadLinkAsMP3()
        {
            var title = "Rick Astley - Never Gonna Give You Up (Official Music Video)";
            var url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";
            await CreateLink(title, url);

            var downloadTask = Page.WaitForDownloadAsync(new PageWaitForDownloadOptions { Timeout = 60000 });
            await ApiResponseOkAfterButtonClick(LinksPageConst.DownloadMP3FileButton, "links");
            var download = await downloadTask;

            var testProjectDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..");
            var tempFolderPath = Path.Combine(testProjectDirectoryPath, "Temp");
            var savePath = Path.Combine(tempFolderPath, download.SuggestedFilename);

            if (!Directory.Exists(tempFolderPath))
                Directory.CreateDirectory(tempFolderPath);

            await download.SaveAsAsync(savePath);

            await DeleteLink(title);
        }

        [Test]
        public async Task DownloadLinkAsMP4()
        {
            var title = "Rick Astley - Never Gonna Give You Up (Official Music Video)";
            var url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";
            await CreateLink(title, url);

            var downloadTask = Page.WaitForDownloadAsync(new PageWaitForDownloadOptions { Timeout = 60000 });
            await ApiResponseOkAfterButtonClick(LinksPageConst.DownloadMP4FileButton, "links");
            var download = await downloadTask;

            var testProjectDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..");
            var tempFolderPath = Path.Combine(testProjectDirectoryPath, "Temp");
            var savePath = Path.Combine(tempFolderPath, download.SuggestedFilename);

            if (!Directory.Exists(tempFolderPath))
                Directory.CreateDirectory(tempFolderPath);

            await download.SaveAsAsync(savePath);

            await DeleteLink(title);
        }

        [Test]
        public async Task SetAllPlaylistLinksAsUndownloaded()
        {

        }

        [Test]
        public async Task SetAllPlaylistLinksAsDownloaded()
        {

        }

        [Test]
        public async Task SwitchToGridView()
        {

        }
    }
}
