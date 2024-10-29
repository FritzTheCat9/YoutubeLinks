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

namespace YoutubeLinks.Blazor.Pages.Playlists;

public partial class PlaylistsPage(
    IExceptionHandler exceptionHandler,
    IPlaylistApiClient playlistApiClient,
    IAuthService authService,
    IStringLocalizer<App> localizer,
    IDialogService dialogService,
    NavigationManager navigationManager,
    IJSRuntime jsRuntime)
    : ComponentBase
{
    private bool _isUserPlaylist;
    private List<BreadcrumbItem> _items;
    private PagedList<PlaylistDto> _playlistPagedList;
    private string _searchString = "";
    private MudTable<PlaylistDto> _table;

    [Parameter] public int UserId { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        _items =
        [
            new BreadcrumbItem(localizer[nameof(AppStrings.Users)], "/users"),
            new BreadcrumbItem(localizer[nameof(AppStrings.Playlists)], null, true)
        ];

        _isUserPlaylist = await authService.IsLoggedInUser(UserId);
    }

    private async Task<TableData<PlaylistDto>> ServerReload(TableState state, CancellationToken token)
    {
        var query = new GetAllUserPlaylists.Query
        {
            Page = state.Page + 1,
            PageSize = state.PageSize,
            SortColumn = state.SortLabel,
            SortOrder = (SortOrder)state.SortDirection,
            SearchTerm = _searchString,
            UserId = UserId
        };

        try
        {
            _playlistPagedList = await playlistApiClient.GetAllUserPlaylists(query);
        }
        catch (Exception ex)
        {
            exceptionHandler.HandleExceptions(ex);
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
        var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true };
        var dialog = await dialogService.ShowAsync<DeleteDialog>(localizer[nameof(AppStrings.Delete)], options);

        var result = await dialog.Result;
        if (result is { Canceled: false })
        {
            try
            {
                await playlistApiClient.DeletePlaylist(id);
                await _table.ReloadServerData();
            }
            catch (Exception ex)
            {
                exceptionHandler.HandleExceptions(ex);
            }
        }
    }

    private async Task UpdateUserPlaylist(PlaylistDto playlistDto)
    {
        var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true };
        var parameters = new DialogParameters<UpdatePlaylistDialog>
        {
            {
                x => x.Command,
                new UpdatePlaylist.Command
                {
                    Id = playlistDto.Id,
                    Name = playlistDto.Name,
                    Public = playlistDto.Public
                }
            }
        };

        var dialog =
            await dialogService.ShowAsync<UpdatePlaylistDialog>(localizer[nameof(AppStrings.UpdatePlaylist)],
                parameters, options);
        var result = await dialog.Result;
        if (result is { Canceled: false })
        {
            await _table.ReloadServerData();
        }
    }

    private async Task CreateUserPlaylist()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true };
        var parameters = new DialogParameters<CreatePlaylistDialog>
        {
            {
                x => x.Command,
                new CreatePlaylist.Command
                {
                    Name = "",
                    Public = true
                }
            }
        };

        var dialog =
            await dialogService.ShowAsync<CreatePlaylistDialog>(localizer[nameof(AppStrings.CreatePlaylist)],
                parameters, options);
        var result = await dialog.Result;
        if (result is { Canceled: false })
        {
            await _table.ReloadServerData();
        }
    }

    private async Task ExportPlaylistToFile(int id, PlaylistFileType playlistFileType)
    {
        try
        {
            var command = new ExportPlaylist.Command
            {
                Id = id,
                PlaylistFileType = playlistFileType
            };
            var fileExtension = PlaylistHelpers.PlaylistFileTypeToString(playlistFileType);

            var response = await playlistApiClient.ExportPlaylist(command);

            var content = await response.Content.ReadAsStreamAsync();
            var streamRef = new DotNetStreamReference(content);
            var filename = response.Content.Headers.ContentDisposition?.FileNameStar ?? $"default_name.{fileExtension}";

            await jsRuntime.InvokeVoidAsync("downloadFile", filename, streamRef);
        }
        catch (Exception ex)
        {
            exceptionHandler.HandleExceptions(ex);
        }
    }

    private async Task ImportPlaylistFromJson()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true };
        var parameters = new DialogParameters<ImportPlaylistDialog>
        {
            {
                x => x.FormModel,
                new ImportPlaylist.FormModel
                {
                    Name = "",
                    Public = true,
                    ExportedLinks = [],
                    File = null
                }
            }
        };

        var dialog =
            await dialogService.ShowAsync<ImportPlaylistDialog>(localizer[nameof(AppStrings.ImportPlaylist)],
                parameters, options);
        var result = await dialog.Result;
        if (result is { Canceled: false })
        {
            await _table.ReloadServerData();
        }
    }

    private void RedirectToLinksPage(int id)
    {
        navigationManager.NavigateTo($"/links/{UserId}/{id}");
    }

    private void RedirectToDownloadPlaylistPage(int id)
    {
        navigationManager.NavigateTo($"/downloadPlaylist/{id}");
    }

    public abstract class PlaylistsPageConst
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
}