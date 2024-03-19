using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using YoutubeLinks.Blazor.Clients;
using YoutubeLinks.Blazor.Exceptions;
using YoutubeLinks.Blazor.Localization;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Users.Helpers;
using YoutubeLinks.Shared.Features.Users.Queries;
using YoutubeLinks.Shared.Features.Users.Responses;

namespace YoutubeLinks.Blazor.Pages.Users
{
    public partial class UsersPage : ComponentBase
    {
        private List<BreadcrumbItem> _items;
        private MudTable<UserDto> _table;

        private string _searchString = "";
        private PagedList<UserDto> _myUsers;

        [Inject] public IExceptionHandler ExceptionHandler { get; set; }
        [Inject] public IUserApiClient UserApiClient { get; set; }

        [Inject] public IStringLocalizer<App> Localizer { get; set; }

        [Inject] public IDialogService DialogService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        protected override void OnParametersSet()
        {
            _items =
            [
                new(Localizer[nameof(AppStrings.Users)], href: null, disabled: true),
            ];
        }

        private async Task<TableData<UserDto>> ServerReload(TableState state)
        {
            var query = new GetAllUsers.Query
            {
                Page = state.Page + 1,
                PageSize = state.PageSize,
                SortColumn = state.SortLabel,
                SortOrder = ((SortOrder)state.SortDirection),
                SearchTerm = _searchString,
            };

            try
            {
                _myUsers = await UserApiClient.GetAllUsers(query);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleExceptions(ex);
                return new() { TotalItems = 0, Items = [] };
            }

            return new()
            {
                TotalItems = _myUsers.TotalCount,
                Items = _myUsers.Items
            };
        }

        private static string GetUserHighestPolicy(UserDto user) 
            => user.IsAdmin ? Policy.Admin : Policy.User;

        private static Color GetColorBasedOnPolicy(string policy) 
            => policy == Policy.Admin ? Color.Error : Color.Success;

        private void RedirectToUserPlaylistsPage(int id) 
            => NavigationManager.NavigateTo($"/playlists/{id}");
    }
}