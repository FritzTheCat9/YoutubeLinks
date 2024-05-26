using static YoutubeLinks.Blazor.Pages.Links.CreateLinkForm;
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
        public async Task Filter()
        {

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
            var testLinkUrl = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";

            await FillInput(CreateLinkFormConst.UrlInput, testLinkUrl);
            await ApiResponseOkAfterButtonClick(CreateLinkFormConst.CreateButton, "links");

            // update

            //delete
        }

        [Test]
        public async Task CopyLinkToClipboard()
        {

        }

        [Test]
        public async Task DownloadLinkAsMP3()
        {

        }

        [Test]
        public async Task DownloadLinkAsMP4()
        {

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
