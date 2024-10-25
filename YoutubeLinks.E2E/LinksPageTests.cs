using Microsoft.Playwright;
using static YoutubeLinks.Blazor.Pages.Links.LinksPage;
using static YoutubeLinks.Blazor.Pages.Playlists.PlaylistsPage;
using static YoutubeLinks.Blazor.Shared.InformationDialog;

namespace YoutubeLinks.E2E
{
    [TestFixture]
    public class LinksPageTests : PageTestBase
    {
        [SetUp]
        public async Task SetUp()
        {
            await NavigateToAdminPlaylists();

            const string playlistName = "Test Playlist";
            const bool playlistIsPublic = false;

            await CreateTestPlaylist(playlistName, playlistIsPublic);

            await SearchPlaylist(playlistName);
            await ClickElement(PlaylistsPageConst.NavigateToPlaylistLinksButton);
        }

        [TearDown]
        public async Task TearDown()
        {
            const string playlistName = "Test Playlist";

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
            const string title = "Rick Astley - Never Gonna Give You Up (Official Music Video)";
            const string url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";
            await CreateLink(title, url);

            const string updatedTitle = "Rick Astley - Never Gonna Give You Up";
            await UpdateLink(title, url, updatedTitle, false);

            await DeleteLink(updatedTitle);
        }

        [Test]
        public async Task CopyLinkToClipboard()
        {
            const string title = "Rick Astley - Never Gonna Give You Up (Official Music Video)";
            const string url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";
            await CreateLink(title, url);

            await ClickElement(LinksPageConst.CopyToClipboardButton);
            var clipboardText = await Page.EvaluateAsync<string>("navigator.clipboard.readText()");
            Assert.That(clipboardText, Is.EqualTo(url));

            await DeleteLink(title);
        }

        [Test]
        public async Task DownloadLinkAsMp3()
        {
            await DownloadLink(LinksPageConst.DownloadMp3FileButton);
        }

        [Test]
        public async Task DownloadLinkAsMp4()
        {
            await DownloadLink(LinksPageConst.DownloadMp4FileButton);
        }

        private async Task DownloadLink(string downloadFileButton)
        {
            const string title = "Rick Astley - Never Gonna Give You Up (Official Music Video)";
            const string url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";
            await CreateLink(title, url);

            var downloadTask = Page.WaitForDownloadAsync(new PageWaitForDownloadOptions { Timeout = 60000 });
            await ApiResponseOkAfterButtonClick(downloadFileButton, "links");
            var download = await downloadTask;

            await SaveDownloadedFile(download);

            await DeleteLink(title);
        }

        [Test]
        public async Task SetAllPlaylistLinksAsDownloadedAndUndownloaded()
        {
            const string title = "Rick Astley - Never Gonna Give You Up (Official Music Video)";
            const string url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";
            await CreateLink(title, url);

            await ClickElement(LinksPageConst.SetAllPlaylistLinksAsDownloadedButton);
            await ApiResponseOkAfterButtonClick(InformationDialogConst.ConfirmButton, "playlists/resetDownloadedFlag");

            await ClickElement(LinksPageConst.SetAllPlaylistLinksAsUndownloadedButton);
            await ApiResponseOkAfterButtonClick(InformationDialogConst.ConfirmButton, "playlists/resetDownloadedFlag");

            await DeleteLink(title);
        }

        [Test]
        public async Task SwitchToGridView()
        {
            const string title = "Rick Astley - Never Gonna Give You Up (Official Music Video)";
            const string url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";
            await CreateLink(title, url);

            await ClickElement(LinksPageConst.SwitchToGridViewButton);
            await ClickElement(LinksPageConst.SwitchToTableViewButton);

            await DeleteLink(title);
        }
    }
}
