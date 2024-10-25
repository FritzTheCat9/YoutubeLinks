using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using MudBlazor;
using YoutubeLinks.Blazor.Auth;
using YoutubeLinks.Blazor.Clients;
using YoutubeLinks.Blazor.Exceptions;
using YoutubeLinks.Blazor.Localization;
using YoutubeLinks.Blazor.Shared;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Playlists.Commands;
using YoutubeLinks.Shared.Features.Playlists.Helpers;
using YoutubeLinks.Shared.Features.Playlists.Queries;
using YoutubeLinks.Shared.Features.Playlists.Responses;

namespace YoutubeLinks.Blazor.Pages.Playlists
{
    public partial class PlaylistsPage : ComponentBase
    {
        private List<BreadcrumbItem> _items;
        private MudTable<PlaylistDto> _table;

        private bool _isUserPlaylist;

        private string _searchString = "";
        private PagedList<PlaylistDto> _playlistPagedList;

        public class PlaylistsPageConst
        {
            public const string CreatePlaylistButton = "playlists-page-create-playlist-button";
            public const string UpdatePlaylistButton = "playlists-page-update-playlist-button";
            public const string DeletePlaylistButton = "playlists-page-delete-playlist-button";
            public const string ImportPlaylistButton = "playlists-page-import-playlist-button";
            public const string ExportPlaylistButton = "playlists-page-export-playlist-button";
            public const string ExportPlaylistToJsonButton = "playlists-page-export-playlist-to-json-button";
            public const string ExportPlaylistToTxtButton = "playlists-page-export-playlist-to-txt-button";
            public const string DownloadPlaylistButton = "playlists-page-download-playlist-button";
            public const string NavigateToPlaylistLinksButton = "playlists-page-navigate-to-playlist-links-button";
            public const string SearchInput = "playlists-page-search-input";
            public const string NameTableSortLabel = "playlists-page-name-table-sort-label";
            public const string NameTableRowData = "playlists-page-name-table-row-data";
        }

        [Parameter] public int UserId { get; set; }

        [Inject] public IExceptionHandler ExceptionHandler { get; set; }
        [Inject] public IPlaylistApiClient PlaylistApiClient { get; set; }

        [Inject] public IAuthService AuthService { get; set; }
        [Inject] public IStringLocalizer<App> Localizer { get; set; }

        [Inject] public IDialogService DialogService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }


        protected override async Task OnParametersSetAsync()
        {
            _items =
            [
                new BreadcrumbItem(Localizer[nameof(AppStrings.Users)], href: $"/users"),
                new BreadcrumbItem(Localizer[nameof(AppStrings.Playlists)], href: null, disabled: true),
            ];

            _isUserPlaylist = await AuthService.IsLoggedInUser(UserId);
        }

        private async Task<TableData<PlaylistDto>> ServerReload(TableState state, CancellationToken token)
        {
            var query = new GetAllUserPlaylists.Query
            {
                Page = state.Page + 1,
                PageSize = state.PageSize,
                SortColumn = state.SortLabel,
                SortOrder = ((SortOrder)state.SortDirection),
                SearchTerm = _searchString,
                UserId = UserId,
            };

            try
            {
                _playlistPagedList = await PlaylistApiClient.GetAllUserPlaylists(query);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleExceptions(ex);
                return new TableData<PlaylistDto> { TotalItems = 0, Items = [] };
            }

            return new TableData<PlaylistDto>
            {
                TotalItems = _playlistPagedList.TotalCount,
                Items = _playlistPagedList.Items
            };
        }

        private async Task DeleteUserPlaylist(int id)
        {
            var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
            var dialog = await DialogService.ShowAsync<DeleteDialog>(Localizer[nameof(AppStrings.Delete)], options);

            var result = await dialog.Result;
            if (!result.Canceled)
            {
                try
                {
                    await PlaylistApiClient.DeletePlaylist(id);
                    await _table.ReloadServerData();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.HandleExceptions(ex);
                }
            }
        }

        private async Task UpdateUserPlaylist(PlaylistDto playlistDto)
        {
            var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
            var parameters = new DialogParameters<UpdatePlaylistDialog>
            {
                {
                    x => x.Command,
                    new UpdatePlaylist.Command
                    {
                        Id = playlistDto.Id,
                        Name = playlistDto.Name,
                        Public = playlistDto.Public,
                    }
                }
            };

            var dialog = await DialogService.ShowAsync<UpdatePlaylistDialog>(Localizer[nameof(AppStrings.UpdatePlaylist)], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
                await _table.ReloadServerData();
        }

        private async Task CreateUserPlaylist()
        {
            var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
            var parameters = new DialogParameters<CreatePlaylistDialog>
            {
                {
                    x => x.Command,
                    new CreatePlaylist.Command
                    {
                        Name = "",
                        Public = true,
                    }
                }
            };

            var dialog = await DialogService.ShowAsync<CreatePlaylistDialog>(Localizer[nameof(AppStrings.CreatePlaylist)], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
                await _table.ReloadServerData();
        }

        private async Task ExportPlaylistToFile(int id, PlaylistFileType playlistFileType)
        {
            try
            {
                var command = new ExportPlaylist.Command
                {
                    Id = id,
                    PlaylistFileType = playlistFileType,
                };
                var fileExtension = PlaylistHelpers.PlaylistFileTypeToString(playlistFileType);

                var response = await PlaylistApiClient.ExportPlaylist(command);

                var content = await response.Content.ReadAsStreamAsync();
                var streamRef = new DotNetStreamReference(content);
                var filename = response.Content.Headers.ContentDisposition.FileNameStar ?? $"default_name.{fileExtension}";

                await JsRuntime.InvokeVoidAsync("downloadFile", filename, streamRef);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleExceptions(ex);
            }
        }

        private async Task ImportPlaylistFromJson()
        {
            var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
            var parameters = new DialogParameters<ImportPlaylistDialog>
            {
                {
                    x => x.FormModel,
                    new ImportPlaylist.FormModel
                    {
                        Name = "",
                        Public = true,
                        ExportedLinks = [],
                        File = null,
                    }
                }
            };

            var dialog = await DialogService.ShowAsync<ImportPlaylistDialog>(Localizer[nameof(AppStrings.ImportPlaylist)], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
                await _table.ReloadServerData();
        }

        private void RedirectToLinksPage(int id)
            => NavigationManager.NavigateTo($"/links/{UserId}/{id}");

        private void RedirectToDownloadPlaylistPage(int id)
            => NavigationManager.NavigateTo($"/downloadPlaylist/{id}");
    }
}