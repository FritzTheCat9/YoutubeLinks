using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using MudBlazor;
using YoutubeLinks.Blazor.Auth;
using YoutubeLinks.Shared.Clients;
using YoutubeLinks.Blazor.Exceptions;
using YoutubeLinks.Blazor.Localization;
using YoutubeLinks.Blazor.Services;
using YoutubeLinks.Blazor.Shared;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Shared.Features.Links.Helpers;
using YoutubeLinks.Shared.Features.Links.Queries;
using YoutubeLinks.Shared.Features.Links.Responses;
using YoutubeLinks.Shared.Features.Playlists.Commands;

namespace YoutubeLinks.Blazor.Pages.Links;

public partial class LinksPage(
    IExceptionHandler exceptionHandler,
    ILinkApiClient linkApiClient,
    IPlaylistApiClient playlistApiClient,
    IAuthService authService,
    ITableViewProvider tableViewProvider,
    IStringLocalizer<App> localizer,
    IDialogService dialogService,
    IJSRuntime jsRuntime)
    : ComponentBase
{
    private readonly CreateLink.Command _createLinkCommand = new()
    {
        Url = ""
    };

    private readonly GetAllPaginatedLinks.Query _query = new()
    {
        Page = 1,
        PageSize = 10,
        SortColumn = "modified",
        SortOrder = SortOrder.Descending,
        SearchTerm = ""
    };

    private bool _disableDownloadPlaylistLinkButtons;
    private bool _isUserPlaylist;
    private List<BreadcrumbItem> _items;
    private PagedList<LinkDto> _linkPagedList;
    private string _searchString = "";
    private MudTable<LinkDto> _table;
    private bool _tableView = true;

    [Parameter] public int UserId { get; set; }
    [Parameter] public int PlaylistId { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        _items =
        [
            new BreadcrumbItem(localizer[nameof(AppStrings.Users)], "/users"),
            new BreadcrumbItem(localizer[nameof(AppStrings.Playlists)], $"/playlists/{UserId}"),
            new BreadcrumbItem(localizer[nameof(AppStrings.Links)], null, true)
        ];

        _createLinkCommand.PlaylistId = PlaylistId;
        _query.PlaylistId = PlaylistId;

        _tableView = await tableViewProvider.GetTableView();
        _isUserPlaylist = await authService.IsLoggedInUser(UserId);

        await RefreshView();
    }

    private async Task RefreshView()
    {
        if (_tableView)
        {
            await _table.ReloadServerData();
        }
        else
        {
            await ReloadLinks();
        }
    }

    private async Task<TableData<LinkDto>> ServerReload(TableState state, CancellationToken token)
    {
        try
        {
            _query.Page = state.Page + 1;
            _query.PageSize = state.PageSize;
            _query.SortColumn = state.SortLabel;
            _query.SortOrder = (SortOrder)state.SortDirection;
            _query.SearchTerm = _searchString;

            _linkPagedList = await linkApiClient.GetAllPaginatedLinks(_query);
        }
        catch (Exception ex)
        {
            exceptionHandler.HandleExceptions(ex);
            return new TableData<LinkDto> { TotalItems = 0, Items = [] };
        }

        return new TableData<LinkDto>
        {
            TotalItems = _linkPagedList.TotalCount,
            Items = _linkPagedList.Items
        };
    }

    private async Task ReloadLinks()
    {
        try
        {
            _query.SearchTerm = _searchString;

            _linkPagedList = await linkApiClient.GetAllPaginatedLinks(_query);
        }
        catch (Exception ex)
        {
            exceptionHandler.HandleExceptions(ex);
        }
    }

    private async Task Paginate(int page)
    {
        _query.Page = page;
        await RefreshView();
    }

    private async Task SwitchView()
    {
        _tableView = !_tableView;
        await tableViewProvider.SetTableView(_tableView);

        _query.Page = 1;
        await RefreshView();
    }

    private async Task DeletePlaylistLink(int id)
    {
        var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true };
        var dialog = await dialogService.ShowAsync<DeleteDialog>(localizer[nameof(AppStrings.Delete)], options);

        var result = await dialog.Result;
        if (result is { Canceled: false })
        {
            try
            {
                await linkApiClient.DeleteLink(id);
                await RefreshView();
            }
            catch (Exception ex)
            {
                exceptionHandler.HandleExceptions(ex);
            }
        }
    }

    private async Task UpdatePlaylistLink(LinkDto linkDto)
    {
        var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true };
        var parameters = new DialogParameters<UpdateLinkDialog>
        {
            {
                x => x.Command,
                new UpdateLink.Command
                {
                    Id = linkDto.Id,
                    Url = linkDto.Url,
                    Title = linkDto.Title,
                    Downloaded = linkDto.Downloaded
                }
            }
        };

        var dialog =
            await dialogService.ShowAsync<UpdateLinkDialog>(localizer[nameof(AppStrings.UpdateLink)], parameters,
                options);
        var result = await dialog.Result;
        if (result is { Canceled: false })
        {
            await RefreshView();
        }
    }

    private async Task DownloadPlaylistLink(int id, YoutubeFileType youtubeFileType)
    {
        try
        {
            _disableDownloadPlaylistLinkButtons = true;

            var command = new DownloadLink.Command
            {
                Id = id,
                YoutubeFileType = youtubeFileType
            };

            var response = await linkApiClient.DownloadLink(command);
            var content = await response.Content.ReadAsStreamAsync();
            var streamRef = new DotNetStreamReference(content);
            var filename = response.Content.Headers.ContentDisposition?.FileNameStar ??
                           $"default_name.{YoutubeHelpers.YoutubeFileTypeToString(command.YoutubeFileType)}";

            await jsRuntime.InvokeVoidAsync("downloadFile", filename, streamRef);

            await SetLinkAsDownloaded(id);

            await RefreshView();
        }
        catch (Exception ex)
        {
            exceptionHandler.HandleExceptions(ex);
        }
        finally
        {
            _disableDownloadPlaylistLinkButtons = false;
        }
    }

    private async Task SetLinkAsDownloaded(int id)
    {
        var command = new SetLinkDownloadedFlag.Command
        {
            Id = id,
            Downloaded = true
        };

        await linkApiClient.SetLinkDownloadedFlag(command);
    }

    private async Task ResetPlaylistLinksDownloadedFlag(bool flag)
    {
        var dialogText = flag
            ? localizer[nameof(AppStrings.SetAllPlaylistLinksAsDownloaded)]
            : localizer[nameof(AppStrings.SetAllPlaylistLinksAsUndownloaded)];

        var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true };
        var dialog = await dialogService.ShowAsync<InformationDialog>(dialogText, options);

        var result = await dialog.Result;
        if (result is { Canceled: false })
        {
            try
            {
                var command = new ResetLinksDownloadedFlag.Command
                {
                    Id = PlaylistId,
                    IsDownloaded = flag
                };

                await playlistApiClient.ResetLinksDownloadedFlag(command);
                await RefreshView();
            }
            catch (Exception ex)
            {
                exceptionHandler.HandleExceptions(ex);
            }
        }
    }

    private static string ConvertToEmbedUrl(string url)
    {
        return url.Replace("/watch?v=", "/embed/");
    }

    public abstract class LinksPageConst
    {
        public const string SetAllPlaylistLinksAsUndownloadedButton =
            "links-page-set-all-playlist-links-as-undownloaded-button";

        public const string SetAllPlaylistLinksAsDownloadedButton =
            "links-page-set-all-playlist-links-as-downloaded-button";

        public const string SwitchToGridViewButton = "links-page-switch-to-grid-view-button";
        public const string SearchInput = "links-page-search-input";
        public const string TitleTableSortLabel = "links-page-title-table-sort-label";
        public const string TitleTableRowData = "links-page-title-table-row-data";
        public const string ModifiedTableSortLabel = "links-page-modified-table-sort-label";
        public const string ModifiedTableRowData = "links-page-modified-table-row-data";
        public const string CopyToClipboardButton = "links-page-copy-to-clipboard-button";
        public const string DownloadMp3FileButton = "links-page-download-mp3-file-button";
        public const string DownloadMp4FileButton = "links-page-download-mp4-file-button";
        public const string UpdateLinkButton = "links-page-update-link-button";
        public const string DeleteLinkButton = "links-page-delete-link-button";
        public const string SwitchToTableViewButton = "links-page-switch-to-table-view-button";
    }
}