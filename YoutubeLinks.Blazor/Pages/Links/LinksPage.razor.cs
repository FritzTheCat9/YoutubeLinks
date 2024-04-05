using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using MudBlazor;
using YoutubeLinks.Blazor.Auth;
using YoutubeLinks.Blazor.Clients;
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

namespace YoutubeLinks.Blazor.Pages.Links
{
    public partial class LinksPage : ComponentBase
    {
        private List<BreadcrumbItem> _items;
        private MudTable<LinkDto> _table;

        private bool _disableDownloadPlaylistLinkButtons = false;
        private bool _tableView = true;
        private bool _isUserPlaylist = false;

        private string _searchString = "";
        private readonly GetAllPaginatedLinks.Query _query = new()
        {
            Page = 1,
            PageSize = 10,
            SortColumn = "modified",
            SortOrder = SortOrder.Descending,
            SearchTerm = "",
        };
        private PagedList<LinkDto> _linkPagedList;
        private readonly CreateLink.Command _createLinkCommand = new()
        {
            Url = "",
        };

        [Parameter] public int UserId { get; set; }
        [Parameter] public int PlaylistId { get; set; }

        [Inject] public IExceptionHandler ExceptionHandler { get; set; }
        [Inject] public ILinkApiClient LinkApiClient { get; set; }
        [Inject] public IPlaylistApiClient PlaylistApiClient { get; set; }

        [Inject] public IAuthService AuthService { get; set; }
        [Inject] public ITableViewProvider TableViewProvider { get; set; }
        [Inject] public IStringLocalizer<App> Localizer { get; set; }

        [Inject] public IDialogService DialogService { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            _items =
            [
                new(Localizer[nameof(AppStrings.Users)], href: $"/users"),
                new(Localizer[nameof(AppStrings.Playlists)], href: $"/playlists/{UserId}"),
                new(Localizer[nameof(AppStrings.Links)], href: null, disabled: true),
            ];

            _createLinkCommand.PlaylistId = PlaylistId;
            _query.PlaylistId = PlaylistId;

            _tableView = await TableViewProvider.GetTableView();
            _isUserPlaylist = await AuthService.IsLoggedInUser(UserId);

            await RefreshView();
        }

        private async Task RefreshView()
        {
            if (_tableView)
                await _table.ReloadServerData();
            else
                await ReloadLinks();
        }

        private async Task<TableData<LinkDto>> ServerReload(TableState state)
        {
            try
            {
                _query.Page = state.Page + 1;
                _query.PageSize = state.PageSize;
                _query.SortColumn = state.SortLabel;
                _query.SortOrder = (SortOrder)state.SortDirection;
                _query.SearchTerm = _searchString;

                _linkPagedList = await LinkApiClient.GetAllPaginatedLinks(_query);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleExceptions(ex);
                return new() { TotalItems = 0, Items = [] };
            }

            return new()
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

                _linkPagedList = await LinkApiClient.GetAllPaginatedLinks(_query);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleExceptions(ex);
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
            await TableViewProvider.SetTableView(_tableView);

            _query.Page = 1;
            await RefreshView();
        }

        private async Task DeletePlaylistLink(int id)
        {
            var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
            var dialog = await DialogService.ShowAsync<DeleteDialog>(Localizer[nameof(AppStrings.Delete)], options);

            var result = await dialog.Result;
            if (!result.Canceled)
            {
                try
                {
                    await LinkApiClient.DeleteLink(id);
                    await RefreshView();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.HandleExceptions(ex);
                }
            }
        }

        private async Task UpdatePlaylistLink(LinkDto linkDto)
        {
            var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
            var parameters = new DialogParameters<UpdateLinkDialog>
            {
                {
                    x => x.Command,
                    new()
                    {
                        Id = linkDto.Id,
                        Url = linkDto.Url,
                        Title = linkDto.Title,
                        Downloaded = linkDto.Downloaded,
                    }
                }
            };

            var dialog = await DialogService.ShowAsync<UpdateLinkDialog>(Localizer[nameof(AppStrings.UpdateLink)], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
                await RefreshView();
        }

        private async Task DownloadPlaylistLink(int id, YoutubeFileType youtubeFileType)
        {
            try
            {
                _disableDownloadPlaylistLinkButtons = true;

                var command = new DownloadLink.Command
                {
                    Id = id,
                    YoutubeFileType = youtubeFileType,
                };

                var response = await LinkApiClient.DownloadLink(command);
                var content = await response.Content.ReadAsByteArrayAsync();
                var filename = response.Content.Headers.ContentDisposition.FileNameStar ?? $"default_name.{YoutubeHelpers.YoutubeFileTypeToString(command.YoutubeFileType)}";

                await JSRuntime.InvokeVoidAsync("downloadFile", filename, content);

                //var command2 = new UpdateLink.Command         // update downloaded flag feature
                //{
                //    Id = id,
                //    Url = filename,
                //    Downloaded = true,
                //};

                //await LinkApiClient.UpdateLink(command2);

                await RefreshView();
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleExceptions(ex);
            }
            finally
            {
                _disableDownloadPlaylistLinkButtons = false;
            }
        }

        private async Task ResetPlaylistLinksDownloadedFlag(bool flag)
        {
            var dialogText = flag ? Localizer[nameof(AppStrings.SetAllPlaylistLinksAsDownloaded)] : 
                Localizer[nameof(AppStrings.SetAllPlaylistLinksAsUndownloaded)];

            var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
            var dialog = await DialogService.ShowAsync<InformationDialog>(dialogText, options);

            var result = await dialog.Result;
            if (!result.Canceled)
            {
                try
                {
                    var command = new ResetLinksDownloadedFlag.Command
                    {
                        Id = PlaylistId,
                        IsDownloaded = flag,
                    };

                    await PlaylistApiClient.ResetLinksDownloadedFlag(command);
                    await RefreshView();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.HandleExceptions(ex);
                }
            }
        }

        private static string ConvertToEmbedUrl(string url)
            => url.Replace("/watch?v=", "/embed/");
    }
}