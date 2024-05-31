using Microsoft.Playwright;
using NUnit.Framework.Internal;
using static YoutubeLinks.Blazor.Pages.Links.DownloadLinkPage;

namespace YoutubeLinks.E2E
{
    [TestFixture]
    public class DownloadLinkPageTests : PageTestBase
    {
        [SetUp]
        public async Task SetUp()
        {
            await NavigateToPage("downloadLink");
        }

        [Test]
        public async Task DownloadMP3()
        {
            var url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";
            await FillInput(DownloadLinkPageConst.UrlInput, url);

            await Page.ClickAsync("div.mud-select-input");
            var mudSelects = await Page.QuerySelectorAllAsync("div.mud-popover div.mud-list-item");
            if (mudSelects.Count >= 2)
                await mudSelects[0].ClickAsync();

            await DownloadLink(DownloadLinkPageConst.DownloadButton);
        }

        [Test]
        public async Task DownloadMP4()
        {
            var url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";
            await FillInput(DownloadLinkPageConst.UrlInput, url);

            await Page.ClickAsync("div.mud-select-input");
            var mudSelects = await Page.QuerySelectorAllAsync("div.mud-popover div.mud-list-item");
            if (mudSelects.Count >= 2)
                await mudSelects[1].ClickAsync();

            await DownloadLink(DownloadLinkPageConst.DownloadButton);
        }

        private async Task DownloadLink(string downloadFileButton)
        {
            var downloadTask = Page.WaitForDownloadAsync(new PageWaitForDownloadOptions { Timeout = 60000 });
            await ApiResponseOkAfterButtonClick(downloadFileButton, "links/downloadSingle");
            var download = await downloadTask;

            var testProjectDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..");
            var tempFolderPath = Path.Combine(testProjectDirectoryPath, "Temp");
            var savePath = Path.Combine(tempFolderPath, download.SuggestedFilename);

            if (!Directory.Exists(tempFolderPath))
                Directory.CreateDirectory(tempFolderPath);

            await download.SaveAsAsync(savePath);
        }
    }
}
