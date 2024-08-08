using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using MudBlazor;
using YoutubeLinks.Blazor.Auth;
using YoutubeLinks.Blazor.Clients;
using YoutubeLinks.Blazor.Components;
using YoutubeLinks.Blazor.Exceptions;
using YoutubeLinks.Blazor.Localization;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Shared.Features.Links.Helpers;
using YoutubeLinks.Shared.Features.Links.Queries;
using YoutubeLinks.Shared.Features.Playlists.Responses;

namespace YoutubeLinks.Blazor.Pages.Playlists
{
    public partial class DownloadPlaylistPage : ComponentBase
    {
        private List<BreadcrumbItem> _items;
        private FritzProcessingButton _processingButton;

        private bool _isUserPlaylist = false;

        private PlaylistDto _playlist;
        private List<GetAllLinks.LinkInfoDto> _linkInfoList;
        private YoutubeFileType _youtubeFileType = YoutubeFileType.MP3;

        private List<DownloadLinkResult> _downloadLinkResults = [];

        private class DownloadLinkResult
        {
            public GetAllLinks.LinkInfoDto Link { get; set; }
            public bool Success { get; set; }
        }

        private bool _downloading;
        private int _downloadedSongsNumber;
        private int _allSongsNumber;
        private double _downloadPercent;
        private string _downloadingSongTitle = "";

        public class DownloadPlaylistPageConst
        {
            public const string YoutubeFileTypeSelect = "download-playlist-page-youtube-file-type-select";
            public const string DownloadButton = "download-playlist-page-download-button";
        }

        [Parameter] public int PlaylistId { get; set; }

        [Inject] public IExceptionHandler ExceptionHandler { get; set; }
        [Inject] public ILinkApiClient LinkApiClient { get; set; }
        [Inject] public IPlaylistApiClient PlaylistApiClient { get; set; }

        [Inject] public IAuthService AuthService { get; set; }
        [Inject] public IStringLocalizer<App> Localizer { get; set; }

        [Inject] public IDialogService DialogService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            try
            {
                _playlist = await PlaylistApiClient.GetPlaylist(PlaylistId);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleExceptions(ex);
            }

            _items =
            [
                new(Localizer[nameof(AppStrings.Users)], href: $"/users"),
                new(Localizer[nameof(AppStrings.Playlists)],  href: $"/playlists/{_playlist.UserId}"),
                new(Localizer[nameof(AppStrings.DownloadPlaylist)], href: null, disabled: true),
            ];

            _isUserPlaylist = await AuthService.IsLoggedInUser(_playlist.UserId);

            if (!_playlist.Public && !_isUserPlaylist)
            {
                NavigationManager.NavigateTo("/error/forbidden-error");
            }

            await LoadLinkInformation();
        }

        private async Task LoadLinkInformation()
        {
            try
            {
                var query = new GetAllLinks.Query
                {
                    PlaylistId = PlaylistId,
                    Downloaded = false,
                };

                _linkInfoList = (await LinkApiClient.GetAllLinks(query)).ToList();
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleExceptions(ex);
            }
        }

        private async Task DownloadPlaylistLinks()
        {
            try
            {
                await LoadLinkInformation();

                _downloading = true;
                _allSongsNumber = _linkInfoList.Count;
                _processingButton.SetProcessing(true);

                _downloadLinkResults = [];
                _downloadedSongsNumber = 0;
                _downloadPercent = 0;
                _downloadingSongTitle = "";

                foreach (var link in _linkInfoList)
                {
                    _downloadingSongTitle = link.Title;

                    StateHasChanged();

                    await DownloadPlaylistLink(link, _youtubeFileType);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleExceptions(ex);
            }
            finally
            {
                _downloading = false;
                _processingButton.SetProcessing(false);
            }
        }

        private async Task DownloadPlaylistLink(GetAllLinks.LinkInfoDto link, YoutubeFileType youtubeFileType)
        {
            try
            {
                var command = new DownloadLink.Command
                {
                    Id = link.Id,
                    YoutubeFileType = youtubeFileType,
                };

                var response = await LinkApiClient.DownloadLink(command);
                var content = await response.Content.ReadAsStreamAsync();
                var streamRef = new DotNetStreamReference(content);
                var filename = response.Content.Headers.ContentDisposition.FileNameStar ?? $"default_name.{YoutubeHelpers.YoutubeFileTypeToString(command.YoutubeFileType)}";

                await JSRuntime.InvokeVoidAsync("downloadFile", filename, streamRef);

                await SetLinkAsDownloaded(link.Id);

                _downloadLinkResults.Add(new()
                {
                    Link = link,
                    Success = true,
                });

            }
            catch (Exception)
            {
                _downloadLinkResults.Add(new()
                {
                    Link = link,
                    Success = false,
                });
            }

            _downloadedSongsNumber++;
            _downloadPercent = (double)_downloadedSongsNumber / _allSongsNumber * 100;

            StateHasChanged();
        }

        private async Task SetLinkAsDownloaded(int id)
        {
            var command = new SetLinkDownloadedFlag.Command
            {
                Id = id,
                Downloaded = true,
            };

            await LinkApiClient.SetLinkDownloadedFlag(command);
        }
    }
}