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

namespace YoutubeLinks.Blazor.Pages.Users;

public partial class UsersPage(
    IExceptionHandler exceptionHandler,
    IUserApiClient userApiClient,
    IStringLocalizer<App> localizer,
    NavigationManager navigationManager)
    : ComponentBase
{
    private List<BreadcrumbItem> _items;
    private PagedList<UserDto> _myUsers;
    private string _searchString = "";
    private MudTable<UserDto> _table;

    protected override void OnParametersSet()
    {
        _items =
        [
            new BreadcrumbItem(localizer[nameof(AppStrings.Users)], null, true)
        ];
    }

    private async Task<TableData<UserDto>> ServerReload(TableState state, CancellationToken token)
    {
        var query = new GetAllUsers.Query
        {
            Page = state.Page + 1,
            PageSize = state.PageSize,
            SortColumn = state.SortLabel,
            SortOrder = (SortOrder)state.SortDirection,
            SearchTerm = _searchString
        };

        try
        {
            _myUsers = await userApiClient.GetAllUsers(query);
        }
        catch (Exception ex)
        {
            exceptionHandler.HandleExceptions(ex);
            return new TableData<UserDto> { TotalItems = 0, Items = [] };
        }

        return new TableData<UserDto>
        {
            TotalItems = _myUsers.TotalCount,
            Items = _myUsers.Items
        };
    }

    private static string GetUserHighestPolicy(UserDto user)
    {
        return user.IsAdmin ? Policy.Admin : Policy.User;
    }

    private static Color GetColorBasedOnPolicy(string policy)
    {
        return policy == Policy.Admin ? Color.Error : Color.Success;
    }

    private void RedirectToUserPlaylistsPage(int id)
    {
        navigationManager.NavigateTo($"/playlists/{id}");
    }

    public abstract class UsersPageConst
    {
        public const string SearchInput = "users-page-search-input";
        public const string UserNameTableSortLabel = "users-page-username-table-sort-label";
        public const string EmailTableSortLabel = "users-page-email-table-sort-label";
        public const string UserNameTableRowData = "users-page-username-table-row-data";
        public const string NavigateToUserPlaylistsButton = "users-page-navigate-to-user-playlists-button";
    }
}