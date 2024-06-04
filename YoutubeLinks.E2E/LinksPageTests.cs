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
            var title = "Rick Astley - Never Gonna Give You Up (Official Music Video)";
            var url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";
            await CreateLink(title, url);

            await ClickElement(LinksPageConst.CopyToClipboardButton);
            string clipboardText = await Page.EvaluateAsync<string>("navigator.clipboard.readText()");
            Assert.That(clipboardText, Is.EqualTo(url));

            await DeleteLink(title);
        }

        [Test]
        public async Task DownloadLinkAsMP3()
        {
            await DownloadLink(LinksPageConst.DownloadMP3FileButton);
        }

        [Test]
        public async Task DownloadLinkAsMP4()
        {
            await DownloadLink(LinksPageConst.DownloadMP4FileButton);
        }

        private async Task DownloadLink(string downloadFileButton)
        {
            var title = "Rick Astley - Never Gonna Give You Up (Official Music Video)";
            var url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";
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
            var title = "Rick Astley - Never Gonna Give You Up (Official Music Video)";
            var url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";
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
            var title = "Rick Astley - Never Gonna Give You Up (Official Music Video)";
            var url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";
            await CreateLink(title, url);

            await ClickElement(LinksPageConst.SwitchToGridViewButton);
            await ClickElement(LinksPageConst.SwitchToTableViewButton);

            await DeleteLink(title);
        }
    }
}
