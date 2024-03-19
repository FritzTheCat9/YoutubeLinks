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
using YoutubeLinks.Shared.Features.Playlists.Queries;
using YoutubeLinks.Shared.Features.Playlists.Responses;

namespace YoutubeLinks.Blazor.Pages.Playlists
{
    public partial class PlaylistsPage : ComponentBase
    {
        private List<BreadcrumbItem> _items;
        private MudTable<PlaylistDto> _table;

        private bool _isUserPlaylist = false;

        private string _searchString = "";
        private PagedList<PlaylistDto> _playlistPagedList;

        [Parameter] public int UserId { get; set; }

        [Inject] public IExceptionHandler ExceptionHandler { get; set; }
        [Inject] public IPlaylistApiClient PlaylistApiClient { get; set; }

        [Inject] public IAuthService AuthService { get; set; }
        [Inject] public IStringLocalizer<App> Localizer { get; set; }

        [Inject] public IDialogService DialogService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }


        protected override async void OnParametersSet()
        {
            _items =
            [
                new(Localizer[nameof(AppStrings.Users)], href: $"/users"),
                new(Localizer[nameof(AppStrings.Playlists)], href: null, disabled: true),
            ];

            _isUserPlaylist = await AuthService.IsLoggedInUser(UserId);
        }

        private async Task<TableData<PlaylistDto>> ServerReload(TableState state)
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
                return new() { TotalItems = 0, Items = [] };
            }

            return new()
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
            var parameters = new DialogParameters<UpdatePlaylistDialog>();
            parameters.Add(x => x.Command, new()
            {
                Id = playlistDto.Id,
                Name = playlistDto.Name,
                Public = playlistDto.Public,
            });

            var dialog = await DialogService.ShowAsync<UpdatePlaylistDialog>(Localizer[nameof(AppStrings.UpdatePlaylist)], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
                await _table.ReloadServerData();
        }

        private async Task CreateUserPlaylist()
        {
            var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
            var parameters = new DialogParameters<CreatePlaylistDialog>();
            parameters.Add(x => x.Command, new()
            {
                Name = "",
                Public = true,
            });

            var dialog = await DialogService.ShowAsync<CreatePlaylistDialog>(Localizer[nameof(AppStrings.CreatePlaylist)], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
                await _table.ReloadServerData();
        }

        private async Task ExportPlaylistToFile(int id, ExportPlaylist.PlaylistFileType playlistFileType)
        {
            try
            {
                var command = new ExportPlaylist.Command
                {
                    Id = id,
                    PlaylistFileType = playlistFileType,
                };
                string fileExtension = GetPlaylistFileType(playlistFileType);

                var response = await PlaylistApiClient.ExportPlaylist(command);
                var content = await response.Content.ReadAsByteArrayAsync();
                var filename = response.Content.Headers.ContentDisposition.FileNameStar ?? $"default_name.{fileExtension}";

                await JSRuntime.InvokeVoidAsync("downloadFile", filename, content);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleExceptions(ex);
            }
        }

        private async Task ImportPlaylistFromJSON()
        {
            var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
            var parameters = new DialogParameters<ImportPlaylistDialog>();
            parameters.Add(x => x.FormModel, new()
            {
                Name = "",
                Public = true,
                ExportedLinks = [],
                File = null,
            });

            var dialog = await DialogService.ShowAsync<ImportPlaylistDialog>(Localizer[nameof(AppStrings.ImportPlaylist)], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
                await _table.ReloadServerData();
        }

        private static string GetPlaylistFileType(ExportPlaylist.PlaylistFileType playlistFileType)
        {
            return playlistFileType switch
            {
                ExportPlaylist.PlaylistFileType.TXT => "txt",
                _ => "json",
            };
        }

        private void RedirectToLinksPage(int id)
            => NavigationManager.NavigateTo($"/links/{UserId}/{id}");

        private void RedirectToDownloadPlaylistPage(int id)
            => NavigationManager.NavigateTo($"/downloadPlaylist/{id}");
    }
}